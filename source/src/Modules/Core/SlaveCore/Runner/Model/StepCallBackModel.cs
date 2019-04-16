using Testflow.Data.Sequence;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class StepCallBackModel : StepModelBase
    {
        public StepCallBackModel(ISequenceStep step, SlaveContext context) : base(step, context)
        {
        }

        public override void FillStatusInfo()
        {
            throw new System.NotImplementedException();
        }

        public override void Invoke()
        {
            throw new System.NotImplementedException();
        }
    }
}