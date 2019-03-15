using Testflow.EngineCore.Common;
using Testflow.Utility.MessageUtil;
using IMessageConsumer = Testflow.EngineCore.Message.IMessageConsumer;

namespace Testflow.EngineCore.Core
{
    /// <summary>
    /// 实现引擎的运行时流程管理功能
    /// </summary>
    internal class EngineFlowController : IMessageConsumer
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