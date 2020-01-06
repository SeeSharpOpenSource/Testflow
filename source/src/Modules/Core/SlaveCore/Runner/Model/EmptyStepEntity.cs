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
            // 调用前置监听
            OnPreListener();

            this.Result = StepResult.NotAvailable;
            this.Result = Actuator.InvokeStep(forceInvoke);
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId,
                $"The empty step {GetStack()} invoked.");
            // 如果当前step被标记为记录状态，则返回状态信息
            if (null != StepData && StepData.RecordStatus)
            {
                RecordRuntimeStatus();
            }
            // 调用后置监听
            OnPostListener();

            if (null != StepData && StepData.HasSubSteps)
            {
                StepTaskEntityBase subStepEntity = SubStepRoot;
                do
                {
                    if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
                    {
                        return;
                    }
                    subStepEntity.Invoke(forceInvoke);
                } while (null != (subStepEntity = subStepEntity.NextStep));
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