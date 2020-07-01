﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Runtime;
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
            this.IsLastRmtGenMessage = false;
            this.Params = new Dictionary<string, string>(CoreConstants.DefaultRuntimeSize);
        }

        public RmtGenMessage(string name, int id): base(name, id, MessageType.RmtGen)
        {
            this.IsLastRmtGenMessage = false;
            this.Params = new Dictionary<string, string>(CoreConstants.DefaultRuntimeSize);
        }

        public RmtGenMessage(string name, int id, RunnerType type) : base(name, id, MessageType.RmtGen)
        {
            this.SequenceType = type;
            this.Sequence = null;
            this.IsLastRmtGenMessage = false;
            this.Params = new Dictionary<string, string>(CoreConstants.DefaultRuntimeSize);
        }

        /// <summary>
        /// 标记当前消息是否为最后一条消息
        /// </summary>
        public bool IsLastRmtGenMessage { get; set; }

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
        /// 测试生成错误堆栈
        /// </summary>
        public CallStack ErrorStack { get; set; }

        /// <summary>
        /// 测试生成错误信息
        /// </summary>
        public string ErrorInfo { get; set; }

        /// <summary>
        /// 额外参数配置
        /// </summary>
        public Dictionary<string, string> Params { get; set; }

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
            info.AddValue("IsLastRmtGenMessage", IsLastRmtGenMessage);
            if (null != this.RunnerHost)
            {
                info.AddValue("RunnerHost", RunnerHost);
            }
            if (null != MasterHost)
            {
                info.AddValue("MasterHost", MasterHost);
            }
            if (!string.IsNullOrWhiteSpace(ErrorInfo))
            {
                info.AddValue("ErrorInfo", ErrorInfo);
            }
            if (null != ErrorStack)
            {
                info.AddValue("ErrorStack", ErrorStack);
            }
        }
    }
}