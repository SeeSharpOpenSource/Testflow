using System.Runtime.Serialization;
using Testflow.EngineCore.Data;

namespace Testflow.EngineCore.Messages
{
    public class DebugMessage : MessageBase
    {
        public DebugMessage(string name, int id, CallStack stack) : base(name, id, MessageType.Debug)
        {
            this.Stack = stack;
        }

        /// <summary>
        /// 当前的堆栈信息
        /// </summary>
        private CallStack Stack { get; set; }

        /// <summary>
        /// 被请求的变量值信息
        /// </summary>
        private DebugData Data { get; set; }

        public DebugMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Stack = info.GetValue("Stack", typeof(CallStack)) as CallStack;
            this.Data = info.GetValue("Data", typeof(DebugData)) as DebugData;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Stack", this.Stack, typeof(CallStack));
            if (null != Data && 0 == Data.Count)
            {
                info.AddValue("Data", Data, typeof(DebugData));
            }
        }
    }
}