using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Logger;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.SequenceManager.Serializer.Convertor;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.Serializer
{
    public static class XmlReaderHelper
    {
        public static TestProject ReadTestProject(string filePath)
        {
            if (!filePath.EndsWith($".{CommonConst.TestGroupFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(SequenceManagerErrorCode.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            XmlReader reader = null;
            try
            {
                reader = CreateXmlReader(filePath);
                Dictionary<string, Type> typeMapping = GetTypeMapping();
                // 找到TestProject节点后跳出
                while (reader.Read())
                {
                    if (XmlNodeType.Element != reader.NodeType)
                    {
                        continue;
                    }
                    if (typeof (TestProject).Name.Equals(reader.Name))
                    {
                        break;
                    }
                }
                TestProject testProject = new TestProject();
                FillDataToObject(reader, testProject, typeMapping);
                return testProject;
            }
            catch (ArgumentException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex);
                throw new TestflowDataException(SequenceManagerErrorCode.DeSerializeFailed, ex.Message, ex);
            }
            catch (IOException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex);
                throw new TestflowRuntimeException(SequenceManagerErrorCode.DeSerializeFailed, ex.Message, ex);
            }
            finally
            {
                reader?.Close();
            }
        }


        public static SequenceGroup ReadSequenceGroup(string filePath)
        {
            if (!filePath.EndsWith($".{CommonConst.SequenceFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(SequenceManagerErrorCode.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            XmlReader reader = null;
            try
            {
                reader = CreateXmlReader(filePath);
                Dictionary<string, Type> typeMapping = GetTypeMapping();
                // 找到TestProject节点后跳出
                while (reader.Read())
                {
                    if (XmlNodeType.Element != reader.NodeType)
                    {
                        continue;
                    }
                    if (typeof(SequenceGroup).Name.Equals(reader.Name))
                    {
                        break;
                    }
                }
                SequenceGroup sequenceGroup = new SequenceGroup();
                FillDataToObject(reader, sequenceGroup, typeMapping);
                return sequenceGroup;
            }
            catch (ArgumentException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex);
                throw new TestflowDataException(SequenceManagerErrorCode.DeSerializeFailed, ex.Message, ex);
            }
            catch (IOException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex);
                throw new TestflowRuntimeException(SequenceManagerErrorCode.DeSerializeFailed, ex.Message, ex);
            }
            finally
            {
                reader?.Close();
            }
        }

        public static SequenceGroupParameter ReadSequenceGroupParameter(string filePath)
        {
            if (!filePath.EndsWith($".{CommonConst.SequenceDataFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(SequenceManagerErrorCode.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            XmlReader reader = null;
            try
            {
                reader = CreateXmlReader(filePath);
                Dictionary<string, Type> typeMapping = GetTypeMapping();
                // 找到TestProject节点后跳出
                while (reader.Read())
                {
                    if (XmlNodeType.Element != reader.NodeType)
                    {
                        continue;
                    }
                    if (typeof(SequenceGroupParameter).Name.Equals(reader.Name))
                    {
                        break;
                    }
                }
                SequenceGroupParameter parameter = new SequenceGroupParameter();
                FillDataToObject(reader, parameter, typeMapping);
                return parameter;
            }
            catch (ArgumentException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex);
                throw new TestflowDataException(SequenceManagerErrorCode.DeSerializeFailed, ex.Message, ex);
            }
            catch (IOException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex);
                throw new TestflowRuntimeException(SequenceManagerErrorCode.DeSerializeFailed, ex.Message, ex);
            }
            finally
            {
                reader?.Close();
            }
        }

        // 处理一个类的所有属性反序列化
        private static void FillDataToObject(XmlReader reader, object objectData, Dictionary<string, Type> typeMapping)
        {
            int currentDepth = reader.Depth;
            string nodeName = reader.Name;
            Type dataType = typeMapping[objectData.GetType().Name];
            if (reader.HasAttributes)
            {
                foreach (PropertyInfo propertyInfo in dataType.GetProperties())
                {
                    string propertyName = propertyInfo.PropertyType.Name;
                    Type propertyType = typeMapping.ContainsKey(propertyName) ? 
                        typeMapping[propertyName] : propertyInfo.PropertyType;
                    SerializationIgnoreAttribute ignoreAttribute =
                        propertyInfo.GetCustomAttribute<SerializationIgnoreAttribute>();
                    GenericCollectionAttribute collectionAttribute =
                        propertyType.GetCustomAttribute<GenericCollectionAttribute>();
                    // 值类型、字符串、枚举类型，并且未被ignore的非集合属性直接用值转换器处理
                    if ((propertyInfo.PropertyType.IsValueType || typeof (string).Equals(propertyInfo.PropertyType) ||
                         propertyInfo.PropertyType.IsEnum) && null == ignoreAttribute && null == collectionAttribute)
                    {
                        string attribute = reader.GetAttribute(propertyInfo.Name);
                        if (null == attribute)
                        {
                            continue;
                        }
                        FillValueToObject(propertyInfo, objectData, attribute);
                    }
                }
            }
            int propertyNodeDepth = currentDepth + 1;
            while (reader.Read())
            {
                // xml层级等于或者小于父节点时应该交给上一层的调用处理
                if (reader.Depth <= currentDepth)
                {
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.Read();
                    }
                    break;
                }
                // 如果不是节点或者是空节点或者是结束节点，则继续下一行读取
                if (reader.NodeType != XmlNodeType.Element || (reader.IsEmptyElement && !reader.HasAttributes) || 
                    reader.NodeType == XmlNodeType.EndElement)
                {
                    continue;
                }
                string propertyName = reader.Name;
                PropertyInfo propertyInfo = dataType.GetProperty(propertyName,
                    BindingFlags.Instance | BindingFlags.Public);
                if (reader.Depth != propertyNodeDepth || null == propertyInfo || 
                    !typeMapping.ContainsKey(propertyInfo.PropertyType.Name))
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(SequenceManagerErrorCode.DeSerializeFailed, 
                        i18N.GetStr("IllegalFileData"));
                }
                object propertyObject = CreateTypeInstance(typeMapping, propertyInfo.PropertyType.Name);
                propertyInfo.SetValue(objectData, propertyObject);
                Type propertyType = typeMapping[propertyInfo.PropertyType.Name];
                GenericCollectionAttribute collectionAttribute =
                    propertyType.GetCustomAttribute<GenericCollectionAttribute>();
                if (null == collectionAttribute)
                {
                    FillDataToObject(reader, propertyObject, typeMapping);
                }
                else
                {
                    FillDataToCollection(reader, propertyObject, typeMapping, collectionAttribute.GenericType,
                        propertyObject.GetType());
                }
            }
        }

        private static void FillDataToCollection(XmlReader reader, object objectData, Dictionary<string, Type> typeMapping, Type elementType, Type parentType)
        {
            const string addMethodName = "Add";
            int currentDepth = reader.Depth;
            bool isValueType = elementType.IsValueType || typeof (string).Equals(elementType) || elementType.IsEnum;
//            Type genericType = rawType.GetGenericArguments()[0];
            Type genericType = GetRawGenericElementType(parentType);
            MethodInfo addMethod = parentType.GetMethod(addMethodName, BindingFlags.Instance | BindingFlags.Public, null,
                CallingConventions.Standard, new Type[] { genericType }, new ParameterModifier[0]);
            GenericCollectionAttribute collectionAttribute =
                    elementType.GetCustomAttribute<GenericCollectionAttribute>();
            reader.Read();
            // xml层级等于或者小于父节点时说明集合已经遍历结束，停止遍历
            // Reader的更新在Read
            while (reader.Depth > currentDepth)
            {
                // 如果不是节点或者是空节点或者是结束节点，则继续下一行读取
                if (reader.NodeType != XmlNodeType.Element || (reader.IsEmptyElement && !reader.HasAttributes) ||
                    reader.NodeType == XmlNodeType.EndElement)
                {
                    continue;
                }
                if (isValueType)
                {
                    string value = reader.GetAttribute(Constants.ValueTypeName);
                    if (null != value)
                    {
                        object element = ValueConvertor.ReadData(elementType, value);
                        addMethod.Invoke(objectData, new object[] { element });
                    }
                    // Value模式时需要手动去调整reader的位置
                    reader.Read();
                }
                else
                {
                    // 填充集合或者类时，reader的shift在FillDataToObject和FillDataToCollection中调用
                    object element = CreateTypeInstance(typeMapping, elementType.Name);
                    if (null == collectionAttribute)
                    {
                        FillDataToObject(reader, element, typeMapping);
                    }
                    else
                    {
                        FillDataToCollection(reader, element, typeMapping, collectionAttribute.GenericType, genericType);
                    }
                    addMethod.Invoke(objectData, new object[] { element });
                }
            }
            // 如果是endElement则直接跳过
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                reader.Read();
            }
        }

        // 获取集合类中接口的原始接口类型，例如通过IList<string>的类型获取string类型
        private static Type GetRawGenericElementType(Type collectionType)
        {
            Type[] genericType = null;
            while (null == genericType || 0 == genericType.Length)
            {
                collectionType = collectionType.GetInterfaces()[0];
                genericType = collectionType.GetGenericArguments();
            }
            return genericType[0];
        }

        private static void FillValueToObject(PropertyInfo propertyInfo, object objectData, string attribute)
        {
            Type propertyType = propertyInfo.PropertyType;
            object value = null;
            if (propertyType.IsEnum)
            {
                value = EnumConvertor.ReadData(propertyType, attribute);
            }
            else
            {
                value = ValueConvertor.ReadData(propertyType, attribute);
            }
            if (null != value)
            {
                try
                {
                    propertyInfo.SetValue(objectData, value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static XmlReader CreateXmlReader(string filePath)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.CloseInput = true;
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreWhitespace = true;
                return XmlReader.Create(filePath, settings);
            }
            catch (IOException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex, ex.Message);
                throw new TestflowRuntimeException(SequenceManagerErrorCode.SerializeFailed, ex.Message, ex);
            }
        }

        private static object CreateTypeInstance(Dictionary<string, Type> typeMapping, string typeName)
        {
            Type propertyType = typeMapping[typeName];
            ConstructorInfo info = propertyType.GetConstructor(BindingFlags.Instance | BindingFlags.Public,
                null, CallingConventions.Standard, new Type[0], new ParameterModifier[0]);
            object propertyObject = info.Invoke(null);
            return propertyObject;
        }

        private static Dictionary<string, Type> GetTypeMapping()
        {
            Dictionary<string, Type> typeMapping = new Dictionary<string, Type>(40);
            const string sequenceNamespace = "Testflow.SequenceManager.SequenceElements";
            foreach (Type type in Assembly.GetAssembly(typeof(TestProject)).GetTypes())
            {
                if (sequenceNamespace.Equals(type.Namespace))
                {
                    typeMapping.Add(type.Name, type);
                    // 添加接口类也到映射中
                    typeMapping.Add($"I{type.Name}", type);
                }
            }
            // 部分容器类的接口不是直接在类名前加I，需要特殊处理
            AddSingleTypeMapping(typeMapping, typeof(IList<IArgument>).Name, typeof(ArgumentCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<IAssemblyInfo>).Name, typeof(AssemblyInfoCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<IParameterData>).Name, typeof(ParameterDataCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<IParameterDataCollection>).Name, typeof(ParameterDataCollections));
            AddSingleTypeMapping(typeMapping, typeof(IList<ISequence>).Name, typeof(SequenceCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<ISequenceGroup>).Name, typeof(SequenceGroupCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<SequenceGroupLocationInfo>).Name, typeof(SequenceGroupLocationInfoCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<ISequenceParameter>).Name, typeof(SequenceParameterCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<ISequenceStep>).Name, typeof(SequenceStepCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<ISequenceStepParameter>).Name, typeof(SequenceStepParameterCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<ITypeData>).Name, typeof(TypeDataCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<IVariable>).Name, typeof(VariableCollection));
            AddSingleTypeMapping(typeMapping, typeof(IList<IVariableInitValue>).Name, typeof(VariableInitValueCollection));
            AddSingleTypeMapping(typeMapping, typeof (IList<IArgument>).Name, typeof (ArgumentCollection));
            return typeMapping;
        }

        private static void AddSingleTypeMapping(Dictionary<string, Type> typeMapping, string name, Type type)
        {
            if (typeMapping.ContainsKey(name))
            {
                return;
            }
            typeMapping.Add(name, type);
        }
    }
}