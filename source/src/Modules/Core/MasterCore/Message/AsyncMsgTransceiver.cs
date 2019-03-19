using Testflow.CoreCommon.Messages;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.Message
{
    internal class AsyncMsgTransceiver : MessageTransceiver
    {
        public AsyncMsgTransceiver(ModuleGlobalInfo globalInfo) : base(globalInfo)
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