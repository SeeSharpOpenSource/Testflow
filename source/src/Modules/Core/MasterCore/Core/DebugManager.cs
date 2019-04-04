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
using Testflow.Runtime;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Core
{
    /// <summary>
    /// 调试状态管理
    /// </summary>
    internal class DebugManager : IMessageHandler
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private readonly Dictionary<int, List<string>> _watchVariables;
        private readonly Dictionary<int, List<CallStack>> _breakPoints;
        private int _debugHitSession;

        public DebugManager(ModuleGlobalInfo globalInfo)
        {
            this._globalInfo = globalInfo;
            _watchVariables = new Dictionary<int, List<string>>(Constants.DefaultRuntimeSize);
            _breakPoints = new Dictionary<int, List<CallStack>>(Constants.DefaultRuntimeSize);
            _debugHitSession = Constants.NoDebugHitSession;
        }

        public void AddWatchVariable(int session, IVariable variable)
        {
            string variableName = CoreUtils.GetRuntimeVariableName(session, variable);
            if (!_watchVariables.ContainsKey(session))
            {
                _watchVariables.Add(session, new List<string>(Constants.DefaultRuntimeSize));
            }
            if (!_watchVariables[session].Contains(variableName))
            {
                _watchVariables[session].Add(variableName);
            }
            RuntimeState runtimeState = _globalInfo.StateMachine.State;
            if (runtimeState == RuntimeState.Running || runtimeState == RuntimeState.Blocked ||
                runtimeState == RuntimeState.DebugBlocked)
            {
                SendRefreshWatchMessage(session);
            }
        }

        public void RemoveWatchVariable(int session, IVariable variable)
        {
            string variableName = CoreUtils.GetRuntimeVariableName(session, variable);
            if (!_watchVariables.ContainsKey(session))
            {
                return;
            }
            if (_watchVariables[session].Contains(variableName))
            {
                _watchVariables[session].Remove(variableName);
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

        public void AddBreakPoint(int sessionId, CallStack breakPoint)
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
                SendAddBreakPointMessage(sessionId, breakPoint);
            }
        }

        public void RemoveBreakPoint(int sessionId, CallStack breakPoint)
        {
            if (null != breakPoint && !_breakPoints.ContainsKey(sessionId) || !_breakPoints[sessionId].Contains(breakPoint))
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
                SendRemoveBreakPointMessage(sessionId, breakPoint);
            }
        }

        public void SendInitBreakPointMessage(int sessionId)
        {
            if (_breakPoints.ContainsKey(sessionId))
            {
                foreach (CallStack breakPoint in _breakPoints[sessionId])
                {
                    SendAddBreakPointMessage(sessionId, breakPoint);
                }
            }
        }

        public void Continue()
        {
            if (_debugHitSession == Constants.NoDebugHitSession)
            {
                return;
            }
            DebugMessage debugMessage = new DebugMessage(MessageNames.DownDebugMsgName, _debugHitSession,
                DebugMessageType.FreeDebugBlock);
            _globalInfo.MessageTransceiver.Send(debugMessage);

            DebugEventInfo debugEvent = new DebugEventInfo(debugMessage);
            _globalInfo.EventQueue.Enqueue(debugEvent);
            this._debugHitSession = Constants.NoDebugHitSession;
        }

        private void SendAddBreakPointMessage(int sessionId, CallStack callStack)
        {
            DebugMessage debugMessage = new DebugMessage(MessageNames.DownDebugMsgName, sessionId,
                DebugMessageType.AddBreakPoint)
            {
                BreakPoint = callStack
            };
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        private void SendRemoveBreakPointMessage(int sessionId, CallStack callStack)
        {
            DebugMessage debugMessage = new DebugMessage(MessageNames.DownDebugMsgName, sessionId,
                DebugMessageType.RemoveBreakPoint)
            {
                BreakPoint = callStack
            };
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        public void SendRefreshWatchMessage(int sessionId)
        {
            DebugMessage debugMessage = new DebugMessage(MessageNames.DownDebugMsgName, sessionId,
                DebugMessageType.RefreshWatch)
            {
                WatchData = new DebugWatchData()
            };
            if (_watchVariables.ContainsKey(sessionId))
            {
                debugMessage.WatchData.Names.AddRange(_watchVariables[sessionId]);
            }
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        public bool HandleMessage(MessageBase message)
        {
            DebugMessage debugMessage = (DebugMessage)message;
            DebugMessageType messageType = debugMessage.DebugMsgType;
            switch (messageType)
            {
                case DebugMessageType.BreakPointHitted:
                    Thread.VolatileWrite(ref _debugHitSession, debugMessage.Id);
                    DebugEventInfo debugEvent = new DebugEventInfo(debugMessage);
                    _globalInfo.EventQueue.Enqueue(debugEvent);
                    break;
                default:
                    throw new ArgumentException();
            }
            return true;
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            
        }
    }
}