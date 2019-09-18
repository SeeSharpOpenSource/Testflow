using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Modules;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.ParameterCheckerTest
{
    class FakeTestflowRunner:TestflowRunner
    {
       
        public FakeTestflowRunner(TestflowRunnerOptions options) : base(options)
        {
           
        }

        public override void Initialize()
        {
            ModuleConfigData configData = new ModuleConfigData();
            configData.InitExtendProperties();

            this.SequenceManager = new SequenceManager.SequenceManager();
            this.ComInterfaceManager = new ComInterfaceManager.InterfaceManager();
            this.ParameterChecker = new ParameterChecker.ParameterChecker();
           
            this.LogService = new Logger.LogService();

            ComInterfaceManager.ApplyConfig(configData);
            SequenceManager.ApplyConfig(configData);
            ParameterChecker.ApplyConfig(configData);

            ComInterfaceManager.DesigntimeInitialize();
            SequenceManager.DesigntimeInitialize();
            LogService.RuntimeInitialize();
        }

        public override void Dispose()
        {
        }

        //假的组件描述信息类
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
            public IDictionary<string, string[]> Enumerations { get; set; }
        }
    }
}
