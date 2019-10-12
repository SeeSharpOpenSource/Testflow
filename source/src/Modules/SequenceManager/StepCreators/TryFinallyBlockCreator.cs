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
            SequenceStep tryBlock = new SequenceStep();
            tryBlock.Name = "Try";
            tryBlock.Parent = step;
            tryBlock.SubSteps = new SequenceStepCollection();
            step.SubSteps.Add(tryBlock);

            SequenceStep finallyBlock = new SequenceStep();
            finallyBlock.Name = "Finally";
            finallyBlock.Parent = step;
            finallyBlock.SubSteps = new SequenceStepCollection();
            step.SubSteps.Add(finallyBlock);
            step.SubSteps.Add(tryBlock);
            return step;
        }
    }
}