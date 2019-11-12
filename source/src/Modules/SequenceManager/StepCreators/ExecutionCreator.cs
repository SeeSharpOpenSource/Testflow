using Testflow.Data.Sequence;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.StepCreators
{
    internal class ExecutionCreator : SequenceStepCreator
    {
        protected override ISequenceStep CreateSequenceStep()
        {
            return new SequenceStep();
        }
    }
}