using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Message
{
    internal interface IMessageHandler
    {
        /// <summary>
        /// 同步处理消息
        /// </summary>
        void HandleMessage(IMessage message);

        /// <summary>
        /// 异步添加消息到待处理队列
        /// </summary>
        void AddToQueue(IMessage message);
    }
}