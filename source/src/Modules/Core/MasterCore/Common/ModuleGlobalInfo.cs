using System;
using System.Text;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.MasterCore.Core;
using Testflow.MasterCore.Message;
using Testflow.MasterCore.StatusManage;
using Testflow.Modules;
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

        public LocalEventQueue<EventInfoBase> EventQueue { get; private set; }

        public RuntimeStateMachine StateMachine { get; set; }

        public ExceptionManager ExceptionManager { get; private set; }

        public EventDispatcher EventDispatcher { get; set; }

        public string RuntimeHash{ get; }

        public ModuleGlobalInfo(IModuleConfigData configData)
        {
            TestflowRunner = TestflowRunner.GetInstance();
            this.I18N = I18N.GetInstance(Constants.I18nName);
            this.LogService = TestflowRunner.LogService;
            this.ConfigData = configData;
            this.ExceptionManager = new ExceptionManager(LogService);
            this.RuntimeHash = ModuleUtils.GetRuntimeHash(configData.GetProperty<Encoding>("PlatformEncoding"));
        }

        public void RuntimeInitialize(MessageTransceiver messageTransceiver)
        {
            this.MessageTransceiver = messageTransceiver;
            this.EventQueue = new LocalEventQueue<EventInfoBase>(Constants.DefaultEventsQueueSize);
        }



        public void Dispose()
        {
        }
    }
}
