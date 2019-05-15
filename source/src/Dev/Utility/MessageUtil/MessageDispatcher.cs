using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.Usr;

namespace Testflow.Utility.MessageUtil
{
    /// <summary>
    /// 消息分发器
    /// </summary>
    internal class MessageDispatcher
    {
        private readonly Messenger _messenger;
        private readonly IDictionary<int, IList<IMessageConsumer>> _consumers;

        public int DispatchInterval { get; set; }
        
        // TODO 暂未实现其他高阶功能和流量控制，后续完善
        public MessageDispatcher(Messenger messenger)
        {
            // 暂时使用异步接收方式
            this._messenger = messenger;
            this._consumers = new Dictionary<int, IList<IMessageConsumer>>(UtilityConstants.DefaultEntityCount);
            this._messenger.MessageReceived += DispatchMessage;
        }

        public void RegisterConsumer(IMessageConsumer consumer)
        {
            if (consumer.SessionId == CommonConst.BroadcastSession)
            {
                foreach (int session in _consumers.Keys)
                {
                    _consumers[session].Add(consumer);
                }
            }
            else
            {
                if (!_consumers.ContainsKey(consumer.SessionId))
                {
                    _consumers.Add(consumer.SessionId, new List<IMessageConsumer>(2));
                }
                _consumers[consumer.SessionId].Add(consumer);
            }
        }

        public void UnregisterConsumer(IMessageConsumer consumer)
        {
            if (consumer.SessionId == CommonConst.BroadcastSession)
            {
                foreach (int session in _consumers.Keys)
                {
                    _consumers[session].Remove(consumer);
                }
            }
            else
            {
                if (_consumers.ContainsKey(consumer.SessionId))
                {
                    _consumers[consumer.SessionId].Remove(consumer);
                }
                
            }
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
            if (message.Id == CommonConst.BroadcastSession)
            {
                foreach (IList<IMessageConsumer> messageConsumers in _consumers.Values)
                {
                    foreach (IMessageConsumer messageConsumer in messageConsumers)
                    {
                        messageConsumer.Handle(message);
                    }
                }
            }
            else
            {
                foreach (IMessageConsumer messageConsumer in _consumers[message.Id])
                {
                    messageConsumer.Handle(message);
                }
            }
        }
    }
}