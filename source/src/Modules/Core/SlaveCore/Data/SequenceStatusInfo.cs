using System;
using System.Collections.Generic;
using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.SlaveCore.Data
{
    internal class SequenceStatusInfo
    {
        public int Sequence { get; }
        public CallStack Stack { get; }
        public RuntimeState SequenceState { get; }
        public StepResult Result { get; }
        public StatusReportType ReportType { get; }
        public FailedInfo FailedInfo { get; }
        public DateTime ExecutionTime { get; set; }
        public long ExecutionTicks { get; set; }
        public int CoroutineId { get; set; }
        public DateTime Time { get; }
        public Dictionary<string, string> WatchDatas { get; set; }

        public SequenceStatusInfo(int sequence, CallStack stack, StatusReportType type, RuntimeState sequenceState, StepResult result, FailedInfo failedInfo = null)
        {
            this.SequenceState = sequenceState;
            this.Sequence = sequence;
            this.Stack = stack;
            this.ReportType = type;
            this.FailedInfo = failedInfo;
            this.Time = DateTime.Now;
            this.Result = result;
        }
    }
}