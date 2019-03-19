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
        }

        /// <summary>
        /// 当前的堆栈信息
        /// </summary>
        public CallStack Stack { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public DebugMessageType DebugMsgType { get; set; }

        /// <summary>
        /// 被请求的变量值信息
        /// </summary>
        public DebugData Data { get; set; }

        public DebugMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Stack = info.GetValue("Stack", typeof(CallStack)) as CallStack;
            this.Data = info.GetValue("Data", typeof(DebugData)) as DebugData;
            this.DebugMsgType = (DebugMessageType) info.GetValue("DebugMsgType", typeof (DebugMessageType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (null != Stack)
            {
                info.AddValue("Stack", this.Stack, typeof(CallStack));
            }
            if (null != Data && 0 == Data.Count)
            {
                info.AddValue("Data", Data, typeof(DebugData));
            }
            info.AddValue("DebugMsgType", DebugMsgType);
        }
    }
}