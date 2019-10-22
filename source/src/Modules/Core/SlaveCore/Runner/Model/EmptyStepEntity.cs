using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class EmptyStepEntity : StepTaskEntityBase
    {
        public EmptyStepEntity(SlaveContext context, int sequenceIndex, ISequenceStep stepData) : base(stepData, context, sequenceIndex)
        {
//            switch (sequenceIndex)
//            {
//                case CommonConst.SetupIndex:
//                    _sequenceName = Constants.EmptySetupName;
//                    break;
//                case CommonConst.TeardownIndex:
//                    _sequenceName = Constants.EmptyTeardownName;
//                    break;
//                default:
//                    _sequenceName = sequenceIndex.ToString();
//                    break;
//            }
//            _sequenceIndex = sequenceIndex;
        }

        protected override void InvokeStepSingleTime(bool forceInvoke)
        {
            // 重置计时时间
            Actuator.ResetTiming();
            // 如果是取消状态并且不是强制执行则返回
            if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
            {
                this.Result = StepResult.Abort;
                return;
            }
            Actuator.InvokeStep(forceInvoke);
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId,
                $"The empty step {GetStack()} invoked.");
            if (null != StepData && StepData.HasSubSteps)
            {
                StepTaskEntityBase subStepEntity = SubStepRoot;
                bool notCancelled = true;
                do
                {
                    if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
                    {
                        return;
                    }
                    subStepEntity.Invoke(forceInvoke);
                    notCancelled = forceInvoke || !Context.Cancellation.IsCancellationRequested;
                } while (null != (subStepEntity = subStepEntity.NextStep) && notCancelled);
            }
        }

        public override CallStack GetStack()
        {
            return null == StepData ? 
                CallStack.GetEmptyStack(Context.SessionId, SequenceIndex)
                : CallStack.GetStack(Context.SessionId, StepData);
        }
    }
}