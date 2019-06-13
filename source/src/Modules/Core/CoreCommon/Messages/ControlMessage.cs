using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 引擎控制消息
    /// </summary>
    [Serializable]
    public class ControlMessage : MessageBase
    {
        public ControlMessage(string name, int id) : base(name, id, MessageType.Ctrl)
        {
            Params = new Dictionary<string, string>(CoreConstants.DefaultRuntimeSize);
        }

        public ControlMessage(string name, int id, Dictionary<string, string> extraParams) : base(name, id, MessageType.Ctrl)
        {
            Params = (null == extraParams) ? 
                new Dictionary<string, string>(CoreConstants.DefaultRuntimeSize):
                new Dictionary<string, string>(extraParams);
        }

        /// <summary>
        /// 控制指令的额外参数
        /// </summary>
        public Dictionary<string, string> Params { get; set; }

        public ControlMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
//            Params = info.GetValue("Params", typeof (Dictionary<string, string>)) as Dictionary<string, string>;
            CoreUtils.SetMessageValue(info, this, this.GetType());
        }

        public void AddParam(string paramName, string paramValue)
        {
            this.Params.Add(paramName, paramValue);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (0 != Params.Count)
            {
                info.AddValue("Params", Params);
            }
        }
    }
}