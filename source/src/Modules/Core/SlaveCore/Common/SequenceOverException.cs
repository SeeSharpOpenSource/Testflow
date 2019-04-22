using System;

namespace Testflow.SlaveCore.Common
{
    internal class SequenceOverException : ApplicationException
    {
        public int SequenceIndex { get; }

        public SequenceOverException(int sequenceIndex) : base($"Sequence {0} is over.")
        {
            this.SequenceIndex = sequenceIndex;
        }
    }
}