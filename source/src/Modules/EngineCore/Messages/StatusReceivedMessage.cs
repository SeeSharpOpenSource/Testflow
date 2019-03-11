using Testflow.EngineCore.Data;

namespace Testflow.EngineCore.Messages
{
    public class StatusReceivedMessage : MessageBase
    {
        public RuntimeStatusInfo Info { get; set; }

        public CallStack CallStack { get; set; }
    }
}