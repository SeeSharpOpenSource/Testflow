using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class GotoStepEntity : StepTaskEntityBase
    {
        public GotoStepEntity(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
        }

        protected override void InvokeStepSingleTime(bool forceInvoke)
        {
            throw new System.NotImplementedException();
        }
    }
}