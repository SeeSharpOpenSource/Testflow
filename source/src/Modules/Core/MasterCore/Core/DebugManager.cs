using System.Collections.Generic;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
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

        public DebugManager(ModuleGlobalInfo globalInfo)
        {
            this._globalInfo = globalInfo;
            _watchVariables = new Dictionary<int, List<string>>(Constants.DefaultRuntimeSize);
        }

        public void AddDebugVariables(int session, IVariable variable)
        {
            string variableName = CoreUtils.GetRuntimeVariableName(variable);
            if (!_watchVariables.ContainsKey(session))
            {
                _watchVariables.Add(session, new List<string>(Constants.DefaultRuntimeSize));
            }
            if (!_watchVariables[session].Contains(variableName))
            {
                _watchVariables[session].Add(variableName);
            }
            if (_globalInfo.StateMachine.State == RuntimeState.Running)
            {
                SendRefreshWatchMessage(session);
            }
        }

        public void RemoveDebugVariables(int session, IVariable variable)
        {
            string variableName = CoreUtils.GetRuntimeVariableName(variable);
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
            if (_globalInfo.StateMachine.State == RuntimeState.Running)
            {
                SendRefreshWatchMessage(session);
            }
        }

        public void AddBreakPoint(int sessionId, CallStack callStack)
        {
            DebugMessage debugMessage = new DebugMessage(MessageNames.DownDebugMsgName, sessionId,
                DebugMessageType.AddBreakPoint)
            {
                Stack = callStack
            };
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        public void RemoveBreakPoint(int sessionId, CallStack callStack)
        {
            DebugMessage debugMessage = new DebugMessage(MessageNames.DownDebugMsgName, sessionId, 
                DebugMessageType.RemoveBreakPoint)
            {
                Stack = callStack
            };
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        private void SendRefreshWatchMessage(int sessionId)
        {
            DebugMessage debugMessage = new DebugMessage(MessageNames.DownDebugMsgName, sessionId,
                DebugMessageType.RemoveBreakPoint)
            {
                Data = new DebugData()
            };
            if (_watchVariables.ContainsKey(sessionId))
            {
                debugMessage.Data.Names.AddRange(_watchVariables[sessionId]);
            }
            _globalInfo.MessageTransceiver.Send(debugMessage);
        }

        public bool HandleMessage(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}