using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class TryFinallyBlockStepEntity : StepTaskEntityBase
    {
        public TryFinallyBlockStepEntity(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
        }

        protected override void InvokeStepSingleTime(bool forceInvoke)
        {
            // 重置计时
            Actuator.ResetTiming();
            // 调用前置监听
            OnPreListener();

            // 开始计时
            Actuator.StartTiming();
            // 停止计时
            Actuator.EndTiming();
            // 应为TryFinally块上级为空，默认为pass
            this.Result = StepResult.Pass;
            // 调用后置监听
            OnPostListener();

            StepTaskEntityBase tryBlock = SubStepRoot;
            StepTaskEntityBase finallyBlock = tryBlock.NextStep;

            try
            {
                tryBlock.Invoke(forceInvoke);
            }
            finally
            {
                // finally模块是强制调用
                finallyBlock.Invoke(true);
            }
            if (null != StepData && StepData.RecordStatus)
            {
                RecordRuntimeStatus();
            }
        }
    }
}