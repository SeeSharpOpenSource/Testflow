using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 调试数据消息
    /// </summary>
    public class DebugMessage : MessageBase
    {
        public DebugMessage(string name, int id, DebugMessageType msgType) : base(name, id, MessageType.Debug)
        {
            this.DebugMsgType = msgType;
            this.BreakPoint = null;
            this.WatchData = null;
        }

        /// <summary>
        /// 当前的堆栈信息
        /// </summary>
        public CallStack BreakPoint { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public DebugMessageType DebugMsgType { get; set; }

        /// <summary>
        /// 被请求的变量值信息
        /// </summary>
        public DebugWatchData WatchData { get; set; }

        public DebugMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.BreakPoint = info.GetValue("Stack", typeof(CallStack)) as CallStack;
            this.WatchData = info.GetValue("Data", typeof(DebugWatchData)) as DebugWatchData;
            this.DebugMsgType = (DebugMessageType) info.GetValue("DebugMsgType", typeof (DebugMessageType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (null != BreakPoint)
            {
                info.AddValue("Stack", this.BreakPoint, typeof(CallStack));
            }
            if (null != WatchData && 0 == WatchData.Count)
            {
                info.AddValue("Data", WatchData, typeof(DebugWatchData));
            }
            info.AddValue("DebugMsgType", DebugMsgType);
        }
    }
}