using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Testflow.Common;
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