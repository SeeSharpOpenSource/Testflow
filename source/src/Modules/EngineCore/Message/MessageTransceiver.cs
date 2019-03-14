using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Common;
using Testflow.EngineCore.Common;
using Testflow.EngineCore.Message.Messages;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Message
{
    internal abstract class MessageTransceiver : IDisposable
    {
        public static MessageTransceiver GetTransceiver(ModuleGlobalInfo globalInfo, bool isSyncDispatch)
        {
            if (isSyncDispatch)
            {
                return new SyncMsgTransceiver(globalInfo);
            }
            else
            {
                return new AsyncMsgTransceiver(globalInfo);
            }
        }

        protected Messenger Messenger;

        private byte _activated = 0;
        protected bool Activated
        {
            get { return 0 != _activated; }
            set
            {
                byte isActivate = value ? (byte)1 : (byte)0;
                Thread.VolatileWrite(ref _activated, isActivate);
            }
        }

        private readonly Dictionary<string, IMessageConsumer> _consumers;

        protected SpinLock OperationLock;

        protected ModuleGlobalInfo GlobalInfo;
        protected FormatterType FormatterType;

        protected MessageTransceiver(ModuleGlobalInfo globalInfo)
        {
            this.GlobalInfo = globalInfo;
            FormatterType = GlobalInfo.ConfigData.GetProperty<FormatterType>("EngineQueueFormat");
            MessengerOption option = new MessengerOption(Constants.MsgQueueName, typeof (ControlMessage),
                typeof (DebugMessage), typeof (RmtGenMessage), typeof (StatusMessage), typeof (TestGenMessage))
            {
                Type = MessengerType.MSMQ,
                HostAddress = Constants.LocalHostAddr,
                ReceiveType = ReceiveType.Synchronous
            };
            Messenger = Messenger.GetMessenger(option);
            this.OperationLock = new SpinLock();
            this._consumers = new Dictionary<string, IMessageConsumer>(Constants.DefaultRuntimeSize);
        }

        protected abstract void StartReceive();
        protected abstract void StopReceive();
        protected abstract void SendMessage(MessageBase message);

        public void AddConsumer(string messageType, IMessageConsumer consumer)
        {
            _consumers.Add(messageType, consumer);
        }

        /// <summary>
        /// 打开消息收发功能
        /// </summary>
        public void Activate()
        {
            GetOperationLock();
            try
            {
                if (Activated)
                {
                    return;
                }
                // TODO 目前只实现功能，未添加状态监控等功能，后期有时间再处理
                StartReceive();
                Messenger.Clear();
                Activated = true;
            }
            finally
            {
                FreeOperationLock();
            }
        }

        /// <summary>
        /// 暂停消息收发功能
        /// </summary>
        public void Deactivate()
        {
            GetOperationLock();
            try
            {
                if (!Activated)
                {
                    return;
                }
                StartReceive();
                StopReceive();
                Messenger.Clear();
                Activated = false;
            }
            finally
            {
                FreeOperationLock();
            }
        }

        public void Send(MessageBase message)
        {
            if (!Activated)
            {
                GlobalInfo.LogService.Print(LogLevel.Debug, CommonConst.PlatformLogSession, 
                    "Cannot send message when messenger is deactivated");
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidOperation, 
                    GlobalInfo.I18N.GetStr("CannotSendWhenDeactive"));
            }
            SendMessage(message);
        }

        protected IMessageConsumer GetConsumer(IMessage message)
        {
            string messageType = message.GetType().Name;
            if (!_consumers.ContainsKey(messageType))
            {
                throw new TestflowRuntimeException(ModuleErrorCode.UnregisteredMessage, 
                    GlobalInfo.I18N.GetFStr("UnregisteredMessage", messageType));
            }
            return _consumers[messageType];
        }

        protected void GetOperationLock()
        {
            bool getLock = false;
            OperationLock.TryEnter(Constants.OperationTimeout, ref getLock);
            if (!getLock)
            {
                GlobalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, "Operation Timeout");
                throw new TestflowRuntimeException(ModuleErrorCode.OperationTimeout, GlobalInfo.I18N.GetStr("OperatoinTimeout"));
            }
        }

        protected void FreeOperationLock()
        {
            OperationLock.Exit();
        }

        public virtual void Dispose()
        {
            Messenger.Dispose();
        }
    }
}