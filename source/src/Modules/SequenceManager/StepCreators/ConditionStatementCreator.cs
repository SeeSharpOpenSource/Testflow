
using Testflow.Data.Sequence;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.StepCreators
{
    internal class ConditionStatementCreator : SequenceStepCreator
    {
        protected override ISequenceStep CreateSequenceStep()
        {
            SequenceStep step = new SequenceStep()
            {
                StepType = SequenceStepType.ConditionStatement,
                SubSteps = new SequenceStepCollection()
            };
            return step;
        }
    }
}