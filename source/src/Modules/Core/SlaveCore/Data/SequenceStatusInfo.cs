using System;
using System.Collections.Generic;
using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;

namespace Testflow.SlaveCore.Data
{
    internal class SequenceStatusInfo
    {
        public int Sequence { get; }
        public CallStack Stack { get; }
        public StepResult Result { get; }
        public StatusReportType ReportType { get; }
        public SequenceFailedInfo FailedInfo { get; }
        public DateTime Time { get; }
        public Dictionary<string, string> WatchDatas { get; set; }

        public SequenceStatusInfo(int sequence, CallStack stack, StatusReportType type, StepResult result, 
            SequenceFailedInfo failedInfo = null)
        {
            this.Sequence = sequence;
            this.Stack = stack;
            this.ReportType = type;
            this.FailedInfo = failedInfo;
            this.Time = DateTime.Now;
            this.Result = result;
        }
    }
}