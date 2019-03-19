using System.Collections.Generic;
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
        /// <summary>
        /// name是断点的全局ID转换为字符串
        /// </summary>
        public DebugMessage(string name, int id, CallStack stack) : base(name, id, MessageType.Debug)
        {
            this.Stack = stack;
        }

        /// <summary>
        /// 当前的堆栈信息
        /// </summary>
        public CallStack Stack { get; set; }

        public List<string> Variable { get; set; }

        /// <summary>
        /// 被请求的变量值信息
        /// </summary>
        private DebugData Data { get; set; }

        public DebugMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Stack = info.GetValue("Stack", typeof(CallStack)) as CallStack;
            this.Variable = info.GetValue("Variable", typeof (List<string>)) as List<string>;
            this.Data = info.GetValue("Data", typeof(DebugData)) as DebugData;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Stack", this.Stack, typeof(CallStack));
            if (null != Variable)
            {
                info.AddValue("Variable", Variable);
            }
            if (null != Data && 0 == Data.Count)
            {
                info.AddValue("Data", Data, typeof(DebugData));
            }
        }
    }
}