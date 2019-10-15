using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Actuators
{
    internal class EmptyActuator : ActuatorBase
    {
        public EmptyActuator(SlaveContext context, ISequenceStep step, int sequenceIndex) : base(step, context, sequenceIndex)
        {
        }

        protected override void GenerateInvokeInfo()
        {
            // ignore
        }

        protected override void InitializeParamsValues()
        {
            // ignore
        }

        public override StepResult InvokeStep(bool forceInvoke)
        {
            return StepResult.Pass;
        }
    }
}