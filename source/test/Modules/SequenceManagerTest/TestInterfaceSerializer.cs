using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManagerTest
{
    [TestClass]
    public class TestSerializerClass
    {
        [TestInitialize]
        public void Setup()
        {
            
        }

        [TestMethod]
        public void TestSerializeInterface()
        {
            Type[] extraType = new Type[] { typeof(TestInterface) };
            XmlSerializer serializer = new XmlSerializer(typeof(TestDataClass), extraType);
            TestDataClass testDataClass = new TestDataClass();
            testDataClass.Data.TestData = "This is Test Data";
            using (FileStream stream = new FileStream(@"D:\test.xml", FileMode.OpenOrCreate))
            {
                serializer.Serialize(stream, testDataClass);
            }
        }

        [TestMethod]
        public void TestCollectionSerialize()
        {
            Type[] extraType = new Type[] { typeof(TestInterface) };
            XmlSerializer serializer = new XmlSerializer(typeof(TestDataCollectionSerialize), extraType);
            TestDataCollectionSerialize testCollection = new TestDataCollectionSerialize();
            testCollection.Data.Add(new TestDataClass() {Data = new TestInterface() {TestData = "1"} });
            testCollection.Data.Add(new TestDataClass() {Data = new TestInterface() {TestData = "2"} });
            testCollection.Data.Add(new TestDataClass() {Data = new TestInterface() {TestData = "3"} });
            testCollection.Data.Add(new TestDataClass() {Data = new TestInterface() {TestData = "4"} });
            testCollection.Data.Add(new TestDataClass() {Data = new TestInterface() {TestData = "5"} });
            using (FileStream stream = new FileStream(@"D:\testcollection.xml", FileMode.OpenOrCreate))
            {
                serializer.Serialize(stream, testCollection);
            }
        }

        [TestMethod]
        public void TestMannualSerialize()
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Async = false,
                Indent = true,
                IndentChars = "    ",
                NewLineChars = Environment.NewLine
            };
            XmlWriter xmlWriter = XmlWriter.Create(@"D:\testMannual.xml", settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Test");
            xmlWriter.WriteStartAttribute("name");
            xmlWriter.WriteValue("TestName");
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteStartElement("book");
            xmlWriter.WriteStartAttribute("attr");
            xmlWriter.WriteValue("attrValue");
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartElement("collectionName");

            xmlWriter.WriteStartElement("book1");
            xmlWriter.WriteStartAttribute("attr1");
            xmlWriter.WriteValue("attrValue1");
            xmlWriter.WriteEndAttribute();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("book2");
            xmlWriter.WriteStartAttribute("attr2");
            xmlWriter.WriteValue("attrValue2");
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();

            AssemblyInfo assemblyInfo1 = new AssemblyInfo();
            AssemblyInfo assemblyInfo2 = new AssemblyInfo();
            AssemblyInfoCollection assemblyInfoCollection = new AssemblyInfoCollection();
            assemblyInfoCollection.Add(assemblyInfo1);
            assemblyInfoCollection.Add(assemblyInfo2);
            object tmpData = assemblyInfoCollection as object;
            Type type = tmpData.GetType();
            GenericCollectionAttribute genericCollectionAttribute = type.GetCustomAttribute<GenericCollectionAttribute>();
            Type genericType = genericCollectionAttribute.GenericType;
            Type[] interfaces = type.GetInterfaces();
            Type listType = typeof(IList<>);
            Type genericTypeData = listType.MakeGenericType(genericType);
            bool isSubclassOf = type.IsSubclassOf(genericTypeData);
        }

        [TestMethod]
        public void TestXmlReader()
        {
//            IList<string> list = new List<string>();
//            Type[] genericArguments = list.GetType().GetGenericArguments();
//            Type elementType = list.GetType();
//            MethodInfo addMethod = elementType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public, null,
//                CallingConventions.Standard, new Type[] { genericArguments[0] }, new ParameterModifier[0]);
//            addMethod.Invoke(list, new object[] { "test" });

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            XmlReader reader = XmlReader.Create(@"D:\testflow\test.tfproj", settings);
            while (reader.Read())
            {
                XmlNodeType xmlNodeType = reader.NodeType;
                string name = reader.Name;
                string value = reader.Value;
                Type valueType = reader.ValueType;
                bool hasAttributes = reader.HasAttributes;
                bool hasValue = reader.HasValue;
                string attribute = reader.GetAttribute("Name");
            }
        }
    }

    [XmlInclude(typeof(TestInterface))]
    public interface ITestInterface
    {
        string TestData { get; set; }
    }

    public class TestInterface : ITestInterface
    {
        public string TestData { get; set; }
    }

    public class TestDataClass
    {
        public TestDataClass()
        {
            this.Data = new TestInterface();
        }
        
        public TestInterface Data { get; set; }
    }

    public class TestDataCollectionSerialize
    {
        public TestDataCollectionSerialize()
        {
            this.Data = new TestDataCollection();
        }

        public TestDataCollection Data { get; set; }
    }

    public class TestDataCollection : IList<TestDataClass>
    {
        private List<TestDataClass> _innerCollection = new List<TestDataClass>(10);
        public IEnumerator<TestDataClass> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TestDataClass item)
        {
            _innerCollection.Add(item);
        }

        public void Clear()
        {
            _innerCollection.Clear();
        }

        public bool Contains(TestDataClass item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(TestDataClass[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TestDataClass item)
        {
            return _innerCollection.Remove(item);
        }

        public int Count => _innerCollection.Count;
        public bool IsReadOnly => false;
        public int IndexOf(TestDataClass item)
        {
            return _innerCollection.IndexOf(item);
        }

        public void Insert(int index, TestDataClass item)
        {
            _innerCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _innerCollection.RemoveAt(index);
        }

        public TestDataClass this[int index]
        {
            get { return _innerCollection[index]; }
            set { _innerCollection[index] = value; }
        }
    }
}