using System;
using System.Runtime.Serialization;
using Testflow.EngineCore.Data;
using Testflow.Runtime;

namespace Testflow.EngineCore.Messages
{
    public class StatusMessage : MessageBase
    {
        public CallStack Stack { get; set; }

        public RuntimeState State { get; set; }

        public PerformanceData Performance { get; set; }

        public StatusMessage(string name, CallStack stack, RuntimeState state, int id) : base(name, id, MessageType.Status)
        {
            this.Stack = stack;
            this.State = state;
        }

        public StatusMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Stack = info.GetValue("Stack", typeof(CallStack)) as CallStack;
            this.State = (RuntimeState) info.GetValue("State", typeof(RuntimeState));
            this.Performance = info.GetValue("Performance", typeof(PerformanceData)) as PerformanceData;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Stack", Stack, typeof(CallStack));
            info.AddValue("State", State, typeof(RuntimeState));
            if (null != Performance)
            {
                info.AddValue("Performance", Performance, typeof(PerformanceData));
            }
        }
    }
}