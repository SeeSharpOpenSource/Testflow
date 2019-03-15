using Testflow.Data.Sequence;
using Testflow.EngineCore.Common;
using Testflow.EngineCore.Core;
using Testflow.EngineCore.Data;
using Testflow.EngineCore.Debugger;
using Testflow.EngineCore.Events;
using Testflow.EngineCore.Generators;
using Testflow.EngineCore.Message;
using Testflow.EngineCore.StatusManage;
using Testflow.EngineCore.SyncManage;
using Testflow.Modules;

namespace Testflow.EngineCore
{
    internal class RuntimeEngine
    {
        private readonly ModuleGlobalInfo _globalInfo;

        private readonly MessageTransceiver _messageTransceiver;

        private readonly RuntimeStatusManager _statusManager;
        private readonly DebugManager _debugManager;
        private readonly ITestGenerator _generator;
        private readonly SynchronousManager _syncManager;
        private EngineFlowController _controller;

        public RuntimeEngine(IModuleConfigData configData)
        {
            _globalInfo = new ModuleGlobalInfo(configData);
            bool isSyncMessenger = _globalInfo.ConfigData.GetProperty<bool>("EngineSyncMessenger");
            _messageTransceiver = MessageTransceiver.GetTransceiver(_globalInfo, isSyncMessenger);

            _controller = new EngineFlowController(_globalInfo);
            _statusManager = new RuntimeStatusManager(_globalInfo);
            _debugManager = new DebugManager(_globalInfo);
            // TODO Generator的生成代码
            _syncManager = new SynchronousManager(_globalInfo);
            InitializeMessageConsumers();
        }

        private void InitializeMessageConsumers()
        {
            //RmtGen消息由远端接收，所以无需分发
            _messageTransceiver.AddConsumer(MessageType.Ctrl.ToString(), _controller);
            _messageTransceiver.AddConsumer(MessageType.Status.ToString(), _statusManager);
            _messageTransceiver.AddConsumer(MessageType.TestGen.ToString(), _generator);
            _messageTransceiver.AddConsumer(MessageType.Debug.ToString(), _debugManager);
            _messageTransceiver.AddConsumer(MessageType.Sync.ToString(), _syncManager);
        }

        public void Initialize(ISequenceFlowContainer sequenceContainer)
        {
            
        }

        public void Clear()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {
            
        }

    }
}