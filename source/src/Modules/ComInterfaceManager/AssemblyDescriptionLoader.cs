using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Usr;
using Testflow.Usr.Common;
using Testflow.Utility.I18nUtil;

namespace Testflow.ComInterfaceManager
{
    public class AssemblyDescriptionLoader : MarshalByRefObject
    {
        private readonly HashSet<string> _ignoreMethod;
        private readonly HashSet<string> _ignoreClass;
        public AssemblyDescriptionLoader()
        {
            _ignoreMethod = new HashSet<string>()
            {
                "ToString",
                "Equals",
                "GetHashCode",
                "GetType"
            };

            _ignoreClass = new HashSet<string>()
            {
                "Exception",
                "Log"
            };
        }

        public Exception Exception { get; private set; }

        public int ErrorCode { get; set; }

        public ComInterfaceDescription LoadAssemblyDescription(string path, ref string assemblyName, ref string version)
        {
            Exception = null;
            ErrorCode = 0;
            if (!File.Exists(path))
            {
                this.ErrorCode = ModuleErrorCode.LibraryNotFound;
                return null;
            }
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(path);
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                this.ErrorCode = ModuleErrorCode.LibraryLoadError;
                return null;
            }

            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                assemblyName = assembly.GetName().Name;
                version = assembly.GetName().Version.ToString();
            }
            else
            {
                if (!CheckVersion(version, assembly))
                {
                    return null;
                }
            }
            try
            {
                ComInterfaceDescription descriptionData = new ComInterfaceDescription()
                {
                    Category = string.Empty,
                    Signature = assembly.FullName,
                    Name = assembly.GetName().Name
                };
//                descriptionData.Assembly = assemblyInfo;
                // TODO 加载xml文件注释
                foreach (Type typeInfo in assembly.GetExportedTypes())
                {
                    HideAttribute hideAttribute = typeInfo.GetCustomAttribute<HideAttribute>();
                
                    //如果隐藏属性为true，则隐藏该类型。如果是屏蔽类型，则跳过
                    if ((null != hideAttribute && hideAttribute.Hide) || _ignoreClass.Any(item => typeInfo.Name.EndsWith(item)))
                    {
                        continue;
                    }
                    
                    if (typeInfo.IsEnum || typeInfo.IsValueType || typeInfo == typeof(string))
                    {
                        AddDataTypeDescription(descriptionData, typeInfo, assemblyName);
                    }
                    else if (typeInfo.IsClass)
                    {
                        AddClassDescription(descriptionData, typeInfo, assemblyName);
                    }
                }
                return descriptionData;
            }
            catch (Exception ex)
            {
                Exception = ex;
                this.ErrorCode = ModuleErrorCode.LibraryLoadError;
                return null;
            }
        }

        private void AddDataTypeDescription(ComInterfaceDescription descriptionData, Type classType, string assemblyName)
        {
            TestflowCategoryAttribute category = classType.GetCustomAttribute<TestflowCategoryAttribute>();
            DescriptionAttribute descriptionAttr = classType.GetCustomAttribute<DescriptionAttribute>();

            string typeCategory = null != category ? category.CategoryString : string.Empty;
            string classDescriptionStr = (null != descriptionAttr) ? descriptionAttr.Description : string.Empty;

            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = assemblyName,
                Category = typeCategory,
                Description = classDescriptionStr,
                Name = classType.Name,
                Namespace = classType.Namespace
            };

            // 枚举类型需要添加枚举值到类型信息中
            if (classType.IsEnum)
            {
                typeDescription.Enumerations = Enum.GetNames(classType);
            }

            descriptionData.TypeDescriptions.Add(typeDescription);
        }

        private void AddClassDescription(ComInterfaceDescription comDescription, Type classType, string assemblyName)
        {
            TestflowTypeAttribute testflowType = classType.GetCustomAttribute<TestflowTypeAttribute>();
            TestflowCategoryAttribute category = classType.GetCustomAttribute<TestflowCategoryAttribute>();
            DescriptionAttribute descriptionAttr = classType.GetCustomAttribute<DescriptionAttribute>();
            string typeCategory = null != category ? category.CategoryString : string.Empty;

            string classDescriptionStr = (null != descriptionAttr) ? descriptionAttr.Description : string.Empty;

            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = assemblyName,
                Category = typeCategory,
                Name = classType.Name,
                Namespace = classType.Namespace,
                Description = classDescriptionStr,
            };
            ClassInterfaceDescription classDescription = new ClassInterfaceDescription()
            {
                ClassTypeDescription = typeDescription,
                Description = typeDescription.Description,
                Name = classType.Name,
            };
            AddConstructorDescription(classType, classDescription);
            AddPropertySetterDescription(classType, classDescription);
            AddMethodDescription(classType, classDescription);

            classDescription.IsStatic = classDescription.Functions.All(
                item => item.FuncType != FunctionType.Constructor && item.FuncType != FunctionType.InstanceFunction);

            comDescription.Classes.Add(classDescription);

            // 如果是Testflow数据类型，则添加到数据类型列表中。默认所有的实例类都是Testflow数据类型(包含实例方法，并且有公开的实例属性)
            if (!classDescription.IsStatic && (null == testflowType || testflowType.IsTestflowDataType))
            {
                comDescription.TypeDescriptions.Add(typeDescription);
            }
        }

        private void AddPropertySetterDescription(Type classType, ClassInterfaceDescription classDescription)
        {
            List<IArgumentDescription> staticProperties = GetPropertyDescriptions(classType, BindingFlags.Static | BindingFlags.Public);
            if (null != staticProperties && staticProperties.Count > 0)
            {
                FunctionInterfaceDescription staticSetterDesp = new FunctionInterfaceDescription()
                {
                    Name = CommonConst.SetStaticPropertyFunc,
                    Arguments = staticProperties,
                    ClassType = classDescription.ClassType,
                    ComponentIndex = classDescription.ComponentIndex,
                    FuncType = FunctionType.StaticPropertySetter,
                    Signature = CommonConst.SetStaticPropertyFunc + "()",
                    Return = null,
                    IsGeneric = false
                };
                classDescription.Functions.Add(staticSetterDesp);
            }

            List<IArgumentDescription> instanceProperties = GetPropertyDescriptions(classType, BindingFlags.Instance | BindingFlags.Public);
            if (null != instanceProperties && instanceProperties.Count > 0)
            {
                FunctionInterfaceDescription instanceSetterDesp = new FunctionInterfaceDescription()
                {
                    Name = CommonConst.SetInstancePropertyFunc,
                    Arguments = instanceProperties,
                    ClassType = classDescription.ClassType,
                    ComponentIndex = classDescription.ComponentIndex,
                    FuncType = FunctionType.InstancePropertySetter,
                    Signature = CommonConst.SetInstancePropertyFunc + "()",
                    Return = null,
                    IsGeneric = false
                };
                classDescription.Functions.Add(instanceSetterDesp);
            }
        }

        private void AddMethodDescription(Type classType, ClassInterfaceDescription classDescription)
        {
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            bool isStatic = methods.All(item => item.IsStatic || _ignoreMethod.Contains(item.Name));
            foreach (MethodInfo methodInfo in methods)
            {
                // 忽略object对象继承方法和实例类的getset方法
                if (_ignoreMethod.Contains(methodInfo.Name))
                {
                    continue;
                }
                if (!isStatic && (methodInfo.Name.StartsWith(Constants.GetPrefix) || 
                    methodInfo.Name.StartsWith(Constants.SetPrefix)))
                {
                    continue;
                }
                DescriptionAttribute descriptionAttribute = methodInfo.GetCustomAttribute<DescriptionAttribute>();
                string descriptionStr = (null == descriptionAttribute)
                    ? string.Empty
                    : descriptionAttribute.Description;

                TestflowCategoryAttribute categoryAttribute = methodInfo.GetCustomAttribute<TestflowCategoryAttribute>();
                string categoryStr = null == categoryAttribute
                    ? classDescription.ClassTypeDescription.Category
                    : categoryAttribute.CategoryString;

                FunctionType funcType = FunctionType.InstanceFunction;
                CallBackAttribute callBackAttribute = methodInfo.GetCustomAttribute<CallBackAttribute>();
                AssertionAttribute assertAttribute = methodInfo.GetCustomAttribute<AssertionAttribute>();
                if (null != callBackAttribute)
                {
                    funcType = FunctionType.CallBack;
                }
                else if (null != assertAttribute)
                {
                    funcType = FunctionType.Assertion;
                }
                else if (methodInfo.IsStatic)
                {
                    funcType = FunctionType.StaticFunction;
                }

                FunctionInterfaceDescription funcDescription = new FunctionInterfaceDescription()
                {
                    Category = categoryStr,
                    Description = descriptionStr,
                    FuncType = funcType,
                    IsGeneric = methodInfo.IsGenericMethod,
                    Name = methodInfo.Name,
                };
                InitMethodParamDescription(methodInfo, funcDescription);
                funcDescription.Signature = ModuleUtils.GetSignature(classType.Name, funcDescription);
                classDescription.Functions.Add(funcDescription);
            }
        }

        // 初始化构造方法的入参
        private static void AddConstructorDescription(Type classType, ClassInterfaceDescription classDescription)
        {
            ConstructorInfo[] constructors = classType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructors.Length > 0)
            {
                int constructorIndex = 1;
                foreach (ConstructorInfo constructorInfo in constructors)
                {
                    DescriptionAttribute descriptionAttribute = constructorInfo.GetCustomAttribute<DescriptionAttribute>();
                    string descriptionStr = (null == descriptionAttribute)
                        ? string.Empty
                        : descriptionAttribute.Description;
                    FunctionInterfaceDescription funcDescription = new FunctionInterfaceDescription()
                    {
                        Category = classDescription.ClassTypeDescription.Category,
                        Description = descriptionStr,
                        FuncType = FunctionType.Constructor,
                        IsGeneric = constructorInfo.IsGenericMethod,
                        Name = $"{classType.Name}_Constructor{constructorIndex++}"
                    };
                    InitConstructorParamDescription(constructorInfo, funcDescription);
                    funcDescription.Signature = ModuleUtils.GetSignature(classType.Name, funcDescription);
                    classDescription.Functions.Add(funcDescription);
                }
            }
        }

        private static void InitConstructorParamDescription(ConstructorInfo constructor,
            FunctionInterfaceDescription funcDescription)
        {
            ParameterInfo[] paramInfo = constructor.GetParameters();
            funcDescription.Arguments = new List<IArgumentDescription>(paramInfo.Length);
            foreach (ParameterInfo parameterInfo in paramInfo)
            {
                ArgumentDescription paramDescription = GetParameterInfo(parameterInfo);
                funcDescription.Arguments.Add(paramDescription);
            }
            funcDescription.Return = null;
        }

        // 构造属性描述
        private static List<IArgumentDescription> GetPropertyDescriptions(Type classType, BindingFlags flags)
        {
            PropertyInfo[] propertyInfos = classType.GetProperties(flags);
            List<IArgumentDescription> properties = new List<IArgumentDescription>(propertyInfos.Length);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                // 如果属性没有set方法，则返回
                if (null == propertyInfo.GetSetMethod())
                {
                    continue;
                }
                VariableType argumentType = VariableType.Undefined;
                Type propertyType = propertyInfo.PropertyType;
                if (propertyType.IsEnum)
                {
                    argumentType = VariableType.Enumeration;
                }
                else if (propertyType.IsClass)
                {
                    argumentType = VariableType.Class;
                }
                else if (propertyType.IsValueType || propertyType == typeof(string))
                {
                    argumentType = VariableType.Value;
                }

                DescriptionAttribute descriptionAttribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
                string descriptionStr = (null == descriptionAttribute) ? string.Empty : descriptionAttribute.Description;

                TypeDescription typeDescription = new TypeDescription()
                {
                    AssemblyName = propertyType.Assembly.GetName().Name,
                    Category = string.Empty,
                    Description = descriptionStr,
                    Name = propertyType.Name,
                    Namespace = propertyType.Namespace
                };

                ArgumentDescription propertyDescription = new ArgumentDescription()
                {
                    Name = propertyInfo.Name,
                    ArgumentType = argumentType,
                    Description = descriptionStr,
                    Modifier = ArgumentModifier.None,
                    DefaultValue = string.Empty,
                    TypeDescription = typeDescription,
                    IsOptional = false
                };
                properties.Add(propertyDescription);
            }
            return properties;
        }

        // 构造方法入参描述信息
        private static void InitMethodParamDescription(MethodInfo method, 
            FunctionInterfaceDescription funcDescription)
        {
            ParameterInfo[] paramInfo = method.GetParameters();
            funcDescription.Arguments = new List<IArgumentDescription>(paramInfo.Length);
            foreach (ParameterInfo parameterInfo in paramInfo)
            {
                ArgumentDescription paramDescription = GetParameterInfo(parameterInfo);
                funcDescription.Arguments.Add(paramDescription);
            }
            funcDescription.Return = GetReturnInfo(method.ReturnParameter);
        }

        // 构造参数信息
        private static ArgumentDescription GetParameterInfo(ParameterInfo parameterInfo)
        {
            VariableType argumentType = VariableType.Undefined;
            Type propertyType = parameterInfo.ParameterType;
            if (propertyType.IsEnum)
            {
                argumentType = VariableType.Enumeration;
            }
            else if (propertyType.IsClass)
            {
                argumentType = VariableType.Class;
            }
            else if (propertyType.IsValueType || propertyType == typeof(string))
            {
                argumentType = VariableType.Value;
            }

            DescriptionAttribute descriptionAttribute = parameterInfo.GetCustomAttribute<DescriptionAttribute>();
            string descriptionStr = (null == descriptionAttribute) ? string.Empty : descriptionAttribute.Description;

            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = propertyType.Assembly.GetName().Name,
                Category = string.Empty,
                Description = descriptionStr,
                Name = propertyType.Name,
                Namespace = propertyType.Namespace
            };

            ArgumentModifier modifier = ArgumentModifier.None;
            if (parameterInfo.ParameterType.IsByRef)
            {
                modifier = parameterInfo.IsOut ? ArgumentModifier.Out : ArgumentModifier.Ref;
            }
            ArgumentDescription paramDescription = new ArgumentDescription()
            {
                Name = parameterInfo.Name,
                ArgumentType = argumentType,
                Description = descriptionStr,
                Modifier = modifier,
                DefaultValue = string.Empty,
                TypeDescription = typeDescription,
                IsOptional = parameterInfo.IsOptional
            };
            if (parameterInfo.HasDefaultValue && null != parameterInfo.DefaultValue &&
                paramDescription.ArgumentType == VariableType.Value)
            {
                paramDescription.DefaultValue = parameterInfo.DefaultValue.ToString();
            }
            return paramDescription;
        }

        // 构造一个参数信息
        private static ArgumentDescription GetReturnInfo(ParameterInfo parameterInfo)
        {
            VariableType argumentType = VariableType.Undefined;
            Type propertyType = parameterInfo.ParameterType;
            if ("Void".Equals(propertyType.Name))
            {
                return null;
            }
            if (propertyType.IsEnum)
            {
                argumentType = VariableType.Enumeration;
            }
            else if (propertyType.IsClass)
            {
                argumentType = VariableType.Class;
            }
            else if (propertyType.IsValueType || propertyType == typeof(string))
            {
                argumentType = VariableType.Value;
            }

            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = propertyType.Assembly.GetName().Name,
                Category = string.Empty,
                Description = string.Empty,
                Name = propertyType.Name,
                Namespace = propertyType.Namespace
            };

            ArgumentDescription paramDescription = new ArgumentDescription()
            {
                Name = string.Empty,
                ArgumentType = argumentType,
                Description = string.Empty,
                Modifier = ArgumentModifier.None,
                DefaultValue = string.Empty,
                TypeDescription = typeDescription,
                IsOptional = false,
            };
            return paramDescription;
        }

        private bool CheckVersion(string writeVersion, Assembly assembly)
        {
            const char delim = '.';
            string[] versionElem = writeVersion.Split(delim);
            int major = int.Parse(versionElem[0]);
            int minor = int.Parse(versionElem[1]);
            int build = versionElem.Length >= 3 ? int.Parse(versionElem[2]) : 0;
            int revision = versionElem.Length >= 4 ? int.Parse(versionElem[3]) : 0;

            Version libVersion = assembly.GetName().Version;

            if (libVersion.Major == major && libVersion.Minor == minor && libVersion.Build == build && 
                libVersion.Revision == revision)
            {
                return true;
            }
            long libVersionNum = libVersion.Major*(long)10E9 + libVersion.Minor* (long)10E6 + libVersion.Build* (long)10E3 +
                                 libVersion.Revision;
            long versionNum = major*(long) 10E9 + minor*(long) 10E6 + build*(long) 10E3 + revision;
            if (libVersionNum > versionNum)
            {
                ErrorCode = ModuleErrorCode.HighVersion;
                return true;
            }
            else
            {
                ErrorCode = ModuleErrorCode.LowVersion;
                return false;
            }
        }

        public ITypeDescription GetPropertyType(ITypeData typeData, string propertyStr)
        {
            const char delim = '.';
            string[] propertyElems = propertyStr.Split(delim);
            Exception = null;
            ErrorCode = 0;
            try
            {
                Type dataType = Type.GetType(ModuleUtils.GetFullName(typeData));
                if (null == dataType)
                {
                    ErrorCode = ModuleErrorCode.TypeCannotLoad;
                    return null;
                }
                foreach (string propertyElem in propertyElems)
                {
                    if (string.IsNullOrWhiteSpace(propertyElem))
                    {
                        continue;
                    }
                    PropertyInfo propertyInfo = dataType.GetProperty(propertyElem,
                        BindingFlags.Instance | BindingFlags.Public);
                    if (null == propertyInfo)
                    {
                        ErrorCode = ModuleErrorCode.PropertyNotFound;
                        return null;
                    }
                    dataType = propertyInfo.PropertyType;
                }

                TestflowCategoryAttribute categoryAttribute = dataType.GetCustomAttribute<TestflowCategoryAttribute>();
                string category = null == categoryAttribute ? string.Empty : categoryAttribute.CategoryString;

                DescriptionAttribute descriptionAttribute = dataType.GetCustomAttribute<DescriptionAttribute>();
                string description = null == descriptionAttribute ? string.Empty : descriptionAttribute.Description;

                TypeDescription typeDescription = new TypeDescription()
                {
                    AssemblyName = dataType.Assembly.GetName().Name,
                    Category = category,
                    Description = description,
                    Name = dataType.Name,
                    Namespace = dataType.Namespace
                };
                return typeDescription;
            }
            catch (Exception ex)
            {
                this.ErrorCode = ModuleErrorCode.LibraryLoadError;
                this.Exception = ex;
                return null;
            }
        }

        public ComInterfaceDescription LoadMscorlibDescription()
        {
            Assembly mscorAssembly = typeof(int).Assembly;
            ComInterfaceDescription description = new ComInterfaceDescription()
            {
                Category = LibraryCategory.Platform.ToString(),
                Description = "mscorelib",
                Name = mscorAssembly.GetName().Name,
                Signature = mscorAssembly.GetName().Name,
            };
            List<Type> loadTypes = new List<Type>()
            {
                typeof (object),
                typeof (bool),
                typeof (double),
                typeof (float),
                typeof (long),
                typeof (ulong),
                typeof (int),
                typeof (uint),
                typeof (short),
                typeof (ushort),
                typeof (char),
                typeof (byte),
                typeof (string),
                typeof (Array),
                typeof (IntPtr),
                typeof (UIntPtr)
            };
            foreach (Type loadType in loadTypes)
            {
                LoadTypeDescriptionByType(loadType, description, mscorAssembly);
            }
            return description;
        }

        private void LoadTypeDescriptionByType(Type type, ComInterfaceDescription comDescription, Assembly assembly)
        {
            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = assembly.GetName().Name,
                Category = LibraryCategory.Platform.ToString(),
                Description = "",
                Name = type.Name,
                Namespace = type.Namespace
            };

            ClassInterfaceDescription classDescription = new ClassInterfaceDescription()
            {
                ClassTypeDescription = typeDescription,
                IsStatic = false,
                Name = typeDescription.Name
            };

            AddConstructorDescription(type, classDescription);
            AddMethodDescription(type, classDescription);

            comDescription.Classes.Add(classDescription);
            comDescription.TypeDescriptions.Add(typeDescription);
        }
    }
}