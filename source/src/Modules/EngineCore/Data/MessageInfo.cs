using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.Data
{
    internal class MessageInfo : IMessage
    {
        public int Id { get; }

        public string EventName { get; set; }

        public object[] Param { get; set; }
    }
}