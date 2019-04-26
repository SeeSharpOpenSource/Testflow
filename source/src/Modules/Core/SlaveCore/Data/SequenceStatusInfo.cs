using System;
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
        public Exception Exception { get; }
        public DateTime Time { get; }

        public SequenceStatusInfo(int sequence, CallStack stack, StatusReportType type, StepResult result, Exception exception = null)
        {
            this.Sequence = sequence;
            this.Stack = stack;
            this.ReportType = type;
            this.Exception = exception;
            this.Time = DateTime.Now;
            this.Result = result;
        }
    }
}