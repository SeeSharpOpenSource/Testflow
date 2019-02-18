using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    public class LoopCounter : ILoopCounter
    {
        public LoopCounter()
        {
            this.Name = string.Empty;
            this.MaxValue = 0;
            this.CounterEnabled = true;
            this.CounterVariable = string.Empty;
        }

        public string Name { get; set; }
        public int MaxValue { get; set; }
        public bool CounterEnabled { get; set; }
        public string CounterVariable { get; set; }

        public ILoopCounter Clone()
        {
            LoopCounter loopCounter = new LoopCounter()
            {
                Name = this.Name + Constants.CopyPostfix,
                MaxValue = this.MaxValue,
                CounterEnabled = this.CounterEnabled,
                CounterVariable = string.Empty
            };
            return loopCounter;
        }
    }
}