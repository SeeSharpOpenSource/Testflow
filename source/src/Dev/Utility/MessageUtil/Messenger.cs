using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Testflow.Common;
using Testflow.i18n.I18nUtil;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.MessageUtil.Messengers;

namespace Testflow.Utility.MessageUtil
{
    /// <summary>
    /// 信使类
    /// </summary>
    public abstract class Messenger : IDisposable
    {
        private static ConcurrentBag<Messenger> _messengers;

        static Messenger()
        {
            // 初始化i18n模块
            I18NOption i18NOption = new I18NOption(typeof(Messenger).Assembly, "Resources.i18n_messenger_en", "Resources.i18n_messenger_en")
            {
                Name = Constants.MessengerName
            };
            _messengers = new ConcurrentBag<Messenger>(new List<Messenger>(2));
        }

        /// <summary>
        /// 接收到消息后的委托
        /// </summary>
        /// <param name="args"></param>
        internal delegate void MessageReceivedAction(IMessage args);

        /// <summary>
        /// 接收到消息后的事件
        /// </summary>
        internal event MessageReceivedAction MessageReceived;

        /// <summary>
        /// 事件回调
        /// </summary>
        protected void OnMessageReceived(IMessage message)
        {
            MessageReceived?.Invoke(message);
        }

        /// <summary>
        /// 使用option创建信使类，如果已存在抛出异常
        /// </summary>
        public static Messenger CreateMessenger(MessengerOption option)
        {
            //此处不存在并发写入的同一个元素的情况，所以未加锁保护
            Messenger messenger;
            if (null == (messenger = _messengers.First(item => item.Option.Equals(option))))
            {
                switch (option.Type)
                {
                    case MessengerType.MSMQ:
                        messenger = new MsmqMessenger(option);
                        break;
                    case MessengerType.Kafka:
                        throw new NotImplementedException();
                        break;
                    default:
                        break;
                }
                _messengers.Add(messenger);
            }
            return messenger;
        }

        /// <summary>
        /// 使用option获取已配置的信使类，如果不存在抛出异常
        /// </summary>
        public static Messenger GetMessenger(MessengerOption option)
        {
            Messenger messenger = _messengers.First(item => item.Option.Equals(option));
            if (null == messenger)
            {
                I18N i18N = I18N.GetInstance(Constants.MessengerName);
                throw new TestflowRuntimeException(TestflowErrorCode.MessengerRuntimeError, i18N.GetStr("MessengerNotInitialized"));
            }
            return messenger;
        }

        private MessageDispatcher _messageDispatcher;

        /// <summary>
        /// 常见Messanger抽象类
        /// </summary>
        /// <param name="option"></param>
        protected Messenger(MessengerOption option)
        {
            this.Option = option;
        }

        /// <summary>
        /// 信使类的选项
        /// </summary>
        public MessengerOption Option { get; }

        /// <summary>
        /// 当前未消化的消息数量
        /// </summary>
        public abstract int MessageCount { get; }

        /// <summary>
        /// 初始化信使类
        /// </summary>
        /// <param name="consumers"></param>
        public virtual void Initialize(IList<IMessageConsumer> consumers)
        {
            this.InitializeMessageQueue();
            this.RegisterEvent();
            _messageDispatcher = new MessageDispatcher(this, consumers);
        }

        /// <summary>
        /// 接收信息，未添加高阶参数配置，后续再更新
        /// </summary>
        internal abstract IMessage Receive();

        /// <summary>
        /// 发送消息，未添加高阶配置，后续再更新
        /// </summary>
        /// <param name="message">待发送的消息</param>
        /// <param name="format">格式器类型</param>
        /// <param name="targetTypes">目标类型</param>
        internal abstract bool Send(IMessage message, FormatterType format, params Type[] targetTypes);

        /// <summary>
        /// 初始化消息队列
        /// </summary>
        public abstract void InitializeMessageQueue();

        /// <summary>
        /// 注册事件
        /// </summary>
        protected abstract void RegisterEvent();

        /// <summary>
        /// 销毁信使实例
        /// </summary>
        public abstract void Dispose();
    }
}
