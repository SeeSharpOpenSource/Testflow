using System;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data.EventInfos;
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

        public SyncMsgTransceiver(ModuleGlobalInfo globalInfo) : base(globalInfo, ReceiveType.Synchronous)
        {
            _engineMessageQueue = new LocalMessageQueue<MessageBase>(Constants.DefaultEventsQueueSize);
            _statusMessageQueue = new LocalMessageQueue<MessageBase>(Constants.DefaultEventsQueueSize);
        }

        protected override void Start()
        {
            _cancellation = new CancellationTokenSource();
            _receiveThread = new Thread(SynchronousReceive)
            {
                Name = "RemoteMessageReceiver",
                IsBackground = true
            };
            _receiveThread.Start();
            ZombieCleaner.Start();

            _engineMessageListener = new Thread(MessageProcessingLoop)
            {
                Name = "EngineMessageListener",
                IsBackground = true
            };

            _statusMessageListener = new Thread(MessageProcessingLoop)
            {
                Name = "StatusMessageListener",
                IsBackground = true
            };
            _engineMessageListener.Start(_engineMessageQueue);
            _statusMessageListener.Start(_statusMessageQueue);
            GlobalInfo.LogService.Print(LogLevel.Info, CommonConst.PlatformLogSession,
                "Message transceiver started.");
        }

        protected override void Stop()
        {
            _cancellation.Cancel();
            ModuleUtils.StopThreadWork(_receiveThread);
            //如果两个队列在被锁的状态则释放锁
            _engineMessageQueue.StopEnqueue();
            _statusMessageQueue.StopEnqueue();

            _engineMessageQueue.FreeLock();
            _statusMessageQueue.FreeLock();

            ModuleUtils.StopThreadWork(_engineMessageListener);
            ModuleUtils.StopThreadWork(_statusMessageListener);
            ZombieCleaner.Stop();

            GlobalInfo.LogService.Print(LogLevel.Info, CommonConst.PlatformLogSession,
                "Message transceiver stopped.");
        }
        
        protected override void SendMessage(MessageBase message)
        {
            DownLinkMessenger.Send(message);
        }

        private void SynchronousReceive(object state)
        {
            try
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    IMessage rawMessage = UpLinkMessenger.Receive();
                    MessageBase message = rawMessage as MessageBase;
                    if (null != message)
                    {
                        if (message.Type == MessageType.Status)
                        {
                            _statusMessageQueue.Enqueue(message);
                        }
                        else
                        {
                            _engineMessageQueue.Enqueue(message);
                        }
                        GlobalInfo.LogService.Print(LogLevel.Debug, CommonConst.PlatformLogSession,
                            $"Message received, Type:{message.Type}, Index:{message.Index}.");
                    }
                    else
                    {
                        GlobalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformSession,
                            $"Unexpected message received. Type:{rawMessage.GetType().Name}, id:{rawMessage.Id}");
                    }
                }
            }
            catch (ThreadAbortException)
            {
                GlobalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                    $"thread {Thread.CurrentThread.Name} is stopped abnormally");

            }
            catch (Exception ex)
            {
                GlobalInfo.EventQueue.Enqueue(new ExceptionEventInfo(ex));
                GlobalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex);
                this.Stop();
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
            try
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    MessageBase message = queue.WaitUntilMessageCome();
                    if (null == message)
                    {
                        continue;
                    }
                    bool operationContinue = GetConsumer(message).HandleMessage(message);
                    // 如果消息执行后确认需要停止，则结束消息队列的处理。
                    if (!operationContinue)
                    {
                        break;
                    }
                }
                GlobalInfo.LogService.Print(LogLevel.Info, CommonConst.PlatformSession,
                    $"Listen thread: {Thread.CurrentThread.Name} is stopped.");
            }
            catch (ThreadAbortException)
            {
                GlobalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession,
                    $"thread {Thread.CurrentThread.Name} is stopped abnormally");
            }
            catch (Exception ex)
            {
                GlobalInfo.EventQueue.Enqueue(new ExceptionEventInfo(ex));
                GlobalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex);
                this.Stop();
            }
        }
    }
}