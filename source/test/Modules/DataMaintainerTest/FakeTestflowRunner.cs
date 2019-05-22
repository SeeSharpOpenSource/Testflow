using System;
using System.Collections.Generic;
using Moq;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Logger;
using Testflow.Modules;

namespace Testflow.DataMaintainerTest
{
    public class FakeTestflowRunner : TestflowRunner
    {
        public FakeTestflowRunner(TestflowRunnerOptions options) : base(options)
        {
        }

        public void SetLogService(ILogService logService)
        {
            this.LogService = logService;
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
            public string LibraryType { get; set; }
        }
    }
}