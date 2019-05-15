using Testflow.CoreCommon.Messages;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Core
{
    internal class CallBackProcessor : IMessageHandler
    {
        public CallBackProcessor(ModuleGlobalInfo globalInfo)
        {
            
        }

        public bool HandleMessage(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }
    }
}