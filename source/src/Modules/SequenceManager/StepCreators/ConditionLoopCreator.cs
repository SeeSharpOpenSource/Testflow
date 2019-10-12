using Testflow.Data.Sequence;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.StepCreators
{
    internal class ConditionLoopCreator : SequenceStepCreator
    {
        protected override ISequenceStep CreateSequenceStep()
        {
            SequenceStep step = new SequenceStep
            {
                StepType = SequenceStepType.ConditionBlock,
                SubSteps = new SequenceStepCollection(),
                Name = "ConditionBlock",
                LoopCounter = new LoopCounter()
                {
                    CounterEnabled = true,
                    CounterVariable = string.Empty,
                    MaxValue = 0,
                    Name = "ConditionLoop"
                }
            };
            return step;
        }
    }
}