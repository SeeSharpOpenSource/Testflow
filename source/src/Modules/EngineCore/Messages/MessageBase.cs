using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Messages
{
    public abstract class MessageBase : IMessage
    {
        public int Id { get; set; }

        /// <summary>
        /// 触发的消息事件名称
        /// </summary>
        public string Name { get; set; }
    }
}