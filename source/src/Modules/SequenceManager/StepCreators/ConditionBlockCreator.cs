using System.Linq;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManager.StepCreators
{
    internal class ConditionBlockCreator : SequenceStepCreator
    {
        protected override ISequenceStep CreateSequenceStep()
        {
            SequenceStep step = new SequenceStep()
            {
                StepType = SequenceStepType.ConditionBlock,
                SubSteps = new SequenceStepCollection(),
                Name = "ConditionBlock"
            };
            SequenceStep conditionStatement = new SequenceStep()
            {
                StepType = SequenceStepType.Execution,
                SubSteps = new SequenceStepCollection()
            };
            step.SubSteps.Add(conditionStatement);
            return step;
        }
    }
}