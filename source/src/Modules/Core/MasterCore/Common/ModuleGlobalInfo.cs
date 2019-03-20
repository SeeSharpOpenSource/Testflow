using System;
using System.Threading;
using Testflow.CoreCommon.Data;
using Testflow.MasterCore.Core;
using Testflow.MasterCore.Message;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Utility.I18nUtil;

namespace Testflow.MasterCore.Common
{
    internal class ModuleGlobalInfo : IDisposable
    {
        public IModuleConfigData ConfigData { get; set; }

        public I18N I18N { get; }

        public ILogService LogService { get; }

        public TestflowRunner TestflowRunner { get; }

        public MessageTransceiver MessageTransceiver { get; private set; }

        public EventQueue EventQueue { get; private set; }

        public RuntimeStateMachine StateMachine { get; set; }

        public ModuleGlobalInfo(IModuleConfigData configData)
        {
            TestflowRunner = TestflowRunner.GetInstance();
            this.I18N = I18N.GetInstance(Constants.I18nName);
            this.LogService = TestflowRunner.LogService;
            this.ConfigData = configData;
        }

        public void RuntimeInitialize(MessageTransceiver messageTransceiver)
        {
            this.MessageTransceiver = messageTransceiver;
            this.EventQueue = new EventQueue();
        }

        public void Dispose()
        {
        }
    }
}
