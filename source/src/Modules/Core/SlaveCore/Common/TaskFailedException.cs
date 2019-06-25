using System;
using Testflow.Runtime;

namespace Testflow.SlaveCore.Common
{
    internal class TaskFailedException : ApplicationException
    {
        public FailedType FailedType { get; }

        public int SequenceIndex { get; }

        public TaskFailedException(int sequenceIndex, FailedType failedType) : base($"Sequence {0} failed.")
        {
            this.SequenceIndex = sequenceIndex;
            this.FailedType = failedType;
        }

        public TaskFailedException(int sequenceIndex, string message, FailedType failedType) : base(message)
        {
            this.SequenceIndex = sequenceIndex;
            this.FailedType = failedType;
        }
    }
}