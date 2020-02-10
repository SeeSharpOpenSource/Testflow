using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Testflow.Usr;
using Testflow.Logger;
using Testflow.Modules;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.Serializer
{
    public static class XmlWriterHelper
    {
        public static void Write(object dataValue, string filePath)
        {
            XmlWriter writer = null;
            try
            {
                writer = CreateXmlWriter(filePath);
                //写入xml头
                writer.WriteStartDocument();
                WriteClassData(dataValue.GetType().Name, dataValue, writer);
                writer.WriteEndDocument();
            }
            finally
            {
                ReleaseXmlWriter(writer);
            }
        }

        private static void WriteClassData(string dataName, object dataValue, XmlWriter writer)
        {
            // 如果属性为null则返回不写入内容
            if (null == dataValue)
            {
                return;
            }

            writer.WriteStartElement(dataName);
            Type dataType = dataValue.GetType();
            List<PropertyInfo> collectionProperties = new List<PropertyInfo>(10);
            List<Type> collectionElemType = new List<Type>(10);
            List<PropertyInfo> classProperties = new List<PropertyInfo>(10);
            SerializationOrderEnableAttribute orderEnableAttr = dataType.GetCustomAttribute<SerializationOrderEnableAttribute>();
            PropertyInfo[] properties = dataType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in properties)
            {
                object propertyValue = propertyInfo.GetValue(dataValue);
                if (null == propertyValue)
                {
                    continue;
                }
                Type propertyType = propertyValue.GetType();
                XmlIgnoreAttribute xmlIgnoreAttribute = propertyInfo.GetCustomAttribute<XmlIgnoreAttribute>();
                SerializationIgnoreAttribute ignoreAttribute = propertyInfo.GetCustomAttribute<SerializationIgnoreAttribute>();
                GenericCollectionAttribute collectionAttribute = propertyType.GetCustomAttribute<GenericCollectionAttribute>();
                if (null != xmlIgnoreAttribute || (null != ignoreAttribute && ignoreAttribute.Ignore))
                {
                    continue;
                }
                // 集合类型的数据和类类型的数据在后面写入
                if (null != collectionAttribute)
                {
                    collectionProperties.Add(propertyInfo);
                    collectionElemType.Add(collectionAttribute.GenericType);
                }
                else if (propertyType.IsClass && !typeof(string).Equals(propertyType))
                {
                    classProperties.Add(propertyInfo);
                }
                else if (propertyType.IsEnum)
                {
                    WriteValueData(propertyInfo.Name, propertyValue.ToString(), writer);
                }
                else
                {
                    WriteValueData(propertyInfo.Name, propertyValue, writer);
                }
            }
            // 如果该对象需要对属性进行排序写入，则先写入已配置顺序的属性
            if (null != orderEnableAttr && orderEnableAttr.OrderEnable)
            {
                WriteOrderEnabledProperties(dataValue, writer, collectionProperties, collectionElemType, classProperties);
            }
            // 写入类信息
            foreach (PropertyInfo propertyInfo in classProperties)
            {
                WriteClassData(propertyInfo.Name, propertyInfo.GetValue(dataValue), writer);
            }
            // 写入集合信息
            for (int i = 0; i < collectionProperties.Count; i++)
            {
                WriteCollectionData(collectionProperties[i].Name, collectionProperties[i].GetValue(dataValue),
                    collectionElemType[i], writer);
            }

            writer.WriteEndElement();
        }

        private static void WriteValueData(string propertyName, object propertyValue, XmlWriter writer)
        {
            writer.WriteStartAttribute(propertyName);
            writer.WriteValue(propertyValue);
            writer.WriteEndAttribute();
        }

        private static void WriteOrderEnabledProperties(object dataValue, XmlWriter writer,
            List<PropertyInfo> collectionProperties, List<Type> collectionElemType, List<PropertyInfo> classProperties)
        {
            int capacity = collectionProperties.Count + classProperties.Count;
            // 按照顺序排序的缓存，从最后写入的到最先写入进行排序
            List<PropertyInfo> orderedProperties = new List<PropertyInfo>(capacity);
            List<Type> orderedElemTypes = new List<Type>(capacity);
            List<int> orderIndex = new List<int>(capacity);
            // 将配置了排序的类属性移出类属性列表，并按照顺序插入新的缓存
            for (int i = classProperties.Count - 1; i >= 0; i--)
            {
                PropertyInfo classProperty = classProperties[i];
                SerializationOrderAttribute orderAttr = classProperty.GetCustomAttribute<SerializationOrderAttribute>();
                if (null != orderAttr && orderAttr.Order >= 0)
                {
                    classProperties.RemoveAt(i);
                    int insertIndex = FindInsertIndex(orderIndex, orderAttr.Order);
                    orderedProperties.Insert(insertIndex, classProperty);
                    orderedElemTypes.Insert(insertIndex, null);
                    orderIndex.Insert(insertIndex, orderAttr.Order);
                }
            }
            // 将配置了排序的集合属性移出集合属性列表，并按照顺序插入新的缓存
            for (int i = collectionProperties.Count - 1; i >= 0; i--)
            {
                PropertyInfo collectionProperty = collectionProperties[i];
                SerializationOrderAttribute orderAttr = collectionProperty.GetCustomAttribute<SerializationOrderAttribute>();
                if (null != orderAttr && orderAttr.Order >= 0)
                {
                    Type elemType = collectionElemType[i];
                    collectionProperties.RemoveAt(i);
                    collectionElemType.RemoveAt(i);
                    int insertIndex = FindInsertIndex(orderIndex, orderAttr.Order);
                    orderedProperties.Insert(insertIndex, collectionProperty);
                    orderedElemTypes.Insert(insertIndex, elemType);
                    orderIndex.Insert(insertIndex, orderAttr.Order);
                }
            }
            // 按照排序后的缓存依次写入属性
            for (int i = orderedProperties.Count - 1; i >= 0; i--)
            {
                PropertyInfo property = orderedProperties[i];
                Type elementType = orderedElemTypes[i];
                if (null == elementType)
                {
                    WriteClassData(property.Name, property.GetValue(dataValue), writer);
                }
                else
                {
                    WriteCollectionData(property.Name, property.GetValue(dataValue), elementType, writer);
                }
            }
        }

        private static int FindInsertIndex(List<int> orderIndex, int order)
        {
            int insertIndex = 0;
            for (int i = orderIndex.Count - 1; i >= 0; i--)
            {
                if (order <= orderIndex[i])
                {
                    return i + 1;
                }
            }
            return insertIndex;
        }

        private static void WriteCollectionData(string dataName, object dataValue, Type elemType, XmlWriter writer)
        {
            IEnumerable dataCollection = dataValue as IEnumerable;
            if (null == dataCollection)
            {
                return;
            }
            writer.WriteStartElement(dataName);
            if (elemType.IsClass && !typeof(string).Equals(elemType))
            {
                foreach (object itemData in dataCollection)
                {
                    WriteClassData(elemType.Name, itemData, writer);
                }
            }
            else
            {
                foreach (object itemData in dataCollection)
                {
                    WriteValueData(elemType.Name, itemData, writer);
                }
            }
            writer.WriteEndElement();
        }

        private static XmlWriter CreateXmlWriter(string filePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Async = false,
                Indent = true,
                IndentChars = "    ",
                NewLineChars = Environment.NewLine
            };
            try
            {
                return XmlWriter.Create(filePath, settings);
            }
            catch (IOException ex)
            {
                ILogService logService = TestflowRunner.GetInstance().LogService;
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex, ex.Message);
                throw new TestflowRuntimeException(ModuleErrorCode.SerializeFailed, ex.Message, ex);
            }
        }

        private static void ReleaseXmlWriter(XmlWriter writer)
        {
            try
            {
                writer?.Flush();
                writer?.Close();
            }
            catch (IOException ex)
            {
                ILogService logService = TestflowRunner.GetInstance().LogService;
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, ex, ex.Message);
                throw new TestflowRuntimeException(ModuleErrorCode.SerializeFailed, ex.Message, ex);
            }
        }
    }
}