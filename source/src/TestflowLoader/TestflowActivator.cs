using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.ComInterfaceManager;
using Testflow.ConfigurationManager;
using Testflow.DesigntimeService;
using Testflow.Logger;
using Testflow.MasterCore;
using Testflow.Modules;

namespace Testflow.Loader
{
    public class TestflowActivator : TestflowRunner
    {
        public static TestflowRunner CreateRunner(TestflowRunnerOptions options)
        {
            TestflowActivator activator = new TestflowActivator(options);
            SetInstance(activator);
            return activator;
        }

        private TestflowActivator(TestflowRunnerOptions options) : base(options)
        {
        }

        public override void Initialize()
        {
            switch (Option.Mode)
            {
                case RunMode.Full:
                    ConfigurationManager = new PlatformConfigManager();
                    LogService = new LogService();
                    ParameterChecker = new ParameterChecker.ParameterChecker();
                    ResultManager = new ResultManager.ResultManager();
                    ComInterfaceManager = new InterfaceManager();
                    SequenceManager = new SequenceManager.SequenceManager();
                    DataMaintainer = new DataMaintainer.DataMaintainer();
                    EngineController = new EngineHandle();
                    DesignTimeService = new DesignTimeService();
                    RuntimeService = new RuntimeService.RuntimeService();
                    break;
                case RunMode.Minimal:
                    ConfigurationManager = new PlatformConfigManager();
                    LogService = new LogService();
                    ResultManager = new ResultManager.ResultManager();
                    SequenceManager = new SequenceManager.SequenceManager();
                    DataMaintainer = new DataMaintainer.DataMaintainer();
                    EngineController = new EngineHandle();
                    RuntimeService = new RuntimeService.RuntimeService();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
