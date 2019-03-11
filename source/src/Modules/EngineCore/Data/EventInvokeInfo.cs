using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Data
{
    public class EventInvokeInfo : IMessage
    {
        public int Id { get; }

        public string EventName { get; set; }

        public object[] Param { get; set; }
    }
}