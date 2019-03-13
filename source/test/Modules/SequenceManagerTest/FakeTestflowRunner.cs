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

            AddMockAssemblies(mock, "TestAssemblyName", "TestDemoPath");
            AddMockAssemblies(mock, "Assembly3", "TestDemoPath");
            AddMockAssemblies(mock, "mscorlib", "SystemPath");
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
            mockObj.Setup(m => m.GetComInterfaceByName(assemblyName)).Returns(new FakeComInterface()
            {
                Assembly = assemblyInfo,
                ComponentId = 0,
                Description = "This is test"
            });
        }

        public override IComInterfaceManager ComInterfaceManager { get; protected set; }
        public override IConfigurationManager ConfigurationManager { get; protected set; }
        public override IDataMaintainer DataMaintainer { get; protected set; }
        public override IEngineController EngineController { get; protected set; }
        public override ILogService LogService { get; protected set; }
        public override IParameterChecker ParameterChecker { get; protected set; }
        public override IResultManager ResultManager { get; protected set; }
        public override ISequenceManager SequenceManager { get; protected set; }
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
            public IList<IClassInterfaceDescription> Functions { get; }
            public IList<ITypeData> VariableTypes { get; set; }
        }
    }
}