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

        private readonly MessageTransceiver _messageTransceiver;

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

            _messageTransceiver = MessageTransceiver.GetTransceiver(_globalInfo, isSyncMessenger);

            _controller = new EngineFlowController(_globalInfo);
            _statusManager = new RuntimeStatusManager(_globalInfo);
            _debugManager = new DebugManager(_globalInfo);
            _syncManager = new SynchronousManager(_globalInfo);
            InitializeMessageConsumers();
        }

        private void InitializeMessageConsumers()
        {
            //RmtGen消息由远端接收，所以无需分发
            _messageTransceiver.AddConsumer(MessageType.Ctrl.ToString(), _controller);
            _messageTransceiver.AddConsumer(MessageType.Status.ToString(), _statusManager);
            _messageTransceiver.AddConsumer(MessageType.TestGen.ToString(), _statusManager);
            _messageTransceiver.AddConsumer(MessageType.Debug.ToString(), _debugManager);
            _messageTransceiver.AddConsumer(MessageType.Sync.ToString(), _syncManager);
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