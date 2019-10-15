using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Actuators
{
    internal class AssertActuator : ActuatorBase
    {
        public string VariableName { get; }

        public string RuntimeVariableName { get; }

        public string Expected { get; }

        public string RealValue { get; }

        public AssertActuator(ISequenceStep stepData, SlaveContext context, int sequenceIndex) : base(stepData, context, sequenceIndex)
        {

        }

        protected override void GenerateInvokeInfo()
        {
            throw new System.NotImplementedException();
        }

        protected override void InitializeParamsValues()
        {
            throw new System.NotImplementedException();
        }

        public override StepResult InvokeStep(bool forceInvoke)
        {
            throw new System.NotImplementedException();
        }
    }
}