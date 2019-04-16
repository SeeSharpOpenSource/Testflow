using Testflow.Data.Sequence;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class StepAssertModel : StepModelBase
    {
        public string VariableName { get; }

        public string RuntimeVariableName { get; }

        public string Expected { get; }

        public string RealValue { get; }

        public StepAssertModel(ISequenceStep step, SlaveContext context) : base(step, context)
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