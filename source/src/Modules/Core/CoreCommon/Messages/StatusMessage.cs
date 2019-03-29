using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 状态报告消息
    /// </summary>
    public class StatusMessage : MessageBase
    {
        public CallStack Stack { get; set; }

        public RuntimeState State { get; set; }

        public PerformanceData Performance { get; set; }

        public Dictionary<string, string> WatchData { get; }

        public StatusMessage(string name, CallStack stack, RuntimeState state, int id) : base(name, id, MessageType.Status)
        {
            this.Stack = stack;
            this.State = state;
            this.WatchData = new Dictionary<string, string>(CoreConstants.DefaultRuntimeSize);
        }

        public StatusMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Stack = info.GetValue("Stack", typeof(CallStack)) as CallStack;
            this.State = (RuntimeState) info.GetValue("State", typeof(RuntimeState));
            this.Performance = info.GetValue("Performance", typeof(PerformanceData)) as PerformanceData;
            this.WatchData =
                info.GetValue("WatchData", typeof (Dictionary<string, string>)) as Dictionary<string, string>;
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
            info.AddValue("WatchData", WatchData);
        }
    }
}