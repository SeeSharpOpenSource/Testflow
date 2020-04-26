using System;
using System.Threading;
using Testflow.CoreCommon;
using Testflow.MasterCore.Common;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.MasterCore.StatusManage
{
    internal class SessionEventPump : IDisposable
    {
        private Thread _eventInvokeThread;
        public LocalEventQueue<EventParam> _eventQueue;

        public int SessionId { get; }

        /// <summary>
        /// 会话测试生成开始事件
        /// </summary>
        public event RuntimeDelegate.SessionGenerationAction SessionGenerationStart;

        /// <summary>
        /// 会话测试生成中间事件，生成过程中会不间断生成该事件
        /// </summary>
        public event RuntimeDelegate.SessionGenerationAction SessionGenerationReport;

        /// <summary>
        /// 会话测试生成结束事件
        /// </summary>
        public event RuntimeDelegate.SessionGenerationAction SessionGenerationEnd;

        /// <summary>
        /// 测试序列组开始执行的事件
        /// </summary>
        public event RuntimeDelegate.SessionStatusAction SessionStart;

        /// <summary>
        /// Events raised when a sequence is start and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        public event RuntimeDelegate.SequenceStatusAction SequenceStarted;

        /// <summary>
        /// Events raised when receive runtime status information. Asynchronous event.
        /// </summary>
        public event RuntimeDelegate.StatusReceivedAction StatusReceived;

        /// <summary>
        /// Events raised when a sequence is over and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        public event RuntimeDelegate.SequenceStatusAction SequenceOver;

        /// <summary>
        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        public event RuntimeDelegate.SessionStatusAction SessionOver;

        /// <summary>
        /// 断点命中事件，当某个断点被命中时触发
        /// </summary>
        public event RuntimeDelegate.BreakPointHittedAction BreakPointHitted;


        private readonly ModuleGlobalInfo _globalInfo;

        private int _eventOverFlag = 1;

        public bool EventOver
        {
            get { return _eventOverFlag != 0; }
            set { Thread.VolatileWrite(ref _eventOverFlag, value ? 1 : 0); }
        }

        public SessionEventPump(int sessionId, ModuleGlobalInfo globalInfo)
        {
            _eventQueue = new LocalEventQueue<EventParam>(Constants.MaxEventsQueueSize);
            this._globalInfo = globalInfo;
            this.SessionId = sessionId;
        }

        public void Register(Delegate callBack, int session, string eventName)
        {
            switch (eventName)
            {
                case Constants.SessionGenerationStart:
                    SessionGenerationStart += ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationReport:
                    SessionGenerationReport += ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationEnd:
                    SessionGenerationEnd += ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionStart:
                    SessionStart += ModuleUtils.GetDeleage<RuntimeDelegate.SessionStatusAction>(callBack);
                    break;
                case Constants.SequenceStarted:
                    SequenceStarted += ModuleUtils.GetDeleage<RuntimeDelegate.SequenceStatusAction>(callBack);
                    break;
                case Constants.StatusReceived:
                    StatusReceived += ModuleUtils.GetDeleage<RuntimeDelegate.StatusReceivedAction>(callBack);
                    break;
                case Constants.SequenceOver:
                    SequenceOver += ModuleUtils.GetDeleage<RuntimeDelegate.SequenceStatusAction>(callBack);
                    break;
                case Constants.SessionOver:
                    SessionOver += ModuleUtils.GetDeleage<RuntimeDelegate.SessionStatusAction>(callBack);
                    break;
                case Constants.BreakPointHitted:
                    BreakPointHitted += ModuleUtils.GetDeleage<RuntimeDelegate.BreakPointHittedAction>(callBack);
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowInternalException(ModuleErrorCode.UnexistEvent, i18N.GetFStr("UnexistEvent", eventName));
                    break;
            }
        }

        public void Unregister(Delegate callBack, string eventName)
        {
            switch (eventName)
            {
                case Constants.SessionGenerationStart:
                    SessionGenerationStart -= ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationReport:
                    SessionGenerationReport -= ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationEnd:
                    SessionGenerationEnd -= ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionStart:
                    SessionStart -= ModuleUtils.GetDeleage<RuntimeDelegate.SessionStatusAction>(callBack);
                    break;
                case Constants.SequenceStarted:
                    SequenceStarted -= ModuleUtils.GetDeleage<RuntimeDelegate.SequenceStatusAction>(callBack);
                    break;
                case Constants.StatusReceived:
                    StatusReceived -= ModuleUtils.GetDeleage<RuntimeDelegate.StatusReceivedAction>(callBack);
                    break;
                case Constants.SequenceOver:
                    SequenceOver -= ModuleUtils.GetDeleage<RuntimeDelegate.SequenceStatusAction>(callBack);
                    break;
                case Constants.SessionOver:
                    SessionOver -= ModuleUtils.GetDeleage<RuntimeDelegate.SessionStatusAction>(callBack);
                    break;
                case Constants.BreakPointHitted:
                    BreakPointHitted -= ModuleUtils.GetDeleage<RuntimeDelegate.BreakPointHittedAction>(callBack);
                    break;
            }
        }

        public void Start()
        {
            EventOver = false;
            Thread.MemoryBarrier();
            _eventInvokeThread = new Thread(InvokeEvents)
            {
                IsBackground = true
            };
            _eventInvokeThread.Start();
        }

        public void PushEventsParamInfo(EventParam eventParamInfo)
        {
            if (EventOver)
            {
                return;
            }
            _eventQueue.Enqueue(eventParamInfo);
        }

        private void InvokeEvents(object state)
        {
            _globalInfo.LogService.Print(LogLevel.Debug, CommonConst.PlatformLogSession,
                                $"Session {SessionId} event pump started.");
            while (!EventOver)
            {
                try
                {
                    while (true)
                    {
                        EventParam eventParam = _eventQueue.WaitUntilMessageCome();
                        // 获取到为null的事件，即是消息执行结束
                        if (null == eventParam)
                        {
                            EventOver = true;
                            _globalInfo.LogService.Print(LogLevel.Debug, CommonConst.PlatformLogSession,
                                $"Session {SessionId} event pump over.");
                            break;
                        }
                        InvokeSingleEvent(eventParam);
                    }
                }
                catch (ThreadAbortException ex)
                {
                    _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex,
                        $"Session {SessionId} event pump aborted.");
                    EventOver = true;
                    break;
                }
                catch (TestflowException ex)
                {
                    _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex,
                        $"Session {SessionId} event pump terminated by interal error.");
                    EventOver = true;
                    break;
                }
                catch (Exception ex)
                {
                    _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex,
                        $"Session {SessionId} event pump error.");
                }
            }
        }

        private void InvokeSingleEvent(EventParam eventParamInfo)
        {
            string eventName = eventParamInfo.EventName;
            object[] eventParam = eventParamInfo.EventParams;
            switch (eventName)
            {
                case Constants.SessionGenerationStart:
                    OnSessionGenerationStart(ModuleUtils.GetParamValue<ISessionGenerationInfo>(eventParam, 0));
                    break;
                case Constants.SessionGenerationReport:
                    OnSessionGenerationReport(ModuleUtils.GetParamValue<ISessionGenerationInfo>(eventParam, 0));
                    break;
                case Constants.SessionGenerationEnd:
                    OnSessionGenerationEnd(ModuleUtils.GetParamValue<ISessionGenerationInfo>(eventParam, 0));
                    break;
                case Constants.SessionStart:
                    OnTestStart(ModuleUtils.GetParamValue<ITestResultCollection>(eventParam, 0));
                    break;
                case Constants.SequenceStarted:
                    OnSequenceStarted(ModuleUtils.GetParamValue<ISequenceTestResult>(eventParam, 0));
                    break;
                case Constants.StatusReceived:
                    OnStatusReceived(ModuleUtils.GetParamValue<IRuntimeStatusInfo>(eventParam, 0));
                    break;
                case Constants.SequenceOver:
                    OnSequenceOver(ModuleUtils.GetParamValue<ISequenceTestResult>(eventParam, 0));
                    break;
                case Constants.SessionOver:
                    //                    eventHandle.OnTestOver(ModuleUtil.GetParamValue<ITestResultCollection>(eventParams, 0),
                    //                        ModuleUtil.GetParamValue<ISequenceGroup>(eventParams, 1));
                    OnTestOver(ModuleUtils.GetParamValue<ITestResultCollection>(eventParam, 0));
                    break;
                case Constants.BreakPointHitted:
                    OnBreakPointHitted(ModuleUtils.GetParamValue<IDebuggerHandle>(eventParam, 0),
                        ModuleUtils.GetParamValue<IDebugInformation>(eventParam, 1));
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowInternalException(ModuleErrorCode.UnexistEvent,
                        i18N.GetFStr("UnexistEvent", eventName));
                    break;
            }
        }

        private void OnSessionGenerationStart(ISessionGenerationInfo generationinfo)
        { 
            SessionGenerationStart?.Invoke(generationinfo);
        }

        private void OnSessionGenerationEnd(ISessionGenerationInfo generationinfo)
        {
            SessionGenerationEnd?.Invoke(generationinfo);
        }

        private void OnSequenceStarted(ISequenceTestResult result)
        {
            SequenceStarted?.Invoke(result);
        }

        private void OnStatusReceived(IRuntimeStatusInfo statusinfo)
        {
            StatusReceived?.Invoke(statusinfo);
        }

        private void OnSequenceOver(ISequenceTestResult result)
        {
            SequenceOver?.Invoke(result);
        }

        private void OnTestOver(ITestResultCollection statistics)
        {
            SessionOver?.Invoke(statistics);
        }

        private void OnTestStart(ITestResultCollection statistics)
        {
            SessionStart?.Invoke(statistics);
        }

        private void OnBreakPointHitted(IDebuggerHandle debuggerHandle, IDebugInformation information)
        {
            BreakPointHitted?.Invoke(debuggerHandle, information);
        }

        private void OnSessionGenerationReport(ISessionGenerationInfo generationinfo)
        {
            SessionGenerationReport?.Invoke(generationinfo);
        }

        private int _disposedFlag = 0;
        public void Dispose()
        {
            if (_disposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _disposedFlag, 1);
            if (EventOver || null == _eventInvokeThread)
            {
                return;
            }
            EventOver = true;
            Thread.MemoryBarrier();
            if (_eventInvokeThread.IsAlive && !_eventInvokeThread.Join(Constants.EventQueueTimeOut))
            {
                _eventInvokeThread.Abort();
            }
        }
    }
}