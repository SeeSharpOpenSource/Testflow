using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Usr;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 远程运行器生成消息
    /// </summary>
    [Serializable]
    public class RmtGenMessage : MessageBase
    {
        public RmtGenMessage(string name, int id, RunnerType type, string sequence) : base(name, id, MessageType.RmtGen)
        {
            this.SequenceType = type;
            this.Sequence = sequence;
            this.Params = new Dictionary<string, string>(CoreConstants.DefaultRuntimeSize);
        }

        /// <summary>
        /// 测试运行主机的信息
        /// </summary>
        public HostInfo RunnerHost { get; set; }

        /// <summary>
        /// 测试引擎主机的信息
        /// </summary>
        public HostInfo MasterHost { get; set; }

        /// <summary>
        /// 序列的类型
        /// </summary>
        public RunnerType SequenceType { get; set; }

        /// <summary>
        /// 测试序列数据序列化后的数据
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// 额外参数配置
        /// </summary>
        public Dictionary<string, string> Params { get; }

        public RmtGenMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
//            this.SequenceType = (RunnerType) info.GetValue("SequenceType", typeof (RunnerType));
//            this.Sequence = (string) info.GetValue("Sequence", typeof (string));
//            this.Params = info.GetValue("Params", typeof (Dictionary<string, string>)) as Dictionary<string, string>;
//            this.MasterHost = info.GetValue("MasterHost", typeof (HostInfo)) as HostInfo;
//            this.RunnerHost = info.GetValue("RunnerHost", typeof (HostInfo)) as HostInfo;
            CoreUtils.SetMessageValue(info, this, this.GetType());
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("SequenceType", SequenceType);
            info.AddValue("Sequence", Sequence);
            info.AddValue("Params", Params);
            if (null != this.RunnerHost)
            {
                info.AddValue("RunnerHost", RunnerHost);
            }
            if (null != MasterHost)
            {
                info.AddValue("MasterHost", MasterHost);
            }
        }
    }
}