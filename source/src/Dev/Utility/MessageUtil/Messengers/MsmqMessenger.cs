using System;
using System.Messaging;
using Testflow.Common;
using Testflow.Utility.I18nUtil;

namespace Testflow.Utility.MessageUtil.Messengers
{
    internal class MsmqMessenger : Messenger
    {
        private MessageQueue _messageQueue;
        public MsmqMessenger(MessengerOption option) : base(option)
        {
        }

        public override int MessageCount => _messageQueue.GetAllMessages().Length;

        internal override IMessage Receive()
        {
            if (!_messageQueue.CanRead)
            {
                return null;
            }
            Message message = _messageQueue.Receive();
            return message?.Body as IMessage;
        }

        public override bool Send(IMessage message, FormatterType format, params Type[] targetTypes)
        {
            if (!_messageQueue.CanWrite)
            {
                return false;
            }
            IMessageFormatter formatter = null;
            switch (format)
            {
                    case FormatterType.Xml:
                    formatter = new XmlMessageFormatter(targetTypes);
                    break;
                case FormatterType.Bin:
                    formatter = new BinaryMessageFormatter();
                    break;
                default:
                    break;

            }
            Message packagedMessage = new Message(message, formatter);
            _messageQueue.Send(packagedMessage);
            return true;
        }

        public override void InitializeMessageQueue()
        {
            if (MessageQueue.Exists(Option.Path))
            {
                I18N i18N = I18N.GetInstance(Constants.MessengerName);
                throw new TestflowRuntimeException(TestflowErrorCode.MessengerRuntimeError, 
                    i18N.GetFStr("MessageQueueExist", Option.Path));
            }
            this._messageQueue = new MessageQueue(Option.Path);
            this._messageQueue.Purge();
        }

        protected override void RegisterEvent()
        {
            this._messageQueue.ReceiveCompleted += CallMessageReceivedEvent;
        }

        private void CallMessageReceivedEvent(object sender, ReceiveCompletedEventArgs args)
        {
            // TODO 未处理其他参数，后期实现
            IMessage message = args.Message.Body as IMessage;
            if (null == message)
            {
                return;
            }
            this.OnMessageReceived(message);
        }

        public override void Dispose()
        {
            if (0 != MessageCount)
            {
                //TODO 未添加日志
            }
            this._messageQueue.Dispose();
        }

        public override void Clear()
        {
            _messageQueue.Purge();
        }
    }
}