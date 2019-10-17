using System;
using System.Text;
using System.Threading;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.MasterCore.Core;
using Testflow.MasterCore.EventData;
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

        /// <summary>
        /// 事件分发器，由StateManageContext创建并初始化到GlobalInfo
        /// </summary>
        public EventDispatcher EventDispatcher { get; set; }

        public DebuggerHandle DebugHandle { get; private set; }

        public string RuntimeHash{ get; }

        /// <summary>
        /// 测试开始阻塞器，在序列生成状态处理结束前阻塞，结束后释放
        /// </summary>
        public ManualResetEventSlim TestGenBlocker { get; }

        public ModuleGlobalInfo(IModuleConfigData configData)
        {
            TestflowRunner = TestflowRunner.GetInstance();
            this.I18N = I18N.GetInstance(Constants.I18nName);
            this.LogService = TestflowRunner.LogService;
            this.ConfigData = configData;
            this.ExceptionManager = new ExceptionManager(LogService);
            this.RuntimeHash = ModuleUtils.GetRuntimeHash(configData.GetProperty<Encoding>("PlatformEncoding"));
            this.TestGenBlocker = new ManualResetEventSlim(false);
        }

        public void RuntimeInitialize(MessageTransceiver messageTransceiver, DebugManager debugManager)
        {
            this.MessageTransceiver = messageTransceiver;
            this.EventQueue = new LocalEventQueue<EventInfoBase>(Constants.DefaultEventsQueueSize);
            this.DebugHandle = new DebuggerHandle(debugManager);
        }

        public void Dispose()
        {
            MessageTransceiver?.Dispose();
            TestGenBlocker?.Dispose();
        }
    }
}
