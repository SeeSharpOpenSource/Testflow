using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 调试数据消息
    /// </summary>
    [Serializable]
    public class DebugMessage : MessageBase
    {
        public DebugMessage(string name, int id, bool isRequest) : base(name, id, MessageType.Debug)
        {
            this.IsRequst = isRequest;
            this.WatchData = null;
            this.BreakPoints = null;
        }

        public DebugMessage(string name, int id, CallStack breakPoint, bool isRequest) : base(name, id, MessageType.Debug)
        {
            this.IsRequst = isRequest;
            this.WatchData = null;
            this.BreakPoints = new List<CallStack>(1);
            this.BreakPoints.Add(breakPoint);
        }

        public DebugMessage(string name, int id, IList<CallStack> breakPoint, bool isRequest) : base(name, id, MessageType.Debug)
        {
            this.IsRequst = isRequest;
            this.WatchData = null;
            this.BreakPoints = new List<CallStack>(breakPoint);
        }

        /// <summary>
        /// 断点列表
        /// </summary>
        public List<CallStack> BreakPoints { get; set; }

        /// <summary>
        /// 该消息是否为请求命令(下行)
        /// </summary>
        public bool IsRequst { get; set; }

        /// <summary>
        /// 被请求的变量值信息
        /// </summary>
        public DebugWatchData WatchData { get; set; }

        public DebugMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
//            this.BreakPoint = info.GetValue("Stack", typeof(CallStack)) as CallStack;
//            this.WatchData = info.GetValue("Data", typeof(DebugWatchData)) as DebugWatchData;
//            this.DebugMsgType = (DebugMessageType) info.GetValue("DebugMsgType", typeof (DebugMessageType));
            CoreUtils.SetMessageValue(info, this, this.GetType());
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("IsRequst", IsRequst);
            if (null != WatchData && WatchData.Count > 0)
            {
                info.AddValue("WatchData", WatchData, typeof(DebugWatchData));
            }
            if (null != BreakPoints && BreakPoints.Count > 0)
            {
                info.AddValue("BreakPoints", BreakPoints, typeof(List<CallStack>));
            }
        }
    }
}