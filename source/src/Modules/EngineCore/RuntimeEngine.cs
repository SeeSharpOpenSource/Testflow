using System;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.EngineCore.Common;
using Testflow.EngineCore.Core;
using Testflow.EngineCore.Data;
using Testflow.EngineCore.Debugger;
using Testflow.EngineCore.Events;
using Testflow.EngineCore.Message;
using Testflow.EngineCore.StatusManage;
using Testflow.EngineCore.SyncManage;
using Testflow.EngineCore.TestMaintain;
using Testflow.Modules;
using Testflow.Runtime;

namespace Testflow.EngineCore
{
    /// <summary>
    /// 运行时引擎的实例
    /// </summary>
    internal class RuntimeEngine : IDisposable
    {
        private readonly ModuleGlobalInfo _globalInfo;


        private readonly RuntimeStatusManager _statusManager;
        private readonly DebugManager _debugManager;
        private readonly ITestEntityMaintainer _testsMaintainer;
        private readonly SynchronousManager _syncManager;
        private EngineFlowController _controller;

        public RuntimeEngine(IModuleConfigData configData)
        {
            _globalInfo = new ModuleGlobalInfo(configData);
            bool isSyncMessenger = _globalInfo.ConfigData.GetProperty<bool>("EngineSyncMessenger");
            // TODO 暂时写死使用LocalTestMaintainer
            _testsMaintainer = new LocalTestEntityMaintainer(_globalInfo);
            // 初始化消息收发器
            MessageTransceiver messageTransceiver = MessageTransceiver.GetTransceiver(_globalInfo, isSyncMessenger);
            _globalInfo.RuntimeInitialize(messageTransceiver);

            _controller = new EngineFlowController(_globalInfo);
            _statusManager = new RuntimeStatusManager(_globalInfo);
            _debugManager = new DebugManager(_globalInfo);
            _syncManager = new SynchronousManager(_globalInfo);
            InitializeMessageConsumers();
        }

        public RuntimeStatusManager StatusManager => _statusManager;
        public DebugManager DebugManager => _debugManager;
        public ITestEntityMaintainer TestMaintainer => _testsMaintainer;
        public SynchronousManager SyncManager => _syncManager;
        public EngineFlowController Controller => _controller;

        private void InitializeMessageConsumers()
        {
            //RmtGen消息由远端接收，所以无需分发
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Ctrl.ToString(), _controller);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Status.ToString(), _statusManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.TestGen.ToString(), _statusManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Debug.ToString(), _debugManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Sync.ToString(), _syncManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.RmtGen.ToString(), _testsMaintainer);
        }

        public void Initialize(ISequenceFlowContainer sequenceContainer)
        {
            try
            {
                if (sequenceContainer is ITestProject)
                {
                    _testsMaintainer.Generate((ITestProject) sequenceContainer);
                }
                else if (sequenceContainer is ISequenceGroup)
                {
                    _testsMaintainer.Generate((ISequenceGroup) sequenceContainer);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            catch (ApplicationException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex);
                _globalInfo.State = RuntimeState.Error;
                throw;
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex);
                _globalInfo.State = RuntimeState.Error;
                throw;
            }
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

        public void FreeTests()
        {
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}