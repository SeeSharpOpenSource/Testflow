using System;
using System.Messaging;
using System.Text;
using System.Threading;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.Utility.MessageUtil.Messengers
{
    internal class MsmqMessenger : Messenger
    {
        private MessageQueue _messageQueue;
        private IMessageFormatter _formatter;

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

        public override bool Send(IMessage message)
        {
            if (!_messageQueue.CanWrite)
            {
                return false;
            }
            Message packagedMessage = new Message(message, _formatter);
            // 在Label中发送消息的类型名
            _messageQueue.Send(packagedMessage, message.GetType().Name);
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
            switch (Option.Formatter)
            {
                case FormatterType.Xml:
                    _formatter = new XmlMessageFormatter(Option.TargetTypes);
                    break;
                case FormatterType.Bin:
                    _formatter = new BinaryMessageFormatter();
                    break;
                case FormatterType.Json:
                    _formatter = new JsonMessageFormatter(Option, Encoding.UTF8);
                    break;
                default:
                    break;
            }
            this._messageQueue.Formatter = _formatter;
//            this._messageQueue.Purge();
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
                throw new TestflowRuntimeException(ModuleErrorCode.MessengerReceiveError, ex.Message, ex);
            }
            this._messageQueue.BeginReceive();
        }

        private int _diposedFlag = 0;
        public override void Dispose()
        {
            if (_diposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _diposedFlag, 1);
            Thread.MemoryBarrier();
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