using System;

namespace Testflow.SlaveCore.Common
{
    internal class TaskFailedException : ApplicationException
    {
        public int SequenceIndex { get; }

        public TaskFailedException(int sequenceIndex) : base($"Sequence {0} is over.")
        {
            this.SequenceIndex = sequenceIndex;
        }

        public TaskFailedException(int sequenceIndex, string message) : base(message)
        {
            this.SequenceIndex = sequenceIndex;
        }
    }
}