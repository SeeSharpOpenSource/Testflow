using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.Runtime;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.StatusManage
{
    /// <summary>
    /// 运行时所有测试的状态管理
    /// </summary>
    internal class RuntimeStatusManager : IMessageHandler, IDisposable
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private Thread _internalMessageThd;
        private CancellationTokenSource _cancellation;
        private readonly Dictionary<string, Action<EventInfoBase>> _eventProcessActions;
        private readonly Dictionary<int, SessionStateHandle> _sessionStateHandles;
        private StateManageContext _stateManageContext;

        public RuntimeStatusManager(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;

            _eventProcessActions = new Dictionary<string, Action<EventInfoBase>>(5);
            _eventProcessActions.Add(typeof(AbortEventInfo).Name, AbortEventProcess);
            _eventProcessActions.Add(typeof(DebugEventInfo).Name, DebugEventProcess);
            _eventProcessActions.Add(typeof(ExceptionEventInfo).Name, ExceptionEventProcess);
            _eventProcessActions.Add(typeof(SyncEventInfo).Name, SyncEventProcess);
            _eventProcessActions.Add(typeof(TestStateEventInfo).Name, TestGenEventProcess);

            this._sessionStateHandles = new Dictionary<int, SessionStateHandle>(Constants.DefaultRuntimeSize);
        }

        public void Initialize(ISequenceFlowContainer sequenceData)
        {
            _stateManageContext = new StateManageContext(_globalInfo, sequenceData);
            if (sequenceData is ITestProject)
            {
                ITestProject testProject = (ITestProject)sequenceData;
                SessionStateHandle testProjectStateHandle = new SessionStateHandle(testProject,
                    _stateManageContext);
                _sessionStateHandles.Add(testProjectStateHandle.Session, testProjectStateHandle);

                for (int index = 0; index < testProject.SequenceGroups.Count; index++)
                {
                    SessionStateHandle stateHandle = new SessionStateHandle(index, testProject.SequenceGroups[index], 
                        _stateManageContext);
                    _sessionStateHandles.Add(stateHandle.Session, stateHandle);
                }
            }
            else
            {
                SessionStateHandle stateHandle = new SessionStateHandle(0, (ISequenceGroup) sequenceData,
                    _stateManageContext);
                _sessionStateHandles.Add(stateHandle.Session, stateHandle);
            }
        }

        public void Start()
        {
            foreach (SessionStateHandle stateHandle in _sessionStateHandles.Values)
            {
                stateHandle.Start();
            }

            _cancellation = new CancellationTokenSource();
            this._internalMessageThd = new Thread(ProcessInternalMessage)
            {
                Name = "InnerMessageListener",
                IsBackground = true
            };
            _internalMessageThd.Start();
        }

        private void ProcessInternalMessage(object state)
        {
            LocalEventQueue<EventInfoBase> internalEventQueue = _globalInfo.EventQueue;
            try
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    EventInfoBase eventInfo = internalEventQueue.WaitUntilMessageCome();
                    if (null == eventInfo)
                    {
                        return;
                    }
                    _eventProcessActions[eventInfo.GetType().Name].Invoke(eventInfo);
                }
                // 处理完堆积的内部消息，然后停止
                while (internalEventQueue.Count > 0)
                {
                    EventInfoBase eventInfo = internalEventQueue.Dequeue();
                    _eventProcessActions[eventInfo.GetType().Name].Invoke(eventInfo);
                }
            }
            catch (TestflowException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex);
                _globalInfo.StateMachine.State = RuntimeState.Error;
                throw;
            }
            catch (ThreadAbortException)
            {
                _globalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                    $"thread {Thread.CurrentThread.Name} is stopped abnormally");
            }
            catch (Exception ex)
            {
                _globalInfo.EventQueue.Enqueue(new ExceptionEventInfo(ex));
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex);
                _globalInfo.StateMachine.State = RuntimeState.Collapsed;
            }
        }

        // 完成的功能：执行结束后构造TestInstanceData并持久化，转发测试消息和事件到下级
        #region 事件和消息处理

        private void AbortEventProcess(EventInfoBase eventInfo)
        {
            AbortEventInfo abortEvent = (AbortEventInfo) eventInfo;
            if (abortEvent.Session == CommonConst.PlatformLogSession || IsRootEvent(eventInfo))
            {
                foreach (SessionStateHandle stateHandle in _sessionStateHandles.Values)
                {
                    stateHandle.AbortEventProcess(abortEvent);
                }
            }
            else
            {
                _sessionStateHandles[abortEvent.Session].AbortEventProcess(abortEvent);
            }
        }

        private void DebugEventProcess(EventInfoBase eventInfo)
        {
            DebugEventInfo debugEvent = (DebugEventInfo) eventInfo;
            _sessionStateHandles[debugEvent.Session].DebugEventProcess(debugEvent);
        }

        private void ExceptionEventProcess(EventInfoBase eventInfo)
        {
            ExceptionEventInfo exceptionEvent = (ExceptionEventInfo) eventInfo;
            if (exceptionEvent.Session == CommonConst.PlatformLogSession || IsRootEvent(eventInfo))
            {
                foreach (SessionStateHandle stateHandle in _sessionStateHandles.Values)
                {
                    stateHandle.ExceptionEventProcess(exceptionEvent);
                }
//                _globalInfo.StateMachine.State = RuntimeState.Failed;
            }
            else
            {
                _sessionStateHandles[exceptionEvent.Session].ExceptionEventProcess(exceptionEvent);
            }
        }

        private void SyncEventProcess(EventInfoBase eventInfo)
        {
            SyncEventInfo syncEvent = (SyncEventInfo) eventInfo;
            _sessionStateHandles[syncEvent.Session].SyncEventProcess(syncEvent);
        }

        private void TestGenEventProcess(EventInfoBase eventInfo)
        {
            TestStateEventInfo testGenEvent = (TestStateEventInfo) eventInfo;
            _sessionStateHandles[testGenEvent.Session].TestGenEventProcess(testGenEvent);
        }

        public bool HandleMessage(MessageBase message)
        {
            SessionStateHandle stateHandle = _sessionStateHandles[message.Id];
            bool handleResult = false;
            switch (message.Type)
            {
                case MessageType.Status:
                    handleResult = stateHandle.HandleStatusMessage((StatusMessage) message);
                    break;
                case MessageType.TestGen:
                    handleResult = stateHandle.HandleTestGenMessage((TestGenMessage) message);
                    break;
                case MessageType.Ctrl:
                case MessageType.Debug:
                case MessageType.RmtGen:
                case MessageType.Sync:
                case MessageType.RuntimeError:
                default:
                    throw new InvalidProgramException();
            }
            return handleResult;
        }

        #endregion


        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            _cancellation.Cancel();
            _globalInfo.EventQueue.StopEnqueue();
            Thread.Sleep(_globalInfo.ConfigData.GetProperty<int>("StopTimeout"));
            ModuleUtils.StopThreadWork(_internalMessageThd);
        }

        public void Dispose()
        {
            this.Stop();
        }

        public void StopIfAllTestOver()
        {
            EventDispatcher eventDispatcher = _stateManageContext.EventDispatcher;
            if (_stateManageContext.IsAllTestOver)
            {
                eventDispatcher.RaiseEvent(CoreConstants.TestProjectOver, CommonConst.PlatformLogSession,
                    _stateManageContext.TestStatus);
            }
        }

        /// <summary>
        /// 是否为根序列的事件
        /// </summary>
        private bool IsRootEvent(EventInfoBase eventInfo)
        {
            return (eventInfo.Session == CommonConst.TestGroupSession) || 1 == _sessionStateHandles.Count;
        }
    }
}