using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Testflow.ComInterfaceManager.Data;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.ComInterfaceManager
{
    internal static class ModuleUtils
    {
        // 使用TypeDescription信息更新VariableTypes和Class中的ClassType信息
        public static void ValidateComDescription(ISequenceManager sequenceManager, ComInterfaceDescription description, 
            DescriptionDataTable descriptionCollection)
        {
            int componentId = description.ComponentId;
            foreach (ITypeDescription typeDescription in description.TypeDescriptions)
            {
                ITypeData classType = GetTypeDataByDescription(sequenceManager, descriptionCollection, typeDescription);
                description.VariableTypes.Add(classType);
                if (null != typeDescription.Enumerations)
                {
                    description.Enumerations.Add(GetFullName(typeDescription), typeDescription.Enumerations);
                }
            }
            description.TypeDescriptions.Clear();
            description.TypeDescriptions = null;
            ((List<ITypeData>)description.VariableTypes).TrimExcess();

            foreach (ClassInterfaceDescription classDescription in description.Classes)
            {
                ITypeData classType = GetTypeDataByDescription(sequenceManager, descriptionCollection,
                    classDescription.ClassTypeDescription);
                classDescription.ClassType = classType;
                classDescription.ClassTypeDescription = null;
            }
            ((List<IClassInterfaceDescription>)description.Classes).TrimExcess();

            I18N i18N = I18N.GetInstance(Constants.I18nName);
            foreach (IClassInterfaceDescription classDescription in description.Classes)
            {
                foreach (IFuncInterfaceDescription functionDescription in classDescription.Functions)
                {
                    // 配置实例属性配置方法和静态属性配置方法的描述信息
                    if (functionDescription.FuncType == FunctionType.InstancePropertySetter)
                    {
                        functionDescription.Description = i18N.GetStr("InstancePropertySetter");
                    }
                    else if (functionDescription.FuncType == FunctionType.StaticPropertySetter)
                    {
                        functionDescription.Description = i18N.GetStr("StaticPropertySetter");
                    }
                    foreach (IArgumentDescription argumentDescription in functionDescription.Arguments)
                    {
                        InitializeArgumentType(sequenceManager, descriptionCollection, argumentDescription);
                    }
                    ((List<IArgumentDescription>) functionDescription.Arguments).TrimExcess();
                    functionDescription.ClassType = classDescription.ClassType;
                    if (null != functionDescription.Return)
                    {
                        InitializeArgumentType(sequenceManager, descriptionCollection, functionDescription.Return);
                    }
                }
            }
        }

        private static ITypeData GetTypeDataByDescription(ISequenceManager sequenceManager,
            DescriptionDataTable descriptionCollection, ITypeDescription typeDescription)
        {
            string classFullName = GetFullName(typeDescription);
            ITypeData classType;
            if (!descriptionCollection.ContainsType(classFullName))
            {
                classType = sequenceManager.CreateTypeData(typeDescription);
                descriptionCollection.AddTypeData(classFullName, classType);
            }
            else
            {
                classType = descriptionCollection.GetTypeData(classFullName);
            }
            return classType;
        }

        private static void InitializeArgumentType(ISequenceManager sequenceManager,
            DescriptionDataTable descriptionCollection, IArgumentDescription argumentDescription)
        {
            ArgumentDescription argDescription = (ArgumentDescription) argumentDescription;
            // 如果类型描述类为空且TypeData非空，则说明该argDescription已经被修改过，无需再执行处理
            if (argDescription.TypeDescription == null)
            {
                if (null == argDescription.Type)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowRuntimeException(ModuleErrorCode.TypeCannotLoad,
                        i18N.GetFStr("InvalidArgType", argDescription.Name));
                }
                return;
            }
            string fullName = GetFullName(argDescription.TypeDescription);
            if (descriptionCollection.ContainsType(fullName))
            {
                argDescription.Type = descriptionCollection.GetTypeData(fullName);
            }
            else
            {
                ITypeData typeData = sequenceManager.CreateTypeData(argDescription.TypeDescription);
                descriptionCollection.AddTypeData(fullName, typeData);
                argDescription.Type = typeData;
            }
            argDescription.TypeDescription = null;
        }

        public static string GetFullName(string namespaceStr, string name)
        {
            const string fullNameFormat = "{0}.{1}";
            return string.Format(fullNameFormat, namespaceStr, name);
        }

        public static string GetFullName(ITypeDescription typeDescription)
        {
            const string fullNameFormat = "{0}.{1}";
            return string.Format(fullNameFormat, typeDescription.Namespace, typeDescription.Name);
        }

        public static string GetFullName(ITypeData typeData)
        {
            const string fullNameFormat = "{0}.{1}";
            return string.Format(fullNameFormat, typeData.Namespace, typeData.Name);
        }

        public static string GetSignature(string className, FunctionInterfaceDescription funcDescription)
        {
            const string signatureFormat = "{0}.{1}({2})";
            StringBuilder paramStr = new StringBuilder(20);
            const string delim = ",";
            foreach (ArgumentDescription argument in funcDescription.Arguments)
            {
                paramStr.Append(argument.TypeDescription.Name).Append(delim);
            }
            if (paramStr.Length > 0)
            {
                paramStr.Remove(paramStr.Length - 1, 1);
            }
            return string.Format(signatureFormat, className, funcDescription.Name, paramStr);
        }

        public static void SetComponentId(ComInterfaceDescription comDescription, int index)
        {
            comDescription.ComponentId = index;
            foreach (IClassInterfaceDescription classDescription in comDescription.Classes)
            {
                classDescription.ComponentIndex = index;
                foreach (IFuncInterfaceDescription funcDescription in classDescription.Functions)
                {
                    funcDescription.ComponentIndex = index;
                }
            }
        }
    }
}