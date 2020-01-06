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
            int index = 0;
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
                        // 如果是取消状态并且不是强制执行则返回。因为ConditionLoop内部有循环，所以需要在内部也做判断
                        if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
                        {
                            BreakAndReportAbortMessage();
                        }
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
                        // 如果执行结果不是bool类型或者为False，或者没有下级节点，则退出当前循环
                        object returnValue = Actuator.Return;
                        bool conditionFail = !(returnValue is bool) || !(bool) returnValue;
                        if (conditionFail || null == StepData || !StepData.HasSubSteps)
                        {
                            return;
                        }
                        // 执行所有的下级Step
                        StepTaskEntityBase subStepEntity = SubStepRoot;
                        do
                        {
                            if (!forceInvoke && Context.Cancellation.IsCancellationRequested)
                            {
                                this.Result = StepResult.Abort;
                                return;
                            }
                            subStepEntity.Invoke(forceInvoke);
                        } while (null != (subStepEntity = subStepEntity.NextStep));
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