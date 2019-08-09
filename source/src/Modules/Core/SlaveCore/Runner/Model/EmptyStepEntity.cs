using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class EmptyStepEntity : StepTaskEntityBase
    {
        private string _sequenceName;
        private int _sequenceIndex;
        public EmptyStepEntity(SlaveContext context, int sequenceIndex, ISequenceStep stepData) : base(stepData, context, sequenceIndex)
        {
            switch (sequenceIndex)
            {
                case CommonConst.SetupIndex:
                    _sequenceName = Constants.EmptySetupName;
                    break;
                case CommonConst.TeardownIndex:
                    _sequenceName = Constants.EmptyTeardownName;
                    break;
                default:
                    _sequenceName = sequenceIndex.ToString();
                    break;
            }
            _sequenceIndex = sequenceIndex;
            this.NextStep = null;
        }

        protected override void GenerateInvokeInfo()
        {
            // ignore
        }

        protected override void InitializeParamsValues()
        {
            // ignore
        }

        protected override void InvokeStep(bool forceInvoke)
        {
            this.Result = StepResult.Pass;
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId,
                $"The empty step {GetStack()} invoked.");
        }

        public override CallStack GetStack()
        {
            return null == StepData ? 
                CallStack.GetEmptyStack(Context.SessionId, SequenceIndex)
                : CallStack.GetStack(Context.SessionId, StepData);
        }
    }
}