using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Utility.MessageUtil;

namespace Testflow.SlaveCore
{
    internal class MessageTransceiver : IDisposable
    {
        private readonly Messenger _uplinkMessenger;
        private readonly Messenger _downLinkMessenger;
        private FormatterType _formatterType;
        private readonly SlaveContext _slaveContext;
        private readonly LocalMessageQueue<MessageBase> _messageQueue;
        private Thread _peakThread;
        private CancellationTokenSource _cancellation;

        public MessageTransceiver(SlaveContext contextManager, int session)
        {
            this._slaveContext = contextManager;

            // 创建上行队列
            _formatterType = contextManager.GetProperty<FormatterType>("EngineQueueFormat");
            MessengerOption receiveOption = new MessengerOption(CoreConstants.UpLinkMQName, typeof(ControlMessage),
                typeof(DebugMessage), typeof(RmtGenMessage), typeof(StatusMessage), typeof(TestGenMessage))
            {
                Type = contextManager.GetProperty<MessengerType>("MessengerType")
            };
            _uplinkMessenger = Messenger.GetMessenger(receiveOption);
            // 创建下行队列
            MessengerOption sendOption = new MessengerOption(CoreConstants.DownLinkMQName, typeof(ControlMessage),
                typeof(DebugMessage), typeof(RmtGenMessage), typeof(StatusMessage), typeof(TestGenMessage))
            {
                Type = contextManager.GetProperty<MessengerType>("MessengerType")
            };
            _downLinkMessenger = Messenger.GetMessenger(sendOption);

            _messageQueue = new LocalMessageQueue<MessageBase>(CoreConstants.DefaultEventsQueueSize);
            this.SessionId = session;
        }

        public LocalMessageQueue<MessageBase> MessageQueue => _messageQueue;

        public void SendMessage(MessageBase message)
        {
            _uplinkMessenger.Send(message, _slaveContext.GetProperty<FormatterType>("EngineQueueFormat"),
                message.GetType());
        }

        public void StartReceive()
        {
            _messageQueue.Clear();
            _cancellation = new CancellationTokenSource();
            this._peakThread = new Thread(PeakMessage)
            {
                Name = "PeakThread",
                IsBackground = true
            };
            _peakThread.Start();
        }

        public int SessionId { get; }

        private void PeakMessage()
        {
            try
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    IMessage message = _downLinkMessenger.Peak();
                    _messageQueue.Enqueue((MessageBase)message);
                }
            }
            catch (ThreadAbortException)
            {
                _slaveContext.LogSession.Print(LogLevel.Warn, CommonConst.PlatformLogSession, "Transceiver peak thread aborted");
            }
        }

        public void StopReceive()
        {
            _cancellation.Cancel();
            Thread.Sleep(100);
            if (_peakThread.IsAlive)
            {
                _peakThread.Abort();
            }
            _messageQueue.Clear();
        }

        public MessageBase Receive()
        {
            return _messageQueue.WaitUntilMessageCome();
        }

        public void Dispose()
        {
            StopReceive();
        }
    }
}