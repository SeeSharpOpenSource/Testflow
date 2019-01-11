namespace Testflow.Utility.MessageUtil
{
    /// <summary>
    /// 消息的消费者接口
    /// </summary>
    public interface IMessageConsumer
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        int SeesionId { get; }

        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="message">待处理的消息</param>
        void Handle(IMessage message);
    }
}