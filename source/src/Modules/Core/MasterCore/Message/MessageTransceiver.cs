using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.MasterCore.Common;
using Testflow.Modules;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Message
{
    /// <summary>
    /// 消息收发器，维护引擎侧的所有消息的吞吐
    /// </summary>
    internal abstract class MessageTransceiver : IDisposable
    {
        protected readonly ZombieMessageCleaner ZombieCleaner;

        public static MessageTransceiver GetTransceiver(ModuleGlobalInfo globalInfo, bool isSyncMessenger)
        {
            // TODO 目前只实现了同步处理方式，异步处理后期实现
            if (isSyncMessenger)
            {
                return new SyncMsgTransceiver(globalInfo);
            }
            else
            {
                return new AsyncMsgTransceiver(globalInfo);
            }
        }

        protected readonly Messenger UpLinkMessenger;
        protected readonly Messenger DownLinkMessenger;

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

        private readonly Dictionary<string, IMessageHandler> _consumers;

        private SpinLock _operationLock;

        protected ModuleGlobalInfo GlobalInfo;

        protected MessageTransceiver(ModuleGlobalInfo globalInfo, ReceiveType receiveType)
        {
            this.GlobalInfo = globalInfo;
            // 创建上行队列
            FormatterType formatterType = GlobalInfo.ConfigData.GetProperty<FormatterType>("EngineQueueFormat");
            MessengerOption receiveOption = new MessengerOption(CoreConstants.UpLinkMQName, GetMessageType)
            {
                Type = MessengerType.MSMQ,
                HostAddress = Constants.LocalHostAddr,
                ReceiveType = receiveType,
                Formatter = formatterType
            };
            UpLinkMessenger = Messenger.GetMessenger(receiveOption);
            this._consumers = new Dictionary<string, IMessageHandler>(Constants.DefaultRuntimeSize);
            // 创建下行队列
            MessengerOption sendOption = new MessengerOption(CoreConstants.DownLinkMQName, GetMessageType)
            {
                Type = MessengerType.MSMQ,
                HostAddress = Constants.LocalHostAddr,
                ReceiveType = receiveType,
                Formatter = formatterType
            };
            DownLinkMessenger = Messenger.GetMessenger(sendOption);
            this._operationLock = new SpinLock();

            this.ZombieCleaner = new ZombieMessageCleaner(DownLinkMessenger, globalInfo);
        }

        protected abstract void Start();
        protected abstract void Stop();
        protected abstract void SendMessage(MessageBase message);

        public void AddConsumer(string messageType, IMessageHandler handler)
        {
            _consumers.Add(messageType, handler);
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
                Start();
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
                Stop();
                UpLinkMessenger.Clear();
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

            GlobalInfo.LogService.Print(LogLevel.Debug, CommonConst.PlatformLogSession,
                $"Send message, Type:{message.Type}, Index:{message.Index}.");
        }

        protected IMessageHandler GetConsumer(MessageBase message)
        {
            string messageType = message.Type.ToString();
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
            _operationLock.TryEnter(Constants.OperationTimeout, ref getLock);
            if (!getLock)
            {
                GlobalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, "Operation Timeout");
                throw new TestflowRuntimeException(ModuleErrorCode.OperationTimeout, GlobalInfo.I18N.GetStr("OperatoinTimeout"));
            }
        }

        protected void FreeOperationLock()
        {
            _operationLock.Exit();
        }

        private int _stopFlag = 0;
        public virtual void Dispose()
        {
            if (_stopFlag == 1)
            {
                return;
            }
            Stop();
            UpLinkMessenger.Clear();
            DownLinkMessenger.Clear();
            UpLinkMessenger.Dispose();
            DownLinkMessenger.Dispose();
            Thread.VolatileWrite(ref _stopFlag, 1);
        }

        // 为了提高效率，暂时写死，后续优化
        public Type GetMessageType(string typeName)
        {
            switch (typeName)
            {
                case "CallBackMessage":
                    return typeof (CallBackMessage);
                    break;
                case "ControlMessage":
                    return typeof (ControlMessage);
                    break;
                case "DebugMessage":
                    return typeof (DebugMessage);
                    break;
                case "ResourceSyncMessage":
                    return typeof (ResourceSyncMessage);
                    break;
                case "RmtGenMessage":
                    return typeof (RmtGenMessage);
                    break;
                case "RuntimeErrorMessage":
                    return typeof (RuntimeErrorMessage);
                    break;
                case "StatusMessage":
                    return typeof (StatusMessage);
                    break;
                case "TestGenMessage":
                    return typeof (TestGenMessage);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public void Clear()
        {
            UpLinkMessenger?.Clear();
            DownLinkMessenger?.Clear();
        }
    }
}