using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.Runtime;
using Testflow.Runtime.Data;
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
            _eventProcessActions.Add(typeof(TestGenEventInfo).Name, TestGenEventProcess);

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

            _globalInfo.EventDispatcher = _stateManageContext.EventDispatcher;
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
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Engine collapsed by fatal error.");
                _globalInfo.StateMachine.State = RuntimeState.Collapsed;
            }
        }

        // 完成的功能：执行结束后构造TestInstanceData并持久化，转发测试消息和事件到下级
        #region 事件和消息处理

        private void TestGenEventProcess(EventInfoBase eventInfo)
        {
            TestGenEventInfo testGenEventInfo = (TestGenEventInfo)eventInfo;
            switch (testGenEventInfo.GenState)
            {
                case TestGenState.StartGeneration:
                    if (RuntimeState.Idle == _globalInfo.StateMachine.State)
                    {
                        _globalInfo.StateMachine.State = RuntimeState.TestGen;

                        _stateManageContext.TestInstance.StartGenTime = eventInfo.TimeStamp;
                        _stateManageContext.DatabaseProxy.WriteData(_stateManageContext.TestInstance);

                        _stateManageContext.EventDispatcher.RaiseEvent(Constants.TestGenerationStart, eventInfo.Session,
                            _stateManageContext.TestGenerationInfo);
                    }

                    _sessionStateHandles[eventInfo.Session].TestGenEventProcess(testGenEventInfo);
                    break;
                case TestGenState.GenerationOver:
                    _sessionStateHandles[testGenEventInfo.Session].TestGenEventProcess(testGenEventInfo);

                    if (_sessionStateHandles.Values.All(item => item.State == RuntimeState.StartIdle))
                    {
                        _globalInfo.StateMachine.State = RuntimeState.StartIdle;

                        _stateManageContext.TestInstance.EndGenTime = testGenEventInfo.TimeStamp;
                        _stateManageContext.DatabaseProxy.UpdateData(_stateManageContext.TestInstance);

                        _stateManageContext.EventDispatcher.RaiseEvent(Constants.TestGenerationEnd, testGenEventInfo.Session,
                        _stateManageContext.TestGenerationInfo);
                        // 测试生成处理结束，释放测试生成阻塞器
                        _globalInfo.TestGenBlocker.Set();
                    }
                    break;
                case TestGenState.Error:
                    _sessionStateHandles[eventInfo.Session].TestGenEventProcess(testGenEventInfo);
                    SetTestInstanceEndTime(testGenEventInfo.TimeStamp);
                    _stateManageContext.DatabaseProxy.UpdateData(_stateManageContext.TestInstance);
                    _stateManageContext.EventDispatcher.RaiseEvent(Constants.TestGenerationEnd, testGenEventInfo.Session, 
                        _stateManageContext.TestGenerationInfo);
                    _globalInfo.StateMachine.State = RuntimeState.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AbortEventProcess(EventInfoBase eventInfo)
        {
            AbortEventInfo abortEventInfo = (AbortEventInfo)eventInfo;
            if (abortEventInfo.IsRequest)
            {
                _globalInfo.StateMachine.State = RuntimeState.AbortRequested;

                _sessionStateHandles[eventInfo.Session].AbortEventProcess(abortEventInfo);
            }
            else
            {
                _sessionStateHandles[abortEventInfo.Session].AbortEventProcess(abortEventInfo);
                // 如果都已经Abort结束，则执行结束操作
                if (_sessionStateHandles.Values.All(item => item.State > RuntimeState.AbortRequested))
                {
                    _globalInfo.StateMachine.State = RuntimeState.Abort;

                    SetTestInstanceEndTime(eventInfo.TimeStamp);
                    _stateManageContext.DatabaseProxy.UpdateData(_stateManageContext.TestInstance);

                    _stateManageContext.EventDispatcher.RaiseEvent(Constants.TestInstanceOver, eventInfo.Session,
                        _stateManageContext.TestResults);
                }
            }

        }

        private void DebugEventProcess(EventInfoBase eventInfo)
        {
            DebugEventInfo debugEvent = (DebugEventInfo) eventInfo;
            _sessionStateHandles[debugEvent.Session].DebugEventProcess(debugEvent);
        }

        private void ExceptionEventProcess(EventInfoBase eventInfo)
        {
            _globalInfo.StateMachine.State = RuntimeState.Error;

            SetTestInstanceEndTime(eventInfo.TimeStamp);
            _stateManageContext.DatabaseProxy.UpdateData(_stateManageContext.TestInstance);

            _stateManageContext.EventDispatcher.RaiseEvent(Constants.TestInstanceOver, eventInfo.Session,
                _stateManageContext.TestResults);
        }

        private void SyncEventProcess(EventInfoBase eventInfo)
        {
            SyncEventInfo syncEvent = (SyncEventInfo) eventInfo;
            _sessionStateHandles[syncEvent.Session].SyncEventProcess(syncEvent);
        }

        public bool HandleMessage(MessageBase message)
        {
            bool handleResult = false;
            switch (message.Type)
            {
                case MessageType.Status:
                    handleResult = HandleStatusMessage(message);
                    break;
                case MessageType.TestGen:
                    handleResult = HandleTestGenMessage(message);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return handleResult;
        }

        private bool HandleTestGenMessage(MessageBase message)
        {
            return _sessionStateHandles[message.Id].HandleTestGenMessage((TestGenMessage) message);
        }

        private bool HandleStatusMessage(MessageBase message)
        {
            SessionStateHandle stateHandle = _sessionStateHandles[message.Id];
            StatusMessage statusMessage = (StatusMessage) message;
            bool result = true;
            switch (statusMessage.Name)
            {
                case MessageNames.StartStatusName:
                    if (RuntimeState.StartIdle == _globalInfo.StateMachine.State)
                    {
                        _stateManageContext.TestInstance.StartTime = message.Time;
                        _stateManageContext.DatabaseProxy.UpdateData(_stateManageContext.TestInstance);

                        _globalInfo.StateMachine.State = RuntimeState.Running;
                        _stateManageContext.EventDispatcher.RaiseEvent(Constants.TestInstanceStart,
                            CommonConst.TestGroupSession, _stateManageContext.TestResults);
                    }
                    stateHandle.HandleStatusMessage(statusMessage);
                    break;
                case MessageNames.ReportStatusName:
                    stateHandle.HandleStatusMessage(statusMessage);
                    break;
                case MessageNames.ResultStatusName:
                case MessageNames.ErrorStatusName:
                    stateHandle.HandleStatusMessage(statusMessage);

                    if (_stateManageContext.IsAllTestOver)
                    {
                        SetTestInstanceEndTime(message.Time);
                        _stateManageContext.DatabaseProxy.UpdateData(_stateManageContext.TestInstance);

                        _globalInfo.StateMachine.State = RuntimeState.Over;
                        _stateManageContext.EventDispatcher.RaiseEvent(Constants.TestInstanceOver,
                            CommonConst.TestGroupSession, _stateManageContext.TestResults);
                    }
                    break;
                case MessageNames.HeartBeatStatusName:
                    stateHandle.HandleStatusMessage(statusMessage);
                    break;
                default:
                    throw new InvalidOperationException();
                    break;
            }
            return result;
        }

        #endregion

        public SessionStateHandle this[int session] => _sessionStateHandles[session];

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        private int _stopFlag = 0;
        public void Stop()
        {
            if (_stopFlag == 1)
            {
                return;
            }
            _globalInfo.TestGenBlocker.Set();
            _cancellation.Cancel();
            _globalInfo.EventQueue.FreeBlocks();
            Thread.Sleep(_globalInfo.ConfigData.GetProperty<int>("StopTimeout"));
            ModuleUtils.StopThreadWork(_internalMessageThd);
            Thread.VolatileWrite(ref _stopFlag, 1);
        }

        public void Dispose()
        {
            // ignore
        }

        /// <summary>
        /// 是否为根序列的事件
        /// </summary>
        private bool IsRootEvent(EventInfoBase eventInfo)
        {
            return (eventInfo.Session == CommonConst.TestGroupSession) || 1 == _sessionStateHandles.Count || eventInfo.Session == CommonConst.PlatformLogSession;
        }

        private void SetTestInstanceEndTime(DateTime endTime)
        {
            TestInstanceData testInstanceData = _stateManageContext.TestInstance;
            testInstanceData.EndTime = endTime;
            if (testInstanceData.StartTime == DateTime.MaxValue)
            {
                testInstanceData.StartTime = endTime;
            }
            if (testInstanceData.EndGenTime == DateTime.MaxValue)
            {
                testInstanceData.EndGenTime = endTime;
            }
            if (testInstanceData.StartGenTime == DateTime.MaxValue)
            {
                testInstanceData.StartGenTime = endTime;
            }
            testInstanceData.ElapsedTime = (testInstanceData.EndTime - testInstanceData.StartGenTime).TotalMilliseconds;
        }

        public RuntimeState GetRuntimeState(int sessionId)
        {
            return _sessionStateHandles[sessionId].State;
        }
    }
}