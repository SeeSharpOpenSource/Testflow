using Testflow.EngineCore.Message.Messages;
using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Message
{
    internal interface IMessageConsumer
    {
        void HandleMessage(IMessage message);

        void AddToQueue(IMessage message);
    }
}