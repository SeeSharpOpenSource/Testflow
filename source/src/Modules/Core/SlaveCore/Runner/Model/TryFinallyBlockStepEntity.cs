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
            // 如果是取消状态并且不是强制执行则返回
            if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
            {
                this.Result = StepResult.Abort;
                return;
            }
            // 应为TryFinally块上级为空，默认为pass
            this.Result = StepResult.Pass;
            
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