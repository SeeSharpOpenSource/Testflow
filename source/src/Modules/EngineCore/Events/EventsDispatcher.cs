﻿using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.EngineCore.Common;
using Testflow.Runtime;
using Testflow.Utility.I18nUtil;

namespace Testflow.EngineCore.Events
{
    internal class EventsDispatcher
    {
        private readonly Dictionary<int, SessionEventHandle> _events;

        public EventsDispatcher()
        {
            _events = new Dictionary<int, SessionEventHandle>(Constants.DefaultRuntimeSize);
        }

        public void InitEventsHandler(ITestProject testProject)
        {
            _events.Add(Constants.TestProjectSessionId, new SessionEventHandle(Constants.TestProjectSessionId));
            for (int i = 0; i < testProject.SequenceGroups.Count; i++)
            {
                _events.Add(i, new SessionEventHandle(i));
            }
        }

        public bool IsTestProjectEvent => _events.ContainsKey(Constants.TestProjectSessionId);

        public void InitEventHander(ISequenceGroup sequenceGroup)
        {
            _events.Add(0, new SessionEventHandle(0));
        }

        public void Register(Delegate callBack, int session, string eventName)
        {
            SessionEventHandle eventHandle = GetSessionEventHandle(session);
            switch (eventName)
            {
                case Constants.TestGenerationStart:
                    eventHandle.TestGenerationStart += ModuleUtil.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.TestGenerationReport:
                    eventHandle.TestGenerationReport += ModuleUtil.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.TestGenerationEnd:
                    eventHandle.TestGenerationEnd += ModuleUtil.GetDeleage<RuntimeDelegate.TestGenerationAction>(callBack);
                    break;
                case Constants.SequenceStarted:
                    eventHandle.SequenceStarted += ModuleUtil.GetDeleage<RuntimeDelegate.StatusReceivedAction>(callBack);
                    break;
                case Constants.StatusReceived:
                    eventHandle.StatusReceived += ModuleUtil.GetDeleage<RuntimeDelegate.StatusReceivedAction>(callBack);
                    break;
                case Constants.SequenceOver:
                    eventHandle.SequenceOver += ModuleUtil.GetDeleage<RuntimeDelegate.StatusReceivedAction>(callBack);
                    break;
                case Constants.TestOver:
                    eventHandle.TestOver += ModuleUtil.GetDeleage<RuntimeDelegate.TestSessionOverAction>(callBack);
                    break;
                case Constants.BreakPointHitted:
                    eventHandle.BreakPointHitted += ModuleUtil.GetDeleage<RuntimeDelegate.BreakPointHittedAction>(callBack);
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowInternalException(ModuleErrorCode.UnexistEvent, i18N.GetFStr("UnexistEvent", eventName));
                    break;
            }
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

        public void InvokeEvent(string eventName, int sessionId, params object[] eventParams)
        {
            SessionEventHandle eventHandle = GetSessionEventHandle(sessionId);
            switch (eventName)
            {
                case Constants.TestGenerationStart:
                    eventHandle.OnTestGenerationStart(ModuleUtil.GetParamValue<ITestGenerationInfo>(eventParams, 0));
                    break;
                case Constants.TestGenerationReport:
                    eventHandle.OnTestGenerationReport(ModuleUtil.GetParamValue<ITestGenerationInfo>(eventParams, 0));
                    break;
                case Constants.TestGenerationEnd:
                    eventHandle.OnTestGenerationEnd(ModuleUtil.GetParamValue<ITestGenerationInfo>(eventParams, 0));
                    break;
                case Constants.SequenceStarted:
                    eventHandle.OnSequenceStarted(ModuleUtil.GetParamValue<IRuntimeStatusInfo>(eventParams, 0),
                        ModuleUtil.GetParamValue<ICallStack>(eventParams, 1));
                    break;
                case Constants.StatusReceived:
                    eventHandle.OnStatusReceived(ModuleUtil.GetParamValue<IRuntimeStatusInfo>(eventParams, 0),
                        ModuleUtil.GetParamValue<ICallStack>(eventParams, 1));
                    break;
                case Constants.SequenceOver:
                    eventHandle.OnSequenceOver(ModuleUtil.GetParamValue<IRuntimeStatusInfo>(eventParams, 0),
                        ModuleUtil.GetParamValue<ICallStack>(eventParams, 1));
                    break;
                case Constants.TestOver:
                    eventHandle.OnTestOver(ModuleUtil.GetParamValue<ITestResultCollection>(eventParams, 0),
                        ModuleUtil.GetParamValue<ISequenceGroup>(eventParams, 1));
                    break;
                case Constants.BreakPointHitted:
                    eventHandle.OnBreakPointHitted(ModuleUtil.GetParamValue<ISequenceDebugger>(eventParams, 0),
                        ModuleUtil.GetParamValue<IDebugInformation>(eventParams, 1));
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowInternalException(ModuleErrorCode.UnexistEvent,
                        i18N.GetFStr("UnexistEvent", eventName));
                    break;
            }
        }
    }
}