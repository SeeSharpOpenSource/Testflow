using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Usr;
using Testflow.Usr.Common;

namespace Testflow.ComInterfaceManager
{
    internal class AssemblyDescriptionLoader : MarshalByRefObject
    {
        public AssemblyDescriptionLoader()
        {

        }

        public Exception Exception { get; private set; }

        public int ErrorCode { get; set; }

        public ComInterfaceDescription LoadAssemblyDescription(IAssemblyInfo assemblyInfo)
        {
            Exception = null;
            ErrorCode = 0;
            if (!File.Exists(assemblyInfo.Path))
            {
                assemblyInfo.Available = false;
                return null;
            }
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(assemblyInfo.Path);
            }
            catch (Exception ex)
            {
                this.Exception = ex;
                this.ErrorCode = ModuleErrorCode.LibraryLoadError;
                assemblyInfo.Available = false;
                return null;
            }

            if (string.IsNullOrWhiteSpace(assemblyInfo.AssemblyName))
            {
                assemblyInfo.AssemblyName = assembly.GetName().Name;
                assemblyInfo.Available = true;
                assemblyInfo.Version = assembly.ImageRuntimeVersion;
            }
            else
            {
                if (!CheckVersion(assemblyInfo.Version, assembly))
                {
                    return null;
                }
            }

            try
            {
                ComInterfaceDescription descriptionData = new ComInterfaceDescription();
                descriptionData.Assembly = assemblyInfo;
                // TODO 加载xml文件注释
                foreach (Type typeInfo in assembly.GetExportedTypes())
                {
                    HideAttribute hideAttribute = typeInfo.GetCustomAttribute<HideAttribute>();
                
                    //如果隐藏属性为true，则隐藏该类型
                    if ((null != hideAttribute && hideAttribute.Hide))
                    {
                        continue;
                    }
                    AddClassDescription(descriptionData, typeInfo);
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

        private void AddClassDescription(ComInterfaceDescription comDescription, Type classType)
        {
            TestflowTypeAttribute testflowType = classType.GetCustomAttribute<TestflowTypeAttribute>();
            TestflowCategoryAttribute category = classType.GetCustomAttribute<TestflowCategoryAttribute>();
            DescriptionAttribute descriptionAttr = classType.GetCustomAttribute<DescriptionAttribute>();
            string typeCategory = null != category ? category.CategoryString : string.Empty;

            string classDescriptionStr = (null != descriptionAttr) ? descriptionAttr.Description : string.Empty;

            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = comDescription.Assembly.AssemblyName,
                Category = typeCategory,
                Name = classType.Name,
                Namespace = classType.Namespace,
                Description = classDescriptionStr,
            };

            if (null != testflowType && !testflowType.IsTestflowDataType)
            {
                comDescription.TypeDescriptions.Add(typeDescription);
            }

            ClassInterfaceDescription classDescription = new ClassInterfaceDescription()
            {
                ClassTypeDescription = typeDescription,
                Description = typeDescription.Description,
                Name = classType.Name,
            };

            AddConstructorDescription(classType, classDescription);
            AddMethodDescription(classType, classDescription);

            comDescription.Classes.Add(classDescription);
        }

        private static void AddMethodDescription(Type classType, ClassInterfaceDescription classDescription)
        {
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public);
            foreach (MethodInfo methodInfo in methods)
            {
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
                List<IArgumentDescription> properties = GetPropertyDescriptions(classType);
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
                    funcDescription.Properties = properties;
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
        private static List<IArgumentDescription> GetPropertyDescriptions(Type classType)
        {
            PropertyInfo[] propertyInfos = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
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
                if (propertyType.IsValueType || propertyType == typeof (string))
                {
                    argumentType = VariableType.Value;
                }
                else if (propertyType.IsClass)
                {
                    argumentType = VariableType.Class;
                }
                else if (propertyType.IsEnum)
                {
                    argumentType = VariableType.Enumeration;
                }

                DescriptionAttribute descriptionAttribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
                string descriptionStr = (null == descriptionAttribute) ? string.Empty : descriptionAttribute.Description;

                TypeDescription typeDescription = new TypeDescription()
                {
                    AssemblyName = propertyType.Assembly.GetName().Name,
                    Category = string.Empty,
                    Description = descriptionStr,
                    Name = propertyInfo.Name,
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
            funcDescription.Return = GetParameterInfo(method.ReturnParameter);
        }

        // 构造一个参数信息
        private static ArgumentDescription GetParameterInfo(ParameterInfo parameterInfo)
        {
            VariableType argumentType = VariableType.Undefined;
            Type propertyType = parameterInfo.ParameterType;
            if (propertyType.IsValueType || propertyType == typeof (string))
            {
                argumentType = VariableType.Value;
            }
            else if (propertyType.IsClass)
            {
                argumentType = VariableType.Class;
            }
            else if (propertyType.IsEnum)
            {
                argumentType = VariableType.Enumeration;
            }

            DescriptionAttribute descriptionAttribute = parameterInfo.GetCustomAttribute<DescriptionAttribute>();
            string descriptionStr = (null == descriptionAttribute) ? string.Empty : descriptionAttribute.Description;

            TypeDescription typeDescription = new TypeDescription()
            {
                AssemblyName = propertyType.Assembly.GetName().Name,
                Category = string.Empty,
                Description = descriptionStr,
                Name = parameterInfo.Name,
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

        private bool CheckVersion(string writeVersion, Assembly assembly)
        {
            const char delim = '.';
            string[] versionElem = writeVersion.Split(delim);
            int major = int.Parse(versionElem[0]);
            int minor = int.Parse(versionElem[1]);
            int revision = versionElem.Length > 2 ? int.Parse(versionElem[2]) : 0;

            Version libVersion = assembly.GetName().Version;
            if (libVersion.Major == major && libVersion.Minor == minor && libVersion.Revision == revision)
            {
                return true;
            }
            if (libVersion.Major > major || libVersion.Minor > minor && libVersion.Revision > revision)
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
    }
}