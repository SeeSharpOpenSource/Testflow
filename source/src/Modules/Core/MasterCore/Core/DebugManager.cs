using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.MasterCore.ObjectManage;
using Testflow.MasterCore.ObjectManage.Objects;
using Testflow.Runtime;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Core
{
    /// <summary>
    /// 调试状态管理
    /// </summary>
    internal class DebugManager : IMessageHandler, IRuntimeObjectCustomer
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private readonly Dictionary<int, List<string>> _watchVariables;
        private readonly Dictionary<int, List<CallStack>> _breakPoints;
        private int _debugHitSession;
        private ISequenceFlowContainer _sequenceData;

        public DebugManager(ModuleGlobalInfo globalInfo)
        {
            this._globalInfo = globalInfo;
            _watchVariables = new Dictionary<int, List<string>>(Constants.DefaultRuntimeSize);
            _breakPoints = new Dictionary<int, List<CallStack>>(Constants.DefaultRuntimeSize);
            _debugHitSession = Constants.NoDebugHitSession;
        }

        #region 监视管理

        private void AddWatchVariable(int session, WatchDataObject watchDataObj)
        {
            string watchDataName = ModuleUtils.GetRuntimeVariableString(watchDataObj, _sequenceData); 
            if (!_watchVariables.ContainsKey(session))
            {
                _watchVariables.Add(session, new List<string>(Constants.DefaultRuntimeSize));
            }
            if (!_watchVariables[session].Contains(watchDataName))
            {
                _watchVariables[session].Add(watchDataName);
            }
            RuntimeState runtimeState = _globalInfo.StateMachine.State;
            if (runtimeState == RuntimeState.Running || runtimeState == RuntimeState.Blocked ||
                runtimeState == RuntimeState.DebugBlocked)
            {
                SendRefreshWatchMessage(session);
            }
        }

        private void RemoveWatchVariable(int session, WatchDataObject watchDataObj)
        {
            string watchDataName = ModuleUtils.GetRuntimeVariableString(watchDataObj, _sequenceData);
            if (!_watchVariables.ContainsKey(session))
            {
                return;
            }
            if (_watchVariables[session].Contains(watchDataName))
            {
                _watchVariables[session].Remove(watchDataName);
                if (0 == _watchVariables[session].Count)
                {
                    _watchVariables.Remove(session);
                }
            }
            RuntimeState runtimeState = _globalInfo.StateMachine.State;
            if (runtimeState == RuntimeState.Running || runtimeState == RuntimeState.Blocked ||
                runtimeState == RuntimeState.DebugBlocked)
            {
                SendRefreshWatchMessage(session);
            }
        }

        private void SendRefreshWatchMessage(int sessionId)
        {
            DebugMessage debugMessage = new DebugMessage(MessageNames.RefreshWatchName, sessionId, true)
            {
                WatchData = new DebugWatchData()
            };
            if (_watchVariables.ContainsKey(sessionId))
            {
                debugMessage.WatchData.Names.AddRange(_watchVariables[sessionId]);
            }
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        #endregion

        #region 表达式管理

        private void EvaluateExpression(EvaluationObject evaluationObject)
        {
            DebugMessage debugMessage = new DebugMessage(MessageNames.RequestValueName, evaluationObject.Session, false);
            debugMessage.WatchData = new DebugWatchData();
            debugMessage.WatchData.Names.Add(evaluationObject.Expression);
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        #endregion


        #region 断点管理

        private void AddBreakPoint(int sessionId, CallStack breakPoint)
        {
            if (_breakPoints.ContainsKey(sessionId) && _breakPoints[sessionId].Contains(breakPoint))
            {
                return;
            }
            if (!_breakPoints.ContainsKey(sessionId))
            {
                _breakPoints.Add(sessionId, new List<CallStack>(Constants.DefaultRuntimeSize));
            }
            _breakPoints[sessionId].Add(breakPoint);
            RuntimeState runtimeState = _globalInfo.StateMachine.State;
            if (runtimeState == RuntimeState.Running || runtimeState == RuntimeState.Blocked ||
                runtimeState == RuntimeState.DebugBlocked)
            {
                SendBreakPointUpdateMessage(sessionId, MessageNames.AddBreakPointName, breakPoint);
            }
        }

        private void RemoveBreakPoint(int sessionId, CallStack breakPoint)
        {
            if (null != breakPoint && !_breakPoints.ContainsKey(sessionId) ||
                !_breakPoints[sessionId].Contains(breakPoint))
            {
                return;
            }
            // breakPoint为null时会删除所有的断点
            if (null == breakPoint)
            {
                _breakPoints[sessionId].Clear();
            }
            else
            {
                _breakPoints[sessionId].Remove(breakPoint);
            }

            RuntimeState runtimeState = _globalInfo.StateMachine.State;
            if (runtimeState == RuntimeState.Running || runtimeState == RuntimeState.Blocked ||
                runtimeState == RuntimeState.DebugBlocked)
            {
                SendBreakPointUpdateMessage(sessionId, MessageNames.DelBreakPointName, breakPoint);
            }
        }

        public void SendInitBreakPointMessage(int sessionId)
        {
            if (_breakPoints.ContainsKey(sessionId))
            {
                DebugMessage debugMessage = new DebugMessage(MessageNames.AddBreakPointName, sessionId,
                    _breakPoints[sessionId], true);
                _globalInfo.MessageTransceiver.Send(debugMessage);
            }
        }

        private void SendBreakPointUpdateMessage(int sessionId, string messageName, CallStack callStack)
        {
            DebugMessage debugMessage = new DebugMessage(messageName, sessionId, callStack, true);
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        #endregion


        #region 调试流程控制

        public void StepInto()
        {
            SendRunDebugStepMessage(MessageNames.StepIntoName);
        }

        public void StepOver()
        {
            SendRunDebugStepMessage(MessageNames.StepOverName);
        }

        public void RunToEnd()
        {
            SendRunDebugStepMessage(MessageNames.RunToEndName);
        }

        public void Continue()
        {
            SendRunDebugStepMessage(MessageNames.ContinueName);
        }

        private void SendRunDebugStepMessage(string messageName)
        {
            if (_debugHitSession == Constants.NoDebugHitSession)
            {
                return;
            }
            int debugSession = Interlocked.Exchange(ref _debugHitSession, Constants.NoDebugHitSession);
            DebugMessage debugMessage = new DebugMessage(messageName, debugSession, true);
            _globalInfo.MessageTransceiver.Send(debugMessage);

            DebugEventInfo debugEvent = new DebugEventInfo(debugMessage);
            _globalInfo.EventQueue.Enqueue(debugEvent);
        }
        
        #endregion

        public bool HandleMessage(MessageBase message)
        {
            DebugMessage debugMessage = (DebugMessage)message;
            switch (message.Name)
            {
                case MessageNames.BreakPointHitName:
                    Thread.VolatileWrite(ref _debugHitSession, debugMessage.Id);
                    DebugEventInfo debugEvent = new DebugEventInfo(debugMessage);
                    _globalInfo.EventQueue.Enqueue(debugEvent);
                    break;
                case MessageNames.AddBreakPointName:

                    break;
                case MessageNames.DelBreakPointName:
                    break;
                case MessageNames.RequestValueName:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                    break;
            }
            return true;
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize(ISequenceFlowContainer sequenceData)
        {
            this._sequenceData = sequenceData;
        }

        #region 运行时对象处理

        public void AddObject(RuntimeObject runtimeObject)
        {
            switch (runtimeObject.ObjectName)
            {
                case Constants.BreakPointObjectName:
                    BreakPointObject breakPointObject = (BreakPointObject)runtimeObject;
                    AddBreakPoint(breakPointObject.BreakPoint.Session, breakPointObject.BreakPoint);
                    break;
                case Constants.WatchDataObjectName:
                    WatchDataObject watchDataObject = (WatchDataObject)runtimeObject;
                    AddWatchVariable(watchDataObject.Session, watchDataObject);
                    break;
                case Constants.EvaluationObjectName:
                    EvaluateExpression((EvaluationObject)runtimeObject);
                    break;
            }
        }

        public void RemoveObject(RuntimeObject runtimeObject)
        {
            switch (runtimeObject.ObjectName)
            {
                case Constants.BreakPointObjectName:
                    BreakPointObject breakPointObject = (BreakPointObject)runtimeObject;
                    RemoveBreakPoint(breakPointObject.BreakPoint.Session, breakPointObject.BreakPoint);
                    break;
                case Constants.WatchDataObjectName:
                    WatchDataObject watchDataObject = (WatchDataObject)runtimeObject;
                    RemoveWatchVariable(watchDataObject.Session, watchDataObject);
                    break;
                case Constants.EvaluationObjectName:
                    // ignore
                    break;
            }
        }

        #endregion

    }
}