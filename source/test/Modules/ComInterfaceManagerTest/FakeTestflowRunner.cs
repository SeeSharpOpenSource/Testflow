using System.Collections.Generic;
using Testflow.ComInterfaceManager;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Logger;
using Testflow.Modules;

namespace Testflow.ComInterfaceManagerTest
{
    public class FakeTestflowRunner : TestflowRunner
    {
        public FakeTestflowRunner(TestflowRunnerOptions options) : base(options)
        {
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
            this.LogService = new LogService();
            this.SequenceManager = new SequenceManager.SequenceManager();

            this.ComInterfaceManager = new InterfaceManager();

            ModuleConfigData configData = new ModuleConfigData();
            configData.InitExtendProperties();

            SequenceManager.ApplyConfig(configData);
            LogService.ApplyConfig(configData);

            SequenceManager.DesigntimeInitialize();
            LogService.RuntimeInitialize();
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
            public string Category { get; set; }
        }
    }
}