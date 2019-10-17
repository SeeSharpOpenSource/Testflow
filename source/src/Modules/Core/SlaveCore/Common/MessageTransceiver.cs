using System;
using System.Runtime.InteropServices;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Utility.MessageUtil;

namespace Testflow.SlaveCore.Common
{
    internal class MessageTransceiver : IDisposable
    {
        private readonly Messenger _uplinkMessenger;
        private readonly Messenger _downLinkMessenger;
        private readonly SlaveContext _slaveContext;
        private readonly LocalMessageQueue<MessageBase> _messageQueue;
        private Thread _peakThread;
        private CancellationTokenSource _cancellation;

        public MessageTransceiver(SlaveContext contextManager, int session)
        {
            this._slaveContext = contextManager;

            // 创建上行队列
            FormatterType formatterType = contextManager.GetProperty<FormatterType>("EngineQueueFormat");
            MessengerOption receiveOption = new MessengerOption(CoreConstants.UpLinkMQName, GetMessageType)
            {
                Type = contextManager.GetProperty<MessengerType>("MessengerType"),
                Formatter = formatterType,
                ReceiveType = ReceiveType.Synchronous
            };
            _uplinkMessenger = Messenger.GetMessenger(receiveOption);
            // 创建下行队列
            MessengerOption sendOption = new MessengerOption(CoreConstants.DownLinkMQName, GetMessageType)
            {
                Type = contextManager.GetProperty<MessengerType>("MessengerType"),
                Formatter = formatterType,
                ReceiveType = ReceiveType.Synchronous
            };
            _downLinkMessenger = Messenger.GetMessenger(sendOption);

            _messageQueue = new LocalMessageQueue<MessageBase>(CoreConstants.DefaultEventsQueueSize);
            this.SessionId = session;
        }

        public LocalMessageQueue<MessageBase> MessageQueue => _messageQueue;

        public void SendMessage(MessageBase message)
        {
            _uplinkMessenger.Send(message);
            // 打印状态日志
            _slaveContext.LogSession.Print(LogLevel.Debug, _slaveContext.SessionId,
                $"Send message, Type:{message.Type}, Index:{message.Index}.");
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
                    // 如果是自己的消息，则在队列中移除并添加到队列中
                    if (message.Id == SessionId)
                    {
                        IMessage receive = _downLinkMessenger.Receive();
                        MessageBase downlinkMessage = (MessageBase)message;
                        _messageQueue.Enqueue(downlinkMessage);
                        // 打印状态日志
                        _slaveContext.LogSession.Print(LogLevel.Debug, _slaveContext.SessionId,
                            $"Message received, Type:{downlinkMessage.Type}, Index:{downlinkMessage.Index}.");
                    }
                    else
                    {
                        Thread.Yield();
                    }
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
            Thread.MemoryBarrier();
            _messageQueue.FreeBlocks();
            Thread.MemoryBarrier();
            // 发送消息，让DownlinkMessageQueue退出阻塞状态。该消息不会被真正处理
            ControlMessage stopMessage = new ControlMessage(MessageNames.CtrlAbort, _slaveContext.SessionId);
            _downLinkMessenger.Send(stopMessage);
            Thread.Sleep(100);
            if (_peakThread.IsAlive)
            {
                _peakThread.Abort();
            }
            // 打印状态日志
            _slaveContext.LogSession.Print(LogLevel.Info, _slaveContext.SessionId,
                            "Message receive stopped.");
        }

        public MessageBase Receive()
        {
            return _messageQueue.WaitUntilMessageCome();
        }

        public void Dispose()
        {
            StopReceive();
        }

        // 为了提高效率，暂时写死，后续优化
        public Type GetMessageType(string typeName)
        {
            switch (typeName)
            {
                case "CallBackMessage":
                    return typeof(CallBackMessage);
                    break;
                case "ControlMessage":
                    return typeof(ControlMessage);
                    break;
                case "DebugMessage":
                    return typeof(DebugMessage);
                    break;
                case "ResourceSyncMessage":
                    return typeof(ResourceSyncMessage);
                    break;
                case "RmtGenMessage":
                    return typeof(RmtGenMessage);
                    break;
                case "RuntimeErrorMessage":
                    return typeof(RuntimeErrorMessage);
                    break;
                case "StatusMessage":
                    return typeof(StatusMessage);
                    break;
                case "TestGenMessage":
                    return typeof(TestGenMessage);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        #region 调用WinApi的跨进程同步函数

        [DllImport("kernel32", EntryPoint = "CreateSemaphore", SetLastError = true, CharSet = CharSet.Unicode)]
         private static extern uint CreateSemaphore(SecurityAttributes auth, int initialCount, int maximumCount,
            string name);

        [DllImport("kernel32", EntryPoint = "WaitForSingleObject",
            SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint WaitForSingleObject(uint hHandle, uint dwMilliseconds);

        [DllImport("kernel32", EntryPoint = "ReleaseSemaphore", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.VariantBool)]
        private static extern bool ReleaseSemaphore(uint hHandle, int lReleaseCount, out int lpPreviousCount);

        [DllImport("kernel32", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.VariantBool)]
        private static extern bool CloseHandle(uint hHandle);

        [StructLayout(LayoutKind.Sequential)]
        private struct SecurityAttributes
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        #endregion

    }
}