using System;
using System.Reflection;
using Testflow.CoreCommon.Data;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.FlowControl;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.Usr;

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
            // 需要处理上次出错的Step的数据
            catch (TestflowAssertException ex)
            {
                StepTaskEntityBase errorStep = StepTaskEntityBase.GetCurrentStep(SequenceIndex, Coroutine.Id);
                if (null != errorStep && errorStep.Result == StepResult.NotAvailable)
                {
                    // 停止计时
                    errorStep.EndTiming();
                    errorStep.Result = StepResult.Failed;
                    errorStep.RecordInvocationError(ex, FailedType.AssertionFailed);
                }
                throw;
            }
            catch (TargetInvocationException ex)
            {
                StepTaskEntityBase errorStep = StepTaskEntityBase.GetCurrentStep(SequenceIndex, Coroutine.Id);
                if (null != errorStep && errorStep.Result == StepResult.NotAvailable)
                {
                    // 停止计时
                    errorStep.EndTiming();
                    errorStep.RecordTargetInvocationError(ex.InnerException);
                }
                throw;
            }
            catch (TargetException ex)
            {
                StepTaskEntityBase errorStep = StepTaskEntityBase.GetCurrentStep(SequenceIndex, Coroutine.Id);
                if (null != errorStep && errorStep.Result == StepResult.NotAvailable)
                {
                    // 停止计时
                    errorStep.EndTiming();
                    errorStep.Result = StepResult.Error;
                    errorStep.RecordInvocationError(ex, FailedType.TargetError);
                }
                throw;
            }
            catch (TestflowInternalException)
            {
                throw;
            }
            catch (TestflowException ex)
            {
                // 停止计时
                Actuator.EndTiming();
                StepTaskEntityBase errorStep = StepTaskEntityBase.GetCurrentStep(SequenceIndex, Coroutine.Id);
                if (errorStep.Result == StepResult.NotAvailable)
                {
                    errorStep.Result = StepResult.Error;
                    if (null != ex.InnerException)
                    {
                        errorStep.RecordTargetInvocationError(ex.InnerException);
                    }
                    else
                    {
                        errorStep.RecordInvocationError(ex, FailedType.TargetError);
                    }
                }
                throw;
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