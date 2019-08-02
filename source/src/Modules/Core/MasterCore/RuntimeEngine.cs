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


        private readonly RuntimeStatusManager _statusManager;
        private readonly SynchronousManager _syncManager;
        private readonly EngineFlowController _controller;
        private readonly RuntimeObjectManager _runtimeObjectManager;
        private readonly CallBackProcessor _callBackProcessor;

        public RuntimeEngine(IModuleConfigData configData)
        {
            _globalInfo = new ModuleGlobalInfo(configData);
            bool isSyncMessenger = _globalInfo.ConfigData.GetProperty<bool>("EngineSyncMessenger");
            // TODO 暂时写死使用LocalTestMaintainer
            // 初始化消息收发器
            MessageTransceiver messageTransceiver = MessageTransceiver.GetTransceiver(_globalInfo, isSyncMessenger);
            
            _controller = new EngineFlowController(_globalInfo);
            _statusManager = new RuntimeStatusManager(_globalInfo);
            _syncManager = new SynchronousManager(_globalInfo);
            _callBackProcessor = new CallBackProcessor(_globalInfo);

            _globalInfo.RuntimeInitialize(messageTransceiver, _controller.Debugger);

            _runtimeObjectManager = new RuntimeObjectManager();

            RuntimeStateMachine stateMachine = new RuntimeStateMachine();
            _globalInfo.StateMachine = stateMachine;
            
            RegisterMessageHandler();

            _globalInfo.LogService.Print(LogLevel.Info, CommonConst.PlatformLogSession, "RuntimeEngine constructed.");
        }

        public RuntimeStatusManager StatusManager => _statusManager;
        public SynchronousManager SyncManager => _syncManager;
        public EngineFlowController Controller => _controller;
        public CallBackProcessor CallBackProcessor => _callBackProcessor;

        public ModuleGlobalInfo GlobalInfo => _globalInfo;

        public RuntimeType RuntimeType => _globalInfo.ConfigData.GetProperty<RuntimeType>("RuntimeType");

        private void RegisterMessageHandler()
        {
            //RmtGen消息由远端接收，所以无需分发
            _controller.RegisterMessageHandler();
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Status.ToString(), _statusManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.TestGen.ToString(), _statusManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Sync.ToString(), _syncManager);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.CallBack.ToString(), _callBackProcessor);
        }

        public void Initialize(ISequenceFlowContainer sequenceContainer)
        {
            try
            {
                _globalInfo.StateMachine.State = RuntimeState.Idle;
                _controller.Initialize(sequenceContainer);
                _statusManager.Initialize(sequenceContainer);
                _callBackProcessor.Initialize(sequenceContainer);
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
                _runtimeObjectManager.RegisterCustomer<BreakPointObject>(_controller.Debugger);
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
                _controller.StartTestWork();
                _controller.WaitForTaskOver();
            }
            catch (TestflowException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, "Start engine internal error.");
                _globalInfo.StateMachine.State = RuntimeState.Error;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
            }
            catch (ApplicationException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, "Start engine runtime error.");
                _globalInfo.StateMachine.State = RuntimeState.Error;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Start engine fatal error.");
                _globalInfo.StateMachine.State = RuntimeState.Collapsed;
                _globalInfo.ExceptionManager.Append(ex);
                // for test
                throw;
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
            object infoValue = null;
            int session = 0;
            int sequenceIndex;
            switch (infoName)
            {
                case Constants.RuntimeStateInfo:
                    if (extraParams.Length == 0)
                    {
                        infoValue = _globalInfo.StateMachine.State;
                    }
                    else if (extraParams.Length == 1)
                    {
                        session = (int)extraParams[0];
                        infoValue = _statusManager[session].State;
                    }
                    else if (extraParams.Length == 2)
                    {
                        session = (int)extraParams[0];
                        sequenceIndex = (int)extraParams[1];
                        infoValue = _statusManager[session][sequenceIndex].State;
                    }
                    break;
                case Constants.ElapsedTimeInfo:
                    if (extraParams.Length == 1)
                    {
                        session = (int)extraParams[0];
                        infoValue = _statusManager[session].ElapsedTime.TotalMilliseconds;
                    }
                    else if (extraParams.Length == 2)
                    {
                        session = (int)extraParams[0];
                        sequenceIndex = (int)extraParams[1];
                        infoValue = _statusManager[session][sequenceIndex].ElapsedTime.TotalMilliseconds;
                    }
                    break;
                default:
                    _globalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                        $"Unsupported runtime object type: {0}.");
                    _globalInfo.ExceptionManager.Append(new TestflowDataException(
                        ModuleErrorCode.InvalidRuntimeInfoName,
                        _globalInfo.I18N.GetFStr("InvalidRuntimeInfoName", infoName)));
                    break;
            }
            return (TDataType) infoValue;
        }

        public long AddRuntimeObject(string objectType, int sessionId, params object[] param)
        {
            long objectId = Constants.InvalidObjectId;
            switch (objectType)
            {
                case Constants.BreakPointObjectName:
                    BreakPointObject breakPointObject = new BreakPointObject((CallStack)param[0]);
                    objectId = breakPointObject.Id;
                    _runtimeObjectManager.AddObject(breakPointObject);
                    break;
                default:
                    _globalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 
                        $"Unsupported runtime object type: {0}.");
                    _globalInfo.ExceptionManager.Append(new TestflowDataException(
                        ModuleErrorCode.InvalidRuntimeObjectType,
                        _globalInfo.I18N.GetFStr("InvalidRuntimeObjType", objectType)));
                    break;
            }
            return objectId;
        }

        public long RemoveRuntimeObject(int objectId, params object[] param)
        {
            if (null == _runtimeObjectManager[objectId])
            {
                return Constants.InvalidObjectId;
            }
            _runtimeObjectManager.RemoveObject(objectId);
            return objectId;
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
                session = (int) extraParams[0];
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