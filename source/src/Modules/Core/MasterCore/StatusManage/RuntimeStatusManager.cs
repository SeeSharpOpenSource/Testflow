using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Common;
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
        private readonly Dictionary<int, SessionStateMaintainer> _stateMaintainers;

        public RuntimeStatusManager(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;

            _eventProcessActions = new Dictionary<string, Action<EventInfoBase>>(5);
            _eventProcessActions.Add(typeof(AbortEventInfo).Name, AbortEventProcess);
            _eventProcessActions.Add(typeof(DebugEventInfo).Name, DebugEventProcess);
            _eventProcessActions.Add(typeof(ExceptionEventInfo).Name, ExceptionEventProcess);
            _eventProcessActions.Add(typeof(SyncEventInfo).Name, SyncEventProcess);
            _eventProcessActions.Add(typeof(TestGenEventInfo).Name, TestGenEventProcess);

            this._stateMaintainers = new Dictionary<int, SessionStateMaintainer>(Constants.DefaultRuntimeSize);
        }

        public void Initialize(ITestProject testProject)
        {
            SessionStateMaintainer testProjectSession = new SessionStateMaintainer(_globalInfo,
                CommonConst.TestGroupSession, testProject);
            _stateMaintainers.Add(testProjectSession.Session, testProjectSession);

            int sessionIndex = 0;
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                SessionStateMaintainer stateManager = new SessionStateMaintainer(_globalInfo, sessionIndex++, sequenceGroup);
                _stateMaintainers.Add(stateManager.Session, stateManager);
            }
        }

        public void Initialize(ISequenceGroup sequenceGroup)
        {
            SessionStateMaintainer stateMaintainer = new SessionStateMaintainer(_globalInfo, 0, sequenceGroup);
            _stateMaintainers.Add(stateMaintainer.Session, stateMaintainer);
        }

        public void Start()
        {
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

        private void AbortEventProcess(EventInfoBase eventInfo)
        {
            AbortEventInfo abortEvent = (AbortEventInfo)eventInfo;
            if (abortEvent.Session == CommonConst.PlatformLogSession || IsRootEvent(eventInfo))
            {
                foreach (SessionStateMaintainer stateMaintainer in _stateMaintainers.Values)
                {
                    stateMaintainer.AbortEventProcess(abortEvent);
                }
                _globalInfo.StateMachine.State = abortEvent.IsRequest ? RuntimeState.AbortRequested : RuntimeState.Abort;
            }
            else
            {
                _stateMaintainers[abortEvent.Session].AbortEventProcess(abortEvent);
            }
        }

        private void DebugEventProcess(EventInfoBase eventInfo)
        {
            DebugEventInfo debugEvent = (DebugEventInfo)eventInfo;
            _stateMaintainers[debugEvent.Session].DebugEventProcess(debugEvent);
        }

        private void ExceptionEventProcess(EventInfoBase eventInfo)
        {
            ExceptionEventInfo abortEvent = (ExceptionEventInfo)eventInfo;
            if (abortEvent.Session == CommonConst.PlatformLogSession || IsRootEvent(eventInfo))
            {
                foreach (SessionStateMaintainer stateMaintainer in _stateMaintainers.Values)
                {
                    stateMaintainer.ExceptionEventProcess(abortEvent);
                }
                _globalInfo.StateMachine.State = RuntimeState.Failed;
            }
            else
            {
                _stateMaintainers[abortEvent.Session].ExceptionEventProcess(abortEvent);
            }
        }

        private void SyncEventProcess(EventInfoBase eventInfo)
        {
            SyncEventInfo syncEvent = (SyncEventInfo) eventInfo;
            _stateMaintainers[syncEvent.Session].SyncEventProcess(syncEvent);
        }

        private void TestGenEventProcess(EventInfoBase eventInfo)
        {
            TestGenEventInfo testGenEvent = (TestGenEventInfo)eventInfo;
            _stateMaintainers[testGenEvent.Session].TestGenEventProcess(testGenEvent);
        }

        public bool HandleMessage(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            _cancellation.Cancel();
            ModuleUtils.StopThreadWork(_internalMessageThd);
        }

        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// 是否为根序列的事件
        /// </summary>
        private bool IsRootEvent(EventInfoBase eventInfo)
        {
            return (eventInfo.Session == CommonConst.TestGroupSession) || 1 == _stateMaintainers.Count;
        }
    }
}