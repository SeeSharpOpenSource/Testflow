using System;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Core;
using Testflow.MasterCore.Message;
using Testflow.MasterCore.ObjectManage;
using Testflow.MasterCore.ObjectManage.Objects;
using Testflow.MasterCore.StatusManage;
using Testflow.MasterCore.SyncManage;
using Testflow.MasterCore.CallBack;
using Testflow.Modules;
using Testflow.Runtime;

namespace Testflow.MasterCore
{
    /// <summary>
    /// 运行时引擎的实例
    /// </summary>
    internal class RuntimeEngine : IDisposable
    {
        private readonly ModuleGlobalInfo _globalInfo;

        private readonly DebugManager _debugManager;

        private readonly RuntimeStatusManager _statusManager;
        private readonly SynchronousManager _syncManager;
        private readonly EngineFlowController _controller;
        private readonly RuntimeObjectManager _runtimeObjectManager;
        private readonly CallBackProcessor _callBackProcessor;
        private readonly RuntimeInfoSelector _runtimeInfoSelector;

        public RuntimeEngine(IModuleConfigData configData)
        {
            _globalInfo = new ModuleGlobalInfo(configData);
            bool isSyncMessenger = _globalInfo.ConfigData.GetProperty<bool>("EngineSyncMessenger");
            // TODO 暂时写死使用LocalTestMaintainer
            // 初始化消息收发器
            MessageTransceiver messageTransceiver = MessageTransceiver.GetTransceiver(_globalInfo, isSyncMessenger);
            messageTransceiver.Clear();
            _controller = new EngineFlowController(_globalInfo);
            _statusManager = new RuntimeStatusManager(_globalInfo);
            _syncManager = new SynchronousManager(_globalInfo);
            _callBackProcessor = new CallBackProcessor(_globalInfo);
            _debugManager = EnableDebug ? new DebugManager(_globalInfo) : null;
            _runtimeInfoSelector = new RuntimeInfoSelector(_globalInfo, this);

            _globalInfo.RuntimeInitialize(messageTransceiver, _debugManager);

            _runtimeObjectManager = new RuntimeObjectManager(_globalInfo);

            RuntimeStateMachine stateMachine = new RuntimeStateMachine();
            _globalInfo.StateMachine = stateMachine;
            
            RegisterMessageHandler();

            _globalInfo.LogService.Print(LogLevel.Info, CommonConst.PlatformLogSession, "RuntimeEngine constructed.");
        }

        public RuntimeStatusManager StatusManager => _statusManager;
        public SynchronousManager SyncManager => _syncManager;
        public EngineFlowController Controller => _controller;
        public CallBackProcessor CallBackProcessor => _callBackProcessor;
        public RuntimeObjectManager RuntimeObjManager => _runtimeObjectManager;
        public DebugManager Debugger => _debugManager;

        public ModuleGlobalInfo GlobalInfo => _globalInfo;

        public bool EnableDebug => RuntimeType.Debug == RuntimeType;

        public RuntimeType RuntimeType => _globalInfo.ConfigData.GetProperty<RuntimeType>("RuntimeType");

        private void RegisterMessageHandler()
        {
            //RmtGen消息由远端接收，所以无需分发
            _controller.RegisterMessageHandler();
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Status.ToString(), _statusManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.TestGen.ToString(), _statusManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Sync.ToString(), _syncManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.CallBack.ToString(), _callBackProcessor);
            if (EnableDebug)
            {
                _globalInfo.MessageTransceiver.AddConsumer(MessageType.Debug.ToString(), _debugManager);
            }
        }

        public void Initialize(ISequenceFlowContainer sequenceContainer)
        {
            try
            {
                _globalInfo.StateMachine.State = RuntimeState.Idle;
                _controller.Initialize(sequenceContainer);
                _statusManager.Initialize(sequenceContainer);
                _callBackProcessor.Initialize(sequenceContainer);
                _debugManager?.Initialize(sequenceContainer);
                // 注册状态更新事件
                _globalInfo.StateMachine.StateAbort += Stop;
                _globalInfo.StateMachine.StateError += Stop;
                _globalInfo.StateMachine.StateCollapsed += Stop;
                _globalInfo.StateMachine.StateOver += Stop;

                // 注册Session结束后自动释放host的事件
                _globalInfo.EventDispatcher.Register(new RuntimeDelegate.SessionStatusAction(
                    (testResult) =>
                    {
                       _controller.TestMaintainer.FreeHost(testResult.Session); 
                    }), CommonConst.BroadcastSession, Constants.SessionOver);
                // 注册运行时对象消费者
                _runtimeObjectManager.RegisterCustomer<BreakPointObject>(_debugManager);
                _runtimeObjectManager.RegisterCustomer<WatchDataObject>(_debugManager);
                _runtimeObjectManager.RegisterCustomer<EvaluationObject>(_debugManager);
            }
            catch (TestflowException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, "Initialize internal error.");
                _globalInfo.StateMachine.State = RuntimeState.Error;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
            }
            catch (ApplicationException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, "Initialize runtime error.");
                _globalInfo.StateMachine.State = RuntimeState.Error;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Initialize fatal error.");
                _globalInfo.StateMachine.State = RuntimeState.Collapsed;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
            }
        }

        public void Start()
        {
            try
            {
                _globalInfo.MessageTransceiver.Activate();
                _statusManager.Start();
                _syncManager.Start();
                bool executionSuccess = _controller.StartTestGeneration();
                if (!executionSuccess)
                {
                    return;
                }
                // 如果使用调试模式，则需要更新所有session的断点和watch信息
                if (EnableDebug)
                {
                    foreach (int session in _controller.TestMaintainer.TestContainers.Keys)
                    {
                        _debugManager.SendDebugWatchAndBreakPointMessage(session);
                    }
                }
                _controller.StartTestWork();
                _controller.WaitForTaskOver();
            }
            catch (TestflowException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex,
                    "Start engine internal error.");
                _globalInfo.StateMachine.State = RuntimeState.Error;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
            }
            catch (ApplicationException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex,
                    "Start engine runtime error.");
                _globalInfo.StateMachine.State = RuntimeState.Error;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex,
                    "Start engine fatal error.");
                _globalInfo.StateMachine.State = RuntimeState.Collapsed;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public void AbortRuntime(int session)
        {
            _controller.Abort(session);
        }

        public void Stop()
        {
            try
            {
                _controller?.Stop();
                _syncManager?.Stop();
                _statusManager?.Stop();
                _globalInfo.MessageTransceiver.Deactivate();
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Stop engine failed.");
                _globalInfo.StateMachine.State = RuntimeState.Collapsed;
                _globalInfo.ExceptionManager.Append(ex);
            }
            finally
            {
                Dispose();
            }
        }

        public void FreeTests()
        {
            _controller.TestMaintainer.FreeHosts();
        }

        public void Dispose()
        {
            _controller.Dispose();
            _syncManager.Dispose();
            _statusManager.Dispose();
            _globalInfo.Dispose();
        }

        #region 处理外部接口

        public RuntimeState GetRuntimeState(int sessionId)
        {
            return _statusManager.GetRuntimeState(sessionId);
        }

        public TDataType GetComponent<TDataType>(string componentName, params object[] extraParams)
        {
            throw new NotImplementedException();
        }

        public TDataType GetRuntimeInfo<TDataType>(string infoName, params object[] extraParams)
        {
            return (TDataType) _runtimeInfoSelector.GetRuntimeInfo(infoName, extraParams);
        }

        public void RegisterRuntimeEvent(Delegate callBack, string eventName, params object[] extraParams)
        {
            //判断委托是否为空。用户可能并没有在外部注册事件，详见runtimeservice里的事件
            if(callBack == null)
            {
                return;
            }

            int session = CommonConst.BroadcastSession;
            //extraParams里只有一个int sessionid，表示发给这个session
            if (0 != extraParams.Length)
            {
                session = Int32.Parse(extraParams[0].ToString());
            }
            _globalInfo.EventDispatcher.Register(callBack, session, eventName);
        }

        public void UnregisterRuntimeEvent(Delegate callBack, string eventName, params object[] extraParams)
        {
            _globalInfo.EventDispatcher.Unregister(callBack, eventName);
        }

        #endregion

    }
}