using System;
using Testflow.Runtime;

namespace Testflow.SlaveCore.Common
{
    internal class TaskFailedException : ApplicationException
    {
        public FailedType FailedType { get; }

        public int SequenceIndex { get; }

        public TaskFailedException(int sequenceIndex, FailedType failedType, int errorCode) : base($"Sequence {0} failed.")
        {
            this.SequenceIndex = sequenceIndex;
            this.FailedType = failedType;
            this.HResult = errorCode;
        }

        public TaskFailedException(int sequenceIndex, string message, FailedType failedType, int errorCode) : base(message)
        {
            this.SequenceIndex = sequenceIndex;
            this.FailedType = failedType;
            this.HResult = errorCode;
        }
    }
}