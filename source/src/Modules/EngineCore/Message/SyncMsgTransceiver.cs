using System.Threading;
using Testflow.EngineCore.Common;
using Testflow.EngineCore.Message.Messages;
using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Message
{
    internal class SyncMsgTransceiver : MessageTransceiver
    {
        private Thread _receiveThread;
        private CancellationToken _cancellation;

        public SyncMsgTransceiver(ModuleGlobalInfo globalInfo) : base(globalInfo)
        {
        }

        protected override void StartReceive()
        {
            _receiveThread = new Thread(SynchronousReceive)
            {
                IsBackground = true
            };
            _cancellation = new CancellationToken(false);
            _receiveThread.Start();
        }

        protected override void StopReceive()
        {
            
        }

        protected override void SendMessage(MessageBase message)
        {
            Messenger.Send(message, FormatterType);
        }

        private void SynchronousReceive(object state)
        {
            while (!_cancellation.IsCancellationRequested)
            {
                IMessage message = Messenger.Receive();
                IMessageConsumer consumer = GetConsumer(message);
                consumer.HandleMessage(message);
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();

        }
    }
}