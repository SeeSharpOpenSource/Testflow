using Testflow.CoreCommon.Data;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Core
{
    /// <summary>
    /// 调试状态管理
    /// </summary>
    internal class DebugManager : IMessageHandler
    {
        private readonly ModuleGlobalInfo _globalInfo;
        public DebugManager(ModuleGlobalInfo globalInfo)
        {
            this._globalInfo = globalInfo;
        }

        public void SendBreakPoint(int sessionId, CallStack callStack)
        {
//            DebugMessage debugMessage = new DebugMessage();
        }

        public void RemoveBreakPoint(int sessionId, CallStack callStack)
        {
            //TODO
        }

        public void HandleMessage(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}