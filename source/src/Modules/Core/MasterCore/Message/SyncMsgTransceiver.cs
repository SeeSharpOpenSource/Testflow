using System;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.MasterCore.Common;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Message
{
    /// <summary>
    /// 同步消息收发器
    /// </summary>
    internal class SyncMsgTransceiver : MessageTransceiver
    {
        private Thread _receiveThread;
        private CancellationTokenSource _cancellation;
        // 考虑到Status接收器的负载可能较大，所以额外使用一个线程处理
        private Thread _engineMessageListener;
        private readonly LocalMessageQueue<MessageBase> _engineMessageQueue;

        private Thread _statusMessageListener;
        private readonly LocalMessageQueue<MessageBase> _statusMessageQueue;

        public SyncMsgTransceiver(ModuleGlobalInfo globalInfo) : base(globalInfo)
        {
            _engineMessageQueue = new LocalMessageQueue<MessageBase>(Constants.DefaultEventsQueueSize);
            _statusMessageQueue = new LocalMessageQueue<MessageBase>(Constants.DefaultEventsQueueSize);
        }

        protected override void Start()
        {
            _receiveThread = new Thread(SynchronousReceive)
            {
                IsBackground = true
            };
            _cancellation = new CancellationTokenSource();
            _receiveThread.Start();
            ZombieCleaner.Start();


            _engineMessageListener = new Thread(MessageProcessingLoop)
            {
                Name = "EngineMessageListener",
                IsBackground = true
            };

            _statusMessageListener = new Thread(MessageProcessingLoop)
            {
                Name = "EngineMessageListener",
                IsBackground = true
            };
            _engineMessageListener.Start(_engineMessageQueue);
            _statusMessageListener.Start(_statusMessageQueue);
        }

        protected override void Stop()
        {
            _cancellation.Cancel();
            if (null != _receiveThread && ThreadState.Running == _receiveThread.ThreadState && !_receiveThread.Join(Constants.OperationTimeout))
            {
                _receiveThread.Abort();
                GlobalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                    "Message receive thread stopped abnormally");
            }
            ZombieCleaner.Stop();
        }

        protected override void SendMessage(MessageBase message)
        {
            DownLinkMessenger.Send(message, FormatterType);
        }

        private void SynchronousReceive(object state)
        {
            while (!_cancellation.IsCancellationRequested)
            {
                IMessage message = UpLinkMessenger.Receive();
                IMessageHandler handler = GetConsumer(message);
                handler.HandleMessage(message);
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            Stop();
        }

        private void MessageProcessingLoop(object queueObj)
        {
            LocalMessageQueue<MessageBase> queue = queueObj as LocalMessageQueue<MessageBase>;
            while (true)
            {
                MessageBase message = queue.WaitUntilMessageCome();
                bool operationContinue = Consumers[message.Type.ToString()].HandleMessage(message);
                // 如果消息执行后确认需要停止，则结束消息队列的处理。
                if (!operationContinue)
                {
                    break;
                }
            }
        }
    }
}