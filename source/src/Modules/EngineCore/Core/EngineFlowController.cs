using Testflow.EngineCore.Common;
using Testflow.EngineCore.Message;
using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Core
{
    /// <summary>
    /// 实现引擎的运行时流程管理功能
    /// </summary>
    internal class EngineFlowController : IMessageHandler
    {
        private readonly ModuleGlobalInfo _globalInfo;
        public EngineFlowController(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
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