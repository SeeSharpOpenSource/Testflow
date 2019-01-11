using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Testflow.Utility.MessageUtil
{
    /// <summary>
    /// 日志消息分发器
    /// </summary>
    internal class MessageDispatcher
    {
        private readonly Messenger _messenger;
        private readonly IDictionary<int, IMessageConsumer> _consumers;

        public int DispatchInterval { get; set; }
        
        // TODO 暂未实现其他高阶功能和流量控制，后续完善
        public MessageDispatcher(Messenger messenger, IList<IMessageConsumer> consumers)
        {
            // 暂时使用异步接收方式
            this._messenger = messenger;
            this._consumers = new Dictionary<int, IMessageConsumer>(consumers.Count);
            foreach (IMessageConsumer consumer in consumers)
            {
                _consumers.Add(consumer.SeesionId, consumer);
            }
            this._messenger.MessageReceived += DispatchMessage;
        }

        /// <summary>
        /// 消息分发
        /// </summary>
        private void DispatchMessage(IMessage message)
        {
            // TODO Exception handling
            if (null == message || !_consumers.ContainsKey(message.Id))
            {
                return;
            }
            _consumers[message.Id].Handle(message);
        }
    }
}