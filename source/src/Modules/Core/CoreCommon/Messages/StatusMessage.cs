﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.CoreCommon.Messages
{
    /// <summary>
    /// 状态报告消息
    /// </summary>
    [Serializable]
    public class StatusMessage : MessageBase
    {
        public List<CallStack> Stacks { get; set; }

        public RuntimeState State { get; set; }

        public List<RuntimeState> SequenceStates { get; set; }

        public List<StepResult> Results { get; set; }

        public PerformanceData Performance { get; set; }

        public Dictionary<int, string> FailedInfo { get; set; }

        /// <summary>
        /// 触发关键位置的序列索引号
        /// </summary>
        public List<int> InterestedSequence { get; set; }

        // 如果使用DateTime直接序列化会损失tick的精度，所以使用string传递，使用的时候再转换
        public List<DateTime> ExecutionTimes { get; set; }

        public List<long> ExecutionTicks { get; set; }

        public List<int> Coroutines { get; set; }

        public Dictionary<string, string> WatchData { get; set; }

        public FailedInfo ExceptionInfo { get; set; }

        public StatusMessage(string name, RuntimeState state, int id) : base(name, id, MessageType.Status)
        {
            this.State = state;
            this.InterestedSequence = new List<int>(CoreConstants.DefaultRuntimeSize);
            this.Stacks = new List<CallStack>(CoreConstants.DefaultRuntimeSize);
            this.SequenceStates = new List<RuntimeState>(CoreConstants.DefaultRuntimeSize);
            this.Results = new List<StepResult>(CoreConstants.DefaultRuntimeSize);
            this.FailedInfo = new Dictionary<int, string>(CoreConstants.DefaultRuntimeSize);
            this.ExecutionTicks = new List<long>(CoreConstants.DefaultRuntimeSize);
            this.ExecutionTimes = new List<DateTime>(CoreConstants.DefaultRuntimeSize);
            this.Coroutines = new List<int>(CoreConstants.DefaultRuntimeSize);
        }

        public StatusMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            CoreUtils.SetMessageValue(info, this, this.GetType());
            if (null == FailedInfo)
            {
                FailedInfo = new Dictionary<int, string>(1);
            }
            if (null == InterestedSequence)
            {
                InterestedSequence = new List<int>(1);
            }
            if (null == WatchData)
            {
                this.WatchData = new Dictionary<string, string>(1);
            }
            if (null == Stacks)
            {
                this.Stacks = new List<CallStack>(1);
            }
            if (null == SequenceStates)
            {
                this.SequenceStates = new List<RuntimeState>(1);
            }
            if (null == Results)
            {
                this.Results = new List<StepResult>(1);
            }
            if (null == ExecutionTicks)
            {
                this.ExecutionTicks = new List<long>(1);
            }
            if (null == ExecutionTimes)
            {
                this.ExecutionTimes = new List<DateTime>(1);
            }

            if (null == Coroutines)
            {
                this.Coroutines = new List<int>(1);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Stacks", Stacks, typeof(List<CallStack>));
            info.AddValue("SequenceStates", SequenceStates, typeof(List<RuntimeState>));
            info.AddValue("State", State, typeof(RuntimeState));
            if (null != Performance)
            {
                info.AddValue("Performance", Performance, typeof(PerformanceData));
            }
            if (null != WatchData && WatchData.Count > 0)
            {
                info.AddValue("WatchData", WatchData, typeof(Dictionary<string, string>));
            }
            if (null != ExceptionInfo)
            {
                info.AddValue("ExceptionInfo", ExceptionInfo, typeof(FailedInfo));
            }
            info.AddValue("Results", Results, typeof(List<StepResult>));
            if (null != FailedInfo && FailedInfo.Count > 0)
            {
                info.AddValue("FailedInfo", FailedInfo, typeof(Dictionary<int, string>));
            }
            if (null != InterestedSequence && InterestedSequence.Count > 0)
            {
                info.AddValue("InterestedSequence", InterestedSequence, typeof(List<int>));
            }
            if (null != ExecutionTimes && ExecutionTimes.Count > 0)
            {
                info.AddValue("ExecutionTimes", ExecutionTimes, typeof(List<string>));
                info.AddValue("ExecutionTicks", ExecutionTicks, typeof(List<long>));
                info.AddValue("Coroutines", Coroutines, typeof(List<int>));
            }
        }
    }
}