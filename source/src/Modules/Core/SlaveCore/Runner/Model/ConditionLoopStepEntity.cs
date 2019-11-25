using System;
using Testflow.CoreCommon.Common;
using Testflow.Data.Sequence;
using Testflow.FlowControl;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class ConditionLoopStepEntity : StepTaskEntityBase
    {
        public ConditionLoopStepEntity(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
            this._loopVariable = null;
        }

        private string _loopVariable;

        public override void Generate(ref int coroutineId)
        {
            base.Generate(ref coroutineId);
            if (null != StepData.LoopCounter && CoreUtils.IsValidVaraible(StepData.LoopCounter.CounterVariable))
            {
                _loopVariable = ModuleUtils.GetVariableFullName(StepData.LoopCounter.CounterVariable, StepData, 
                    Context.SessionId);
            }
        }

        protected override void InvokeStepSingleTime(bool forceInvoke)
        {
            int index = -1;
            while (true)
            {
                try
                {
                    while (true)
                    {
                        // 设置循环变量的值
                        if (null != _loopVariable)
                        {
                            Context.VariableMapper.SetParamValue(_loopVariable, StepData.LoopCounter.CounterVariable,
                                index++);
                        }
                        // 重置计时时间
                        Actuator.ResetTiming();
                        // 如果是取消状态并且不是强制执行则返回
                        if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
                        {
                            this.Result = StepResult.Abort;
                            return;
                        }
                        this.Result = StepResult.Error;

                        this.Result = Actuator.InvokeStep(forceInvoke);
                        // 如果当前step被标记为记录状态，则返回状态信息
                        if (null != StepData && StepData.RecordStatus)
                        {
                            RecordRuntimeStatus();
                        }
                        // 如果执行结果不是bool类型或者为False，则退出当前循环
                        object returnValue = Actuator.Return;
                        if (!(returnValue is bool) || !(bool) returnValue)
                        {
                            return;
                        }
                        // 执行所有的下级Step
                        if (null != StepData && StepData.HasSubSteps)
                        {
                            StepTaskEntityBase subStepEntity = SubStepRoot;
                            bool notCancelled = true;
                            do
                            {
                                if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
                                {
                                    this.Result = StepResult.Abort;
                                    return;
                                }
                                subStepEntity.Invoke(forceInvoke);
                                notCancelled = forceInvoke || !Context.Cancellation.IsCancellationRequested;
                            } while (null != (subStepEntity = subStepEntity.NextStep) && notCancelled);
                        }
                    }
                }
                catch (TestflowLoopBreakException ex)
                {
                    ex.CalcDown();
                    // 如果计数大于0，则继续抛出到上层
                    if (ex.Count > 0)
                    {
                        throw;
                    }
                    // 如果是跳出当前循环，则return
                    if (ex.BreakLoop)
                    {
                        return;
                    }
                }
            }
        }
    }
}