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
        public DebugMessage(string name, int id, int objectId, CallStack stack) : base(name, id, MessageType.Debug)
        {
            this.Stack = stack;
            this.ObjectId = objectId;
        }

        /// <summary>
        /// 运行时对象id
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// 当前的堆栈信息
        /// </summary>
        public CallStack Stack { get; set; }

        /// <summary>
        /// 被请求的变量值信息
        /// </summary>
        private DebugData Data { get; set; }

        public DebugMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Stack = info.GetValue("Stack", typeof(CallStack)) as CallStack;
            this.Data = info.GetValue("Data", typeof(DebugData)) as DebugData;
            this.ObjectId = (int) info.GetValue("ObjectId", typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Stack", this.Stack, typeof(CallStack));
            info.AddValue("ObjectId", ObjectId);
            if (null != Data && 0 == Data.Count)
            {
                info.AddValue("Data", Data, typeof(DebugData));
            }
        }
    }
}