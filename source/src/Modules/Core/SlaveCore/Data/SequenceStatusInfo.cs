using System;
using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;

namespace Testflow.SlaveCore.Data
{
    internal class SequenceStatusInfo
    {
        public int Sequence { get; }
        public CallStack Stack { get; }
        public StatusReportType ReportType { get; }
        public Exception Exception { get; }

        public SequenceStatusInfo(int sequence, CallStack stack, StatusReportType type, Exception exception = null)
        {
            this.Sequence = sequence;
            this.Stack = stack;
            this.ReportType = type;
            this.Exception = exception;
        }
    }
}