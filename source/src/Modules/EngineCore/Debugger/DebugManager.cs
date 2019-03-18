using Testflow.EngineCore.Common;
using Testflow.EngineCore.Message;
using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Debugger
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

        public void HandleMessage(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}