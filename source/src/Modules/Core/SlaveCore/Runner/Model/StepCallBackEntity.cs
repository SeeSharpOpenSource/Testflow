using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class StepCallBackEntity : StepTaskEntityBase
    {
        public StepCallBackEntity(ISequenceStep step, SlaveContext context) : base(step, context)
        {
        }

        public override void GenerateInvokeInfo()
        {
            throw new System.NotImplementedException();
        }

        public override void InitializeParamsValues()
        {
            throw new System.NotImplementedException();
        }

        public override void FillStatusInfo(StatusMessage statusMessage)
        {
            throw new System.NotImplementedException();
        }

        public override void Invoke()
        {
            throw new System.NotImplementedException();
        }
    }
}