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

        public override IMessage Receive(params Type[] targetTypes)
        {
            if (!_messageQueue.CanRead)
            {
                return null;
            }
            Message message = _messageQueue.Receive();
            return message?.Body as IMessage;
        }

        public override IMessage Peak(params Type[] targetTypes)
        {
            if (!_messageQueue.CanRead)
            {
                return null;
            }
            Message message = _messageQueue.Peek();
            return message?.Body as IMessage;
        }

        public override bool Send(IMessage message, FormatterType format, params Type[] targetTypes)
        {
            if (!_messageQueue.CanWrite)
            {
                return false;
            }
            IMessageFormatter formatter = this._messageQueue.Formatter;
            // 如果Option中没有配置目标类型，则说明每次的Formatter需要单独配置
            if (null == Option.TargetTypes || 0 == Option.TargetTypes.Length)
            {
                targetTypes = Option.TargetTypes;
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
            }
            Message packagedMessage = new Message(message, formatter);
            _messageQueue.Send(packagedMessage);
            return true;
        }

        public override void InitializeMessageQueue()
        {
            if (MessageQueue.Exists(Option.Path))
            {
                // TODO 暂时无法解决如果两个Queue在不同的AppDomain时也会报错的问题，所以暂时屏蔽该异常
//                I18N i18N = I18N.GetInstance(Constants.MessengerName);
//                throw new TestflowRuntimeException(TestflowErrorCode.MessengerRuntimeError, 
//                    i18N.GetFStr("MessageQueueExist", Option.Path));
                this._messageQueue = new MessageQueue(Option.Path);
            }
            else
            {
                this._messageQueue = MessageQueue.Create(Option.Path);

            }
            if (null != Option.TargetTypes && 0 < Option.TargetTypes.Length)
            {
                IMessageFormatter formatter;
                Type[] targetTypes = Option.TargetTypes;
                switch (Option.Formatter)
                {
                    case FormatterType.Xml:
                        formatter = new XmlMessageFormatter(targetTypes);
                        break;
                    case FormatterType.Bin:
                        formatter = new BinaryMessageFormatter();
                        break;
                    default:
                        throw new NotImplementedException();
                        break;
                }
                this._messageQueue.Formatter = formatter;
            }
            this._messageQueue.Purge();
        }

        protected override void RegisterEvent()
        {
            this._messageQueue.ReceiveCompleted += CallMessageReceivedEvent;
            this._messageQueue.BeginReceive();
        }

        private void CallMessageReceivedEvent(object sender, ReceiveCompletedEventArgs args)
        {
            // TODO 未处理其他参数，后期实现
            // TODO 暂时只实现异步操作
            Message rawMessage = this._messageQueue.EndReceive(args.AsyncResult);
            try
            {
                object body = rawMessage.Body;
                IMessage message = body as IMessage;
                if (null == message)
                {
                    return;
                }
                this.OnMessageReceived(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            this._messageQueue.BeginReceive();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (0 != MessageCount)
            {
                //TODO 未添加日志
            }
            MessageQueue.Delete(this._messageQueue.Path);
        }

        public override void Clear()
        {
            _messageQueue.Purge();
        }
    }
}