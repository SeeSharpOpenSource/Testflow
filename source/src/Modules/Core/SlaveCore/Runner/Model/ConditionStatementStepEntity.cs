using Testflow.CoreCommon;
using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class ConditionStatementStepEntity : StepTaskEntityBase
    {
        public ConditionStatementStepEntity(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
        }

        protected override void InvokeStepSingleTime(bool forceInvoke)
        {
            // 重置计时时间
            Actuator.ResetTiming();
            // 调用前置监听
            OnPreListener();
            this.Result = StepResult.NotAvailable;
            this.Result = Actuator.InvokeStep(forceInvoke);

            // 如果当前step被标记为记录状态，则返回状态信息
            if (null != StepData && StepData.RecordStatus)
            {
                RecordRuntimeStatus();
            }
            // 调用后置监听
            OnPostListener();
            object returnValue = Actuator.Return;
            // 如果执行结果不是bool类型或者为False，或者没有下级节点，则退出当前循环
            bool conditionFail = !(returnValue is bool) || !(bool) returnValue;
            if (conditionFail || null == StepData || !StepData.HasSubSteps)
            {
                return;
            }
            // 执行下级的所有Step
            StepTaskEntityBase subStepEntity = SubStepRoot;
            do
            {
                if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
                {
                    FailedInfo failedInfo = new FailedInfo(Context.I18N.GetStr("OperationAborted"), FailedType.Abort)
                    {
                        ErrorCode = ModuleErrorCode.UserForceFailed,
                        Source = ModuleUtils.GetTypeFullName(this.GetType())
                    };
                    SetStatusAndSendErrorEvent(StepResult.Abort, failedInfo);
                    return;
                }
                subStepEntity.Invoke(forceInvoke);
            } while (null != (subStepEntity = subStepEntity.NextStep));
        }
    }
}