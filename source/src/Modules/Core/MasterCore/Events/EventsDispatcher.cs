using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.Runtime;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Events
{
    /// <summary>
    /// 外部消息分发
    /// </summary>
    internal class EventsDispatcher
    {
        private readonly Dictionary<int, SessionEventHandle> _events;

        public EventsDispatcher(LocalEventQueue<EventInfoBase> eventQueue)
        {
            _events = new Dictionary<int, SessionEventHandle>(Constants.DefaultRuntimeSize);
            this.AsyncDispatch = true;
        }

        public bool AsyncDispatch { get; set; }

        public bool IsTestProjectEvent => _events.ContainsKey(Constants.TestProjectSessionId);

        public void InitEventsHandler(ISequenceFlowContainer sequenceContainer)
        {
            if (sequenceContainer is ITestProject)
            {
                ITestProject testProject = (ITestProject)sequenceContainer;
                _events.Add(Constants.TestProjectSessionId, new SessionEventHandle(Constants.TestProjectSessionId));
                for (int i = 0; i < testProject.SequenceGroups.Count; i++)
                {
                    _events.Add(i, new SessionEventHandle(i));
                }
            }
            else if (sequenceContainer is ISequenceGroup)
            {
                _events.Add(0, new SessionEventHandle(0));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void Register(Delegate callBack, int session, string eventName)
        {
            SessionEventHandle eventHandle = GetSessionEventHandle(session);
            switch (eventName)
            {
                case Constants.TestGenerationStart:
                    eventHandle.TestGenerationStart += ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.TestGenerationReport:
                    eventHandle.TestGenerationReport += ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.TestGenerationEnd:
                    eventHandle.TestGenerationEnd += ModuleUtils.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.SequenceStarted:
                    eventHandle.SequenceStarted += ModuleUtils.GetDeleage<RuntimeDelegate.StatusReceivedAction>(callBack);
                    break;
                case Constants.StatusReceived:
                    eventHandle.StatusReceived += ModuleUtils.GetDeleage<RuntimeDelegate.StatusReceivedAction>(callBack);
                    break;
                case Constants.SequenceOver:
                    eventHandle.SequenceOver += ModuleUtils.GetDeleage<RuntimeDelegate.StatusReceivedAction>(callBack);
                    break;
                case Constants.TestOver:
                    eventHandle.TestOver += ModuleUtils.GetDeleage<RuntimeDelegate.TestSessionOverAction>(callBack);
                    break;
                case Constants.BreakPointHitted:
                    eventHandle.BreakPointHitted += ModuleUtils.GetDeleage<RuntimeDelegate.BreakPointHittedAction>(callBack);
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowInternalException(ModuleErrorCode.UnexistEvent, i18N.GetFStr("UnexistEvent", eventName));
                    break;
            }
        }

        public void Start()
        {
            // TODO 
        }

        public void Stop()
        {
            // TODO
        }

        private void InvokeEvent(string eventName, int sessionId, params object[] eventParams)
        {
            SessionEventHandle eventHandle = GetSessionEventHandle(sessionId);
            switch (eventName)
            {
                case Constants.TestGenerationStart:
                    eventHandle.OnTestGenerationStart(ModuleUtils.GetParamValue<ITestGenerationInfo>(eventParams, 0));
                    break;
                case Constants.TestGenerationReport:
                    eventHandle.OnTestGenerationReport(ModuleUtils.GetParamValue<ITestGenerationInfo>(eventParams, 0));
                    break;
                case Constants.TestGenerationEnd:
                    eventHandle.OnTestGenerationEnd(ModuleUtils.GetParamValue<ITestGenerationInfo>(eventParams, 0));
                    break;
                case Constants.SequenceStarted:
                    eventHandle.OnSequenceStarted(ModuleUtils.GetParamValue<IRuntimeStatusInfo>(eventParams, 0),
                        ModuleUtils.GetParamValue<ICallStack>(eventParams, 1));
                    break;
                case Constants.StatusReceived:
                    eventHandle.OnStatusReceived(ModuleUtils.GetParamValue<IRuntimeStatusInfo>(eventParams, 0),
                        ModuleUtils.GetParamValue<ICallStack>(eventParams, 1));
                    break;
                case Constants.SequenceOver:
                    eventHandle.OnSequenceOver(ModuleUtils.GetParamValue<IRuntimeStatusInfo>(eventParams, 0),
                        ModuleUtils.GetParamValue<ICallStack>(eventParams, 1));
                    break;
                case Constants.TestOver:
//                    eventHandle.OnTestOver(ModuleUtil.GetParamValue<ITestResultCollection>(eventParams, 0),
//                        ModuleUtil.GetParamValue<ISequenceGroup>(eventParams, 1));
                    eventHandle.OnTestOver(ModuleUtils.GetParamValue<ITestResultCollection>(eventParams, 0),
                        (int)eventParams[1]);
                    break;
                case Constants.BreakPointHitted:
                    eventHandle.OnBreakPointHitted(ModuleUtils.GetParamValue<ISequenceDebugger>(eventParams, 0),
                        ModuleUtils.GetParamValue<IDebugInformation>(eventParams, 1));
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowInternalException(ModuleErrorCode.UnexistEvent,
                        i18N.GetFStr("UnexistEvent", eventName));
                    break;
            }
        }

        public void Clear()
        {
            this._events.Clear();
        }

        private SessionEventHandle GetSessionEventHandle(int session)
        {
            if (_events.ContainsKey(session) || null == _events[session])
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.UnexistSession,
                    i18N.GetFStr("UnexistSession", session.ToString()));
            }
            SessionEventHandle eventHandle = _events[session];
            return eventHandle;
        }
    }
}