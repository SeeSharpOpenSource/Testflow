using Testflow.EngineCore.Common;
using Testflow.Utility.MessageUtil;
using IMessageConsumer = Testflow.EngineCore.Message.IMessageConsumer;

namespace Testflow.EngineCore.Core
{
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