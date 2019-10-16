using Testflow.Data.Sequence;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.StepCreators
{
    internal class ExecutionCreator : SequenceStepCreator
    {
        protected override ISequenceStep CreateSequenceStep()
        {
            SequenceStep sequenceStep = new SequenceStep()
            {
                Behavior = RunBehavior.Normal,
                BreakIfFailed = true,
                RecordStatus = false,
                StepType = SequenceStepType.Execution
            };
            return sequenceStep;
        }
    }
}