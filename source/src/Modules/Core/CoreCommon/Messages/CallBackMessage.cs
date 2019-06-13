using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;

namespace Testflow.CoreCommon.Messages
{
    [Serializable]
    public class CallBackMessage : MessageBase
    {
        public List<string> Args { get; set; }

        /// <summary>
        /// 创建回调返回消息实例
        /// </summary>
        /// <param name="name">回调功能的名称</param>
        /// <param name="id">消息的ID</param>
        /// <param name="callBackArgs">回调参数</param>
        public CallBackMessage(string name, int id, params string[] callBackArgs) : base(name, id, MessageType.Ctrl)
        {
            Args = new List<string>(callBackArgs);
        }

        public CallBackMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Args = info.GetValue("Args", typeof (List<string>)) as List<string>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Args", Args);
        }
    }
}