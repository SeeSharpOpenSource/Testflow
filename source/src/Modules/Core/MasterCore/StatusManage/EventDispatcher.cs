using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Testflow.Usr;
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
    internal class EventDispatcher : IDisposable
    {
        public EventDispatcher(ModuleGlobalInfo globalInfo, ISequenceFlowContainer sequenceData)
        {
            this.AsyncDispatch = true;
            // 初始化事件泵
            if (sequenceData is ISequenceGroup)
            {
                _eventPumps = new Dictionary<int, SessionEventPump>(1)
                {
                    {0, new SessionEventPump(0, globalInfo) }
                };
            }
            else if (sequenceData is ITestProject)
            {
                ITestProject testProject = ((ITestProject) sequenceData);
                _eventPumps = new Dictionary<int, SessionEventPump>(testProject.SequenceGroups.Count + 1)
                {
                    {CommonConst.TestGroupSession, new SessionEventPump(CommonConst.TestGroupSession, globalInfo)}
                };
                for (int sessionId = 0; sessionId < testProject.SequenceGroups.Count; sessionId++)
                {
                    _eventPumps.Add(sessionId, new SessionEventPump(sessionId, globalInfo));
                }
            }
        }

        private readonly IDictionary<int, SessionEventPump> _eventPumps;

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
                case Constants.TestInstanceStart:
                    TestInstanceStart += ModuleUtils.GetDeleage<RuntimeDelegate.TestInstanceStatusAction>(callBack);
                    break;
                case Constants.TestInstanceOver:
                    TestInstanceOver += ModuleUtils.GetDeleage<RuntimeDelegate.TestInstanceStatusAction>(callBack);
                    break;
                case Constants.SessionGenerationStart:
                case Constants.SessionGenerationReport:
                case Constants.SessionGenerationEnd:
                case Constants.SessionStart:
                case Constants.SequenceStarted:
                case Constants.StatusReceived:
                case Constants.SequenceOver:
                case Constants.SessionOver:
                case Constants.BreakPointHitted:
                    if (_eventPumps.ContainsKey(session))
                    {
                        _eventPumps[session].Register(callBack, session, eventName);
                    }
                    else
                    {
                        foreach (SessionEventPump sessionEventPump in _eventPumps.Values)
                        {
                            sessionEventPump.Register(callBack, session, eventName);
                        }
                    }
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowInternalException(ModuleErrorCode.UnexistEvent, i18N.GetFStr("UnexistEvent", eventName));
                    break;
            }
        }

        public void Unregister(Delegate callBack, int session, string eventName)
        {
            switch (eventName)
            {
                case Constants.TestGenerationStart:
                    TestGenerationStart -= ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.TestGenerationEnd:
                    TestGenerationEnd -= ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.TestInstanceStart:
                    TestInstanceStart -= ModuleUtils.GetDeleage<RuntimeDelegate.TestInstanceStatusAction>(callBack);
                    break;
                case Constants.TestInstanceOver:
                    TestInstanceOver -= ModuleUtils.GetDeleage<RuntimeDelegate.TestInstanceStatusAction>(callBack);
                    break;
                case Constants.SessionGenerationStart:
                case Constants.SessionGenerationReport:
                case Constants.SessionGenerationEnd:
                case Constants.SessionStart:
                case Constants.SequenceStarted:
                case Constants.StatusReceived:
                case Constants.SequenceOver:
                case Constants.SessionOver:
                case Constants.BreakPointHitted:
                    if (_eventPumps.ContainsKey(session))
                    {
                        _eventPumps[session].Unregister(callBack, eventName);
                    }
                    else
                    {
                        foreach (SessionEventPump sessionEventPump in _eventPumps.Values)
                        {
                            sessionEventPump.Unregister(callBack,  eventName);
                        }
                    }
                    break;
            }
        }

        public void Start()
        {
            foreach (SessionEventPump eventPump in _eventPumps.Values)
            {
                eventPump.Start();
            }
        }

        public void Stop()
        {
            foreach (SessionEventPump eventPump in _eventPumps.Values)
            {
                eventPump.PushEventsParamInfo(null);
            }
        }

        public void RaiseEvent(string eventName, int sessionId, params object[] eventParam)
        {
            EventParam eventParamInfo = new EventParam(eventName, sessionId, eventParam);
            switch (eventName)
            {
                case Constants.SessionGenerationStart:
                case Constants.SessionGenerationReport:
                case Constants.SessionGenerationEnd:
                case Constants.SessionStart:
                case Constants.SequenceStarted:
                case Constants.StatusReceived:
                case Constants.SequenceOver:
                case Constants.SessionOver:
                case Constants.BreakPointHitted:
                    if (_eventPumps.ContainsKey(eventParamInfo.Session))
                    {
                        _eventPumps[eventParamInfo.Session].PushEventsParamInfo(eventParamInfo);
                    }
                    else
                    {
                        foreach (SessionEventPump sessionEventPump in _eventPumps.Values)
                        {
                            sessionEventPump.PushEventsParamInfo(eventParamInfo);
                        }
                    }
                    break;
                default:
                    // TestInstance相关事件使用线程池触发
                    ThreadPool.QueueUserWorkItem(InvokeEvent, eventParamInfo);
                    break;
            }
        }

        private void InvokeEvent(object state)
        {
            EventParam eventParamInfo = (EventParam)state;
            string eventName = eventParamInfo.EventName;
            object[] eventParam = eventParamInfo.EventParams;
            switch (eventName)
            {
                case Constants.TestGenerationStart:
                    OnTestGenerationStart(ModuleUtils.GetParamValue<ITestGenerationInfo>(eventParam, 0));
                    break;
                case Constants.TestGenerationEnd:
                    OnTestGenerationEnd(ModuleUtils.GetParamValue<ITestGenerationInfo>(eventParam, 0));
                    break;
                case Constants.TestInstanceStart:
                    OnTestProjectStart(ModuleUtils.GetParamValue<List<ITestResultCollection>>(eventParam, 0));
                    break;
                case Constants.TestInstanceOver:
                    OnTestProjectOver(ModuleUtils.GetParamValue<List<ITestResultCollection>>(eventParam, 0));
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
        /// 测试工程开始事件
        /// </summary>
        public event RuntimeDelegate.TestInstanceStatusAction TestInstanceStart;

        /// <summary>
        /// 测试工程结束事件
        /// </summary>
        public event RuntimeDelegate.TestInstanceStatusAction TestInstanceOver;

        
        //

        //        /// <summary>

        //        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.

        //        /// </summary>

        //        event RuntimeDelegate.StatusReceivedAction SequenceFailed;

        private void OnTestGenerationStart(ITestGenerationInfo generationinfo)
        {
            TestGenerationStart?.Invoke(generationinfo);
        }

        private void OnTestGenerationEnd(ITestGenerationInfo generationinfo)
        {
            TestGenerationEnd?.Invoke(generationinfo);
        }

        private void OnTestProjectStart(IList<ITestResultCollection> testResults)
        {
            TestInstanceStart?.Invoke(testResults);
        }

        private void OnTestProjectOver(IList<ITestResultCollection> testResults)
        {
            // 需要等待所有消息泵执行结束后再执行
            while (_eventPumps.Values.Any(item => !item.EventOver))
            {
                Thread.Sleep(100);
            }
            TestInstanceOver?.Invoke(testResults);
        }
        #endregion

        private int _disposedFlag = 0;
        public void Dispose()
        {
            if (_disposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _disposedFlag, 1);
            Thread.MemoryBarrier();
            foreach (SessionEventPump eventPump in _eventPumps.Values)
            {
                eventPump?.Dispose();
            }
            _eventPumps.Clear();
        }
    }
}