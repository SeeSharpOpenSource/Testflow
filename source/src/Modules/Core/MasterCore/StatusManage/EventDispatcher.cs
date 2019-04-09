using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.EventData;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Utility.I18nUtil;

namespace Testflow.MasterCore.StatusManage
{
    /// <summary>
    /// 外部消息分发
    /// </summary>
    internal class EventDispatcher
    {

        public EventDispatcher()
        {
            this.AsyncDispatch = true;
        }

        public bool AsyncDispatch { get; set; }


        public void Register(Delegate callBack, int session, string eventName)
        {
            switch (eventName)
            {
                case Constants.TestGenerationStart:
                    TestGenerationStart += ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.TestGenerationEnd:
                    TestGenerationEnd += ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationStart:
                    SessionGenerationStart += ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationReport:
                    SessionGenerationReport += ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.TestProjectStart:
                    TestProjectStart += ModuleUtils.GetDeleage<RuntimeDelegate.TestProjectStatusAction>(callBack);
                    break;
                case Constants.SessionStart:
                    SessionStart += ModuleUtils.GetDeleage<RuntimeDelegate.SessionStatusAction>(callBack);
                    break;
                case Constants.SessionGenerationEnd:
                    SessionGenerationEnd += ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
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
                case Constants.TestProjectOver:
                    TestProjectOver += ModuleUtils.GetDeleage<RuntimeDelegate.TestProjectStatusAction>(callBack);
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
                case Constants.TestGenerationStart:
                    TestGenerationStart -= ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.TestGenerationEnd:
                    TestGenerationEnd -= ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationStart:
                    SessionGenerationStart -= ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationReport:
                    SessionGenerationReport -= ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.SessionGenerationEnd:
                    SessionGenerationEnd -= ModuleUtils.GetDeleage<RuntimeDelegate.SessionGenerationAction>(callBack);
                    break;
                case Constants.TestProjectStart:
                    TestProjectStart -= ModuleUtils.GetDeleage<RuntimeDelegate.TestProjectStatusAction>(callBack);
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
                case Constants.TestProjectOver:
                    TestProjectOver -= ModuleUtils.GetDeleage<RuntimeDelegate.TestProjectStatusAction>(callBack);
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowInternalException(ModuleErrorCode.UnexistEvent, i18N.GetFStr("UnexistEvent", eventName));
                    break;
            }
        }

        public void RaiseEvent(string eventName, int sessionId, params object[] eventParam)
        {
            // TODO 暂时直接使用线程池调用，后期优化
            ThreadPool.QueueUserWorkItem(InvokeEvent, new EventParam(eventName, sessionId, eventParam));
        }

        private void InvokeEvent(object state)
        {
            EventParam eventParams = (EventParam)state;
            string eventName = eventParams.EventName;
            int sessionId = eventParams.Session;
            object[] eventParam = eventParams.EventParams;
            switch (eventName)
            {
                case Constants.TestGenerationStart:
                    OnTestGenerationStart(ModuleUtils.GetParamValue<ITestGenerationInfo>(eventParam, 0));
                    break;
                case Constants.TestGenerationEnd:
                    OnTestGenerationEnd(ModuleUtils.GetParamValue<ITestGenerationInfo>(eventParam, 0));
                    break;
                case Constants.SessionGenerationStart:
                    OnSessionGenerationStart(ModuleUtils.GetParamValue<ISessionGenerationInfo>(eventParam, 0));
                    break;
                case Constants.SessionGenerationReport:
                    OnSessionGenerationReport(ModuleUtils.GetParamValue<ISessionGenerationInfo>(eventParam, 0));
                    break;
                case Constants.SessionGenerationEnd:
                    OnSessionGenerationEnd(ModuleUtils.GetParamValue<ISessionGenerationInfo>(eventParam, 0));
                    break;
                case Constants.TestProjectStart:
                    OnTestProjectStart(ModuleUtils.GetParamValue<List<ITestResultCollection>>(eventParam, 0));
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
                case Constants.TestProjectOver:
                    OnTestProjectOver(ModuleUtils.GetParamValue<List<ITestResultCollection>>(eventParam, 0));
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

        #region 事件调用

        /// <summary>
        /// 测试生成开始事件
        /// </summary>
        public event RuntimeDelegate.TestGenerationAction TestGenerationStart;

        /// <summary>
        /// 测试生成结束事件
        /// </summary>
        public event RuntimeDelegate.TestGenerationAction TestGenerationEnd;

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
        /// 测试工程开始事件
        /// </summary>
        public event RuntimeDelegate.TestProjectStatusAction TestProjectStart;

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
        /// 测试工程结束事件
        /// </summary>
        public event RuntimeDelegate.TestProjectStatusAction TestProjectOver;

        /// <summary>
        /// 断点命中事件，当某个断点被命中时触发
        /// </summary>
        public event RuntimeDelegate.BreakPointHittedAction BreakPointHitted;

        //

        //        /// <summary>

        //        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.

        //        /// </summary>

        //        event RuntimeDelegate.StatusReceivedAction SequenceFailed;

        internal void OnTestGenerationStart(ITestGenerationInfo generationinfo)
        {
            TestGenerationStart?.Invoke(generationinfo);
        }

        internal void OnTestGenerationEnd(ITestGenerationInfo generationinfo)
        {
            TestGenerationEnd?.Invoke(generationinfo);
        }

        internal void OnSessionGenerationStart(ISessionGenerationInfo generationinfo)
        {
            SessionGenerationStart?.Invoke(generationinfo);
        }

        internal void OnSessionGenerationEnd(ISessionGenerationInfo generationinfo)
        {
            SessionGenerationEnd?.Invoke(generationinfo);
        }

        internal void OnSequenceStarted(ISequenceTestResult result)
        {
            SequenceStarted?.Invoke(result);
        }

        internal void OnStatusReceived(IRuntimeStatusInfo statusinfo)
        {
            StatusReceived?.Invoke(statusinfo);
        }

        internal void OnSequenceOver(ISequenceTestResult result)
        {
            SequenceOver?.Invoke(result);
        }

        internal void OnTestOver(ITestResultCollection statistics)
        {
            SessionOver?.Invoke(statistics);
        }

        internal void OnTestStart(ITestResultCollection statistics)
        {
            SessionStart?.Invoke(statistics);
        }

        internal void OnBreakPointHitted(IDebuggerHandle debuggerHandle, IDebugInformation information)
        {
            BreakPointHitted?.Invoke(debuggerHandle, information);
        }

        internal void OnSessionGenerationReport(ISessionGenerationInfo generationinfo)
        {
            SessionGenerationReport?.Invoke(generationinfo);
        }

        internal void OnTestProjectStart(IList<ITestResultCollection> testResults)
        {
            TestProjectStart?.Invoke(testResults);
        }

        internal void OnTestProjectOver(IList<ITestResultCollection> testResults)
        {
            TestProjectOver?.Invoke(testResults);
        }

        #endregion
        
        internal class EventParam
        {
            public string EventName { get; }

            public int Session { get; }

            public object[] EventParams { get; }

            public EventParam(string eventName, int session, object[] eventParams)
            {
                this.EventName = eventName;
                this.Session = session;
                this.EventParams = eventParams;
            }
        }
    }
}