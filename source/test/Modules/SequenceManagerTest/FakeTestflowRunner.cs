using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManagerTest
{
    public class FakeTestflowRunner : TestflowRunner
    {
        public FakeTestflowRunner(TestflowRunnerOptions options) : base(options)
        {
            Mock<IComInterfaceManager> mock = new Mock<IComInterfaceManager>();
            this.ComInterfaceManager = mock.Object;
            Type intType = typeof(int);
            AddMockTypeData(mock, intType.Name, intType.Namespace, intType.Assembly.GetName().Name);
            AddMockTypeData(mock, "ArgumentDemo", "Testflow.Test", "Assembly3");
            AddMockTypeData(mock, "Algorithm", "Testflow.Test", "TestAssemblyName");
            AddMockTypeData(mock, "Double", "System", "TestAssemblyName");

            AddMockAssemblies(mock, "TestAssemblyName", Environment.CurrentDirectory + @"\Test\SequenceGroup1\TestDemoPath");
            AddMockAssemblies(mock, "Assembly3", Environment.CurrentDirectory + @"\Test\SequenceGroup1\TestDemoPath");
            AddMockAssemblies(mock, "mscorlib", Environment.CurrentDirectory + @"\Test\SequenceGroup1\TestDemoPath");
        }

        private void AddMockTypeData(Mock<IComInterfaceManager> mockObj, string typeName, string namespaceStr, string assemblyName)
        {
            mockObj.Setup(m => m.GetTypeByName(typeName, namespaceStr)).Returns(new TypeData()
            {
                AssemblyName = assemblyName,
                Name = typeName,
                Namespace = namespaceStr
            });
        }

        private void AddMockAssemblies(Mock<IComInterfaceManager> mockObj, string assemblyName, string path)
        {
            AssemblyInfo assemblyInfo = new AssemblyInfo()
            {
                AssemblyName = assemblyName,
                Path = path,
                Version = "1.0.2",
                Available = true

            };
            mockObj.Setup(m => m.GetAssemblyInfo(assemblyName)).Returns(assemblyInfo);
        }

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }

        public class FakeComInterface : IComInterfaceDescription
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int ComponentId { get; set; }
            public string Signature { get; }
            public IAssemblyInfo Assembly { get; set; }
            public IList<IClassInterfaceDescription> Classes { get; }
            public IList<ITypeData> VariableTypes { get; set; }
            public IList<ITypeDescription> TypeDescriptions { get; set; }
            public string Category { get; set; }
            public IDictionary<string, string[]> Enumerations { get; set; }
        }
    }
}