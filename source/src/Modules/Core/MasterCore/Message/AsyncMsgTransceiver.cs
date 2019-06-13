using Testflow.CoreCommon.Messages;
using Testflow.MasterCore.Common;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Message
{
    internal class AsyncMsgTransceiver : MessageTransceiver
    {
        public AsyncMsgTransceiver(ModuleGlobalInfo globalInfo) : base(globalInfo, ReceiveType.Asynchronous)
        {
        }

        protected override void Start()
        {
            throw new System.NotImplementedException();
        }

        protected override void Stop()
        {
            throw new System.NotImplementedException();
        }

        protected override void SendMessage(MessageBase message)
        {
            throw new System.NotImplementedException();
        }
    }
}