using Testflow.Data.Sequence;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.StepCreators
{
    internal class TryFinallyBlockCreator : SequenceStepCreator
    {
        protected override ISequenceStep CreateSequenceStep()
        {
            SequenceStep step = new SequenceStep
            {
                StepType = SequenceStepType.TryFinallyBlock,
                SubSteps = new SequenceStepCollection()
            };
            SequenceStep tryBlock = new SequenceStep
            {
                Name = "Try",
                Parent = step,
                SubSteps = new SequenceStepCollection()
            };
            step.SubSteps.Add(tryBlock);

            SequenceStep finallyBlock = new SequenceStep
            {
                Name = "Finally",
                Parent = step,
                SubSteps = new SequenceStepCollection()
            };
            step.SubSteps.Add(finallyBlock);
            return step;
        }
    }
}