using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Usr;
using Testflow.Usr.Common;

namespace Testflow.ComInterfaceManager
{
    public class AssemblyDescriptionLoader : MarshalByRefObject
    {
        private readonly HashSet<string> _ignoreMethod;
        private readonly HashSet<string> _ignoreClass;
        private readonly Dictionary<string, Assembly> _assemblies;

        private readonly HashSet<string> _simpleValueType;
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
            _assemblies = new Dictionary<string, Assembly>(100);
            _simpleValueType = new HashSet<string>();
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (string)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (long)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (ulong)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (int)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (uint)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (short)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (ushort)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (byte)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (char)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (decimal)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (DateTime)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (double)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (float)));
            _simpleValueType.Add(ModuleUtils.GetFullName(typeof (bool)));
        }

        public Exception Exception { get; private set; }

        public int ErrorCode { get; set; }

        public override object InitializeLifetimeService()
        {
            // 定义该对象的声明周期为无限生存期
            return null;
        }

        public ComInterfaceDescription LoadAssemblyDescription(string path, ref string assemblyName, ref string version)
        {
            Exception = null;
            ErrorCode = 0;
            Assembly assembly;
            if (!_assemblies.ContainsKey(assemblyName))
            {
                if (!File.Exists(path))
                {
                    this.ErrorCode = ModuleErrorCode.LibraryNotFound;
                    return null;
                }
                try
                {
                    assembly = Assembly.LoadFrom(path);
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
                if (!_assemblies.ContainsKey(assemblyName))
                {
                    _assemblies.Add(assemblyName, assembly);
                }
            }
            else
            {
                assembly = _assemblies[assemblyName];
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
                    VariableType classKind = GetKindOfType(typeInfo);
                    if (classKind == VariableType.Enumeration || classKind == VariableType.Value)
                    {
                        AddDataTypeDescription(descriptionData, typeInfo, assemblyName, classKind);
                    }
                    else if (classKind == VariableType.Class || classKind == VariableType.Struct)
                    {
                        AddClassDescription(descriptionData, typeInfo, assemblyName, classKind);
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

        private void AddDataTypeDescription(ComInterfaceDescription descriptionData, Type classType, string assemblyName, VariableType classKind)
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
                Name = GetTypeName(classType),
                Namespace = GetNamespace(classType),
                Kind = classKind
            };

            // 枚举类型需要添加枚举值到类型信息中
            if (classType.IsEnum)
            {
                typeDescription.Enumerations = Enum.GetNames(classType);
            }

            descriptionData.TypeDescriptions.Add(typeDescription);
        }

        private void AddClassDescription(ComInterfaceDescription comDescription, Type classType, string assemblyName, VariableType classKind)
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
                Name = GetTypeName(classType),
                Namespace = GetNamespace(classType),
                Description = classDescriptionStr,
                Kind = classKind
            };
            ClassInterfaceDescription classDescription = new ClassInterfaceDescription()
            {
                ClassTypeDescription = typeDescription,
                Description = typeDescription.Description,
                Name = GetTypeName(classType),
            };
            AddConstructorDescription(classType, classDescription, classKind);
            AddPropertySetterDescription(classType, classDescription);
            AddFieldSetterDescription(classType, classDescription);
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

        private void AddFieldSetterDescription(Type classType, ClassInterfaceDescription classDescription)
        {
            List<IArgumentDescription> staticProperties = GetFieldDescriptions(classType, BindingFlags.Static | BindingFlags.Public);
            if (null != staticProperties && staticProperties.Count > 0)
            {
                FunctionInterfaceDescription staticSetterDesp = new FunctionInterfaceDescription()
                {
                    Name = CommonConst.SetStaticFieldFunc,
                    Arguments = staticProperties,
                    ClassType = classDescription.ClassType,
                    ComponentIndex = classDescription.ComponentIndex,
                    FuncType = FunctionType.StaticFieldSetter,
                    Signature = CommonConst.SetStaticFieldFunc + "()",
                    Return = null,
                    IsGeneric = false
                };
                classDescription.Functions.Add(staticSetterDesp);
            }

            List<IArgumentDescription> instanceProperties = GetFieldDescriptions(classType, BindingFlags.Instance | BindingFlags.Public);
            if (null != instanceProperties && instanceProperties.Count > 0)
            {
                FunctionInterfaceDescription instanceSetterDesp = new FunctionInterfaceDescription()
                {
                    Name = CommonConst.SetInstanceFieldFunc,
                    Arguments = instanceProperties,
                    ClassType = classDescription.ClassType,
                    ComponentIndex = classDescription.ComponentIndex,
                    FuncType = FunctionType.InstanceFieldSetter,
                    Signature = CommonConst.SetInstanceFieldFunc + "()",
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
                funcDescription.Signature = ModuleUtils.GetSignature(classDescription.Name, funcDescription);

                classDescription.Functions.Add(funcDescription);
            }
        }

        // 初始化构造方法的入参
        private void AddConstructorDescription(Type classType, ClassInterfaceDescription classDescription, VariableType classKind)
        {
            if (classKind == VariableType.Struct)
            {
                AddStructDefaultConstructor(classType, classDescription);
            }
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

        // 添加Struct默认构造函数
        private static void AddStructDefaultConstructor(Type classType, ClassInterfaceDescription classDescription)
        {
            string descriptionStr = "Struct default constructor";
            FunctionInterfaceDescription funcDescription = new FunctionInterfaceDescription()
            {
                Category = classDescription.ClassTypeDescription.Category,
                Description = descriptionStr,
                FuncType = FunctionType.StructConstructor,
                IsGeneric = false,
                Name = $"{classType.Name}_Constructor"
            };
            funcDescription.Arguments = new List<IArgumentDescription>(1);
            funcDescription.Return = null;
            funcDescription.Signature = ModuleUtils.GetSignature(classType.Name, funcDescription);
            classDescription.Functions.Add(funcDescription);
        }

        private void InitConstructorParamDescription(ConstructorInfo constructor,
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
        private List<IArgumentDescription> GetPropertyDescriptions(Type classType, BindingFlags flags)
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
                Type propertyType = propertyInfo.PropertyType;

                DescriptionAttribute descriptionAttribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
                string descriptionStr = (null == descriptionAttribute) ? string.Empty : descriptionAttribute.Description;

                TypeDescription typeDescription = new TypeDescription()
                {
                    AssemblyName = propertyType.Assembly.GetName().Name,
                    Category = string.Empty,
                    Description = descriptionStr,
                    Name = GetTypeName(propertyType),
                    Namespace = GetNamespace(propertyType)
                };

                ArgumentDescription propertyDescription = new ArgumentDescription()
                {
                    Name = propertyInfo.Name,
                    ArgumentType = GetKindOfType(propertyType),
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

        // 构造属性描述
        private List<IArgumentDescription> GetFieldDescriptions(Type classType, BindingFlags flags)
        {
            FieldInfo[] fieldInfos = classType.GetFields(flags);
            List<IArgumentDescription> fields = new List<IArgumentDescription>(fieldInfos.Length);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Type fieldType = fieldInfo.FieldType;
                DescriptionAttribute descriptionAttribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                string descriptionStr = (null == descriptionAttribute) ? string.Empty : descriptionAttribute.Description;

                TypeDescription typeDescription = new TypeDescription()
                {
                    AssemblyName = fieldType.Assembly.GetName().Name,
                    Category = string.Empty,
                    Description = descriptionStr,
                    Name = GetTypeName(fieldType),
                    Namespace = GetNamespace(fieldType)
                };

                ArgumentDescription propertyDescription = new ArgumentDescription()
                {
                    Name = fieldInfo.Name,
                    ArgumentType = GetKindOfType(fieldType),
                    Description = descriptionStr,
                    Modifier = ArgumentModifier.None,
                    DefaultValue = string.Empty,
                    TypeDescription = typeDescription,
                    IsOptional = false
                };
                fields.Add(propertyDescription);
            }
            return fields;
        }

        // 构造方法入参描述信息
        private void InitMethodParamDescription(MethodInfo method, 
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
        private ArgumentDescription GetParameterInfo(ParameterInfo parameterInfo)
        {
            Type parameterType = parameterInfo.ParameterType;
            DescriptionAttribute descriptionAttribute = parameterInfo.GetCustomAttribute<DescriptionAttribute>();
            string descriptionStr = (null == descriptionAttribute) ? string.Empty : descriptionAttribute.Description;

            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = parameterType.Assembly.GetName().Name,
                Category = string.Empty,
                Description = descriptionStr,
                Name = GetParamTypeName(parameterType),
                Namespace = GetNamespace(parameterType)
            };

            ArgumentModifier modifier = ArgumentModifier.None;
            if (parameterInfo.ParameterType.IsByRef)
            {
                modifier = parameterInfo.IsOut ? ArgumentModifier.Out : ArgumentModifier.Ref;
            }
            ArgumentDescription paramDescription = new ArgumentDescription()
            {
                Name = parameterInfo.Name,
                ArgumentType = GetKindOfType(parameterType),
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
        private ArgumentDescription GetReturnInfo(ParameterInfo parameterInfo)
        {
            Type propertyType = parameterInfo.ParameterType;
            if ("Void".Equals(propertyType.Name))
            {
                return null;
            }

            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = propertyType.Assembly.GetName().Name,
                Category = string.Empty,
                Description = string.Empty,
                Name = GetTypeName(propertyType),
                Namespace = GetNamespace(propertyType)
            };

            ArgumentDescription paramDescription = new ArgumentDescription()
            {
                Name = string.Empty,
                ArgumentType = GetKindOfType(propertyType),
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

        public ITypeDescription GetPropertyType(string assemblyName, string typeName, string propertyStr)
        {
            const char delim = '.';
            string[] propertyElems = propertyStr.Split(delim);
            Exception = null;
            ErrorCode = 0;
            if (!_assemblies.ContainsKey(assemblyName))
            {
                this.ErrorCode = ModuleErrorCode.AssemblyNotLoad;
                return null;
            }
            try
            {
                Type dataType = _assemblies[assemblyName].GetType(typeName);
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
                    Name = GetTypeName(dataType),
                    Namespace = GetNamespace(dataType)
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
            _assemblies.Add(mscorAssembly.GetName().Name, mscorAssembly);
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
                Name = GetTypeName(type),
                Namespace = GetNamespace(type)
            };

            ClassInterfaceDescription classDescription = new ClassInterfaceDescription()
            {
                ClassTypeDescription = typeDescription,
                IsStatic = false,
                Name = typeDescription.Name
            };
            VariableType classType = GetKindOfType(type);
            AddConstructorDescription(type, classDescription, classType);
            AddMethodDescription(type, classDescription);

            comDescription.Classes.Add(classDescription);
            comDescription.TypeDescriptions.Add(typeDescription);
        }

        private static string GetTypeName(Type type)
        {
            const char delim = '.';
            // 如果DeclaringType为空则该类不是nestedType，直接返回名称
            if (null == type.DeclaringType)
            {
                return type.Name;
            }
            // 如果是nested类型，则其名称应该为：上级类类名.本类类名
            Type nestedType = type;
            StringBuilder typeName = new StringBuilder(50);
            typeName.Append(type.Name);
            while (null != nestedType.DeclaringType)
            {
                nestedType = nestedType.DeclaringType;
                typeName.Insert(0, delim).Insert(0, nestedType.Name);
            }
            return typeName.ToString();
        }

        // 部分类型支持原生的ref类型type，这在slave端会导致识别的困难，需要将其替换为非ref类型。例如所有基础类型和数组都有原生的ref类型
        private static string GetParamTypeName(Type type)
        {
            const string refTypeSymbol = "&";
            string typeName = GetTypeName(type);
            return !typeName.Contains(refTypeSymbol) ? typeName : typeName.Replace(refTypeSymbol, "");
        }

        // 获取类型对应的种类
        private VariableType GetKindOfType(Type realType)
        {
            VariableType argumentType = VariableType.Undefined;
            // 枚举类型
            if (realType.IsEnum)
            {
                argumentType = VariableType.Enumeration;
            }
            // 简单值类型
            else if (_simpleValueType.Contains(ModuleUtils.GetFullName(realType)))
            {
                argumentType = VariableType.Value;
            }
            // Class类型
            else if (realType.IsClass || realType.IsInterface)
            {
                argumentType = VariableType.Class;
            }
            // 非简单类型的值类型被判定为Struct类型
            else if (realType.IsValueType)
            {
                argumentType = VariableType.Struct;
            }
            return argumentType;
        }

        private static string GetNamespace(Type type)
        {
            return type.Namespace;
//            if (null == type.DeclaringType)
//            {
//                return type.Namespace;
//            }
//            Type nestedType = type;
//            while (null != nestedType.DeclaringType)
//            {
//                nestedType = nestedType.DeclaringType;
//            }
//            return $"{nestedType.Namespace}.{nestedType.Name}";
        }
    }
}