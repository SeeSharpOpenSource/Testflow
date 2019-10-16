using System;
using System.Collections.Generic;
using System.Reflection;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner.Actuators;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore.Runner.Model
{
    internal abstract class StepTaskEntityBase
    {
        public static StepTaskEntityBase GetStepModel(ISequenceStep stepData, SlaveContext context, int sequenceIndex)
        {
            if (null == stepData.Function)
            {
                return new EmptyStepEntity(context, sequenceIndex, stepData);
            }
            StepTaskEntityBase stepEntity;
            switch (stepData.StepType)
            {
                case SequenceStepType.Execution:
                    stepEntity = new ExecutionStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.ConditionBlock:
                    stepEntity = new ConditionBlockStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.ConditionStatement:
                    stepEntity = new ConditionStatementStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.TryFinallyBlock:
                    stepEntity = new TryFinallyBlockStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.ConditionLoop:
                    stepEntity = new ConditionLoopStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.SequenceCall:
                    stepEntity = new SequenceCallStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.Goto:
                    stepEntity = new GotoStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.MultiThreadBlock:
                    stepEntity = new MultiThreadStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.TimerBlock:
                    stepEntity = new TimerBlockStepEntity(stepData, context, sequenceIndex);
                    break;
                case SequenceStepType.BatchBlock:
                    stepEntity = new BatchBlockStepEntity(stepData, context, sequenceIndex);
                    break;
                default:
                    context.LogSession.Print(LogLevel.Error, context.SessionId, 
                        $"The step type of <{stepData.Name}> is invalid.");
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.SequenceDataError, 
                        i18N.GetFStr("UnsupportedStepType", stepData.StepType.ToString()));
            }
            return stepEntity;
        }

        public static StepTaskEntityBase GetEmptyStepModel(SlaveContext context, int sequenceIndex)
        {
            return new EmptyStepEntity(context, sequenceIndex, null);
        }

        private static readonly Dictionary<int, StepTaskEntityBase> CurrentModel = new Dictionary<int, StepTaskEntityBase>(Constants.DefaultRuntimeSize);

        public static StepTaskEntityBase GetCurrentStep(int sequenceIndex)
        {
            return CurrentModel.ContainsKey(sequenceIndex) ? CurrentModel[sequenceIndex] : null;
        }

        public static void AddSequenceEntrance(StepTaskEntityBase stepModel)
        {
            CurrentModel.Add(stepModel.SequenceIndex, stepModel);
        }

        public StepTaskEntityBase NextStep { get; set; }

        protected readonly SlaveContext Context;
        protected readonly ISequenceStep StepData;
        protected readonly StepTaskEntityBase SubStepRoot;
        private Action<bool> _invokeStepAction;
        private bool _hasLoopCounter = false;
        protected readonly ActuatorBase Actuator;

        public StepResult Result { get; protected set; }
        public int SequenceIndex { get; }
        public object Return => Actuator?.Return ?? null;

        public bool BreakIfFailed => StepData?.BreakIfFailed ?? false;

        protected StepTaskEntityBase(ISequenceStep step, SlaveContext context, int sequenceIndex)
        {
            this.Context = context;
            this.StepData = step;
            this.Result = StepResult.NotAvailable;
            this.SequenceIndex = sequenceIndex;
            this.Actuator = ActuatorBase.GetActuator(step, context, sequenceIndex);
            if (null != StepData && StepData.HasSubSteps)
            {
                this.SubStepRoot = ModuleUtils.CreateSubStepModelChain(StepData.SubSteps, Context, sequenceIndex);
            }
        }

        public virtual CallStack GetStack()
        {
            return CallStack.GetStack(Context.SessionId, StepData);
        }

        public virtual void Generate()
        {
            Actuator.Generate();
            // 只有在StepData的LoopCounter不为null，loop最大值大于1，并且Step类型不是ConditionLoop的情况下才会执行LoopCounter
            _hasLoopCounter = (StepData?.LoopCounter != null && StepData.LoopCounter.MaxValue > 1 && 
                StepData.StepType != SequenceStepType.ConditionLoop);
            if (StepData?.HasSubSteps ?? false)
            {
                StepTaskEntityBase subStepEntity = SubStepRoot;
                do
                {
                    subStepEntity.Generate();
                } while (null != (subStepEntity = subStepEntity.NextStep));
            }
            InintializeInvokeAction();
        }

        private void InintializeInvokeAction()
        {
            if (null == StepData)
            {
                _invokeStepAction = InvokeStepSingleTime;
                return;
            }
            bool retryEnabled = null != StepData.RetryCounter && StepData.RetryCounter.MaxRetryTimes > 0 && StepData.RetryCounter.RetryEnabled;
            bool breakIfFailed = StepData.BreakIfFailed;
            switch (StepData.Behavior)
            {
                case RunBehavior.Normal:
                    if (retryEnabled)
                    {
                        if (breakIfFailed)
                        {
                            _invokeStepAction = InvokeStepWithRetry;
                        }
                        else
                        {
                            _invokeStepAction = InvokeStepWithRetryAndForceContinue;
                        }
                    }
                    else
                    {
                        if (breakIfFailed)
                        {
                            _invokeStepAction = InvokeStepSingleTime;
                        }
                        else
                        {
                            _invokeStepAction = InvokeStepAndForceContinue;
                        }
                    }
                    break;
                case RunBehavior.Skip:
                    _invokeStepAction = InvokeStepWithSkip;
                    break;
                case RunBehavior.ForceSuccess:
                    // 不使能Retry和BreakIfFailed
                    _invokeStepAction = InvokeStepAndForcePass;
                    break;
                case RunBehavior.ForceFailed:
                    // 不使能Retry和BreakIfFailed
                    _invokeStepAction = InvokeStepAndForceFailed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 当指定时间内该序列没有额外信息到达时传递运行时状态的信息，该方法只有在某个Sequence没有关键信息上报时使用
        /// </summary>
        public virtual void FillStatusInfo(StatusMessage statusMessage)
        {
            statusMessage.Stacks.Add(GetStack());
            statusMessage.Results.Add(this.Result);
        }

        public void SetStatusAndSendErrorEvent(StepResult result, FailedInfo failedInfo)
        {
            this.Result = result;
            // 如果发生错误，无论该步骤是否被配置为recordStatus，都需要发送状态信息
            SequenceStatusInfo statusInfo = new SequenceStatusInfo(SequenceIndex, this.GetStack(), StatusReportType.Record, Result, failedInfo);
            // 更新watch变量值
            statusInfo.WatchDatas = Context.VariableMapper.GetWatchDataValues(StepData);
            Context.StatusQueue.Enqueue(statusInfo);
        }

        /// <summary>
        /// 调用序列
        /// </summary>
        /// <param name="forceInvoke">是否忽略取消标识强制调用，在teardown中配置为true</param>
        public void Invoke(bool forceInvoke)
        {
            CurrentModel[SequenceIndex] = this;
            if (_hasLoopCounter)
            {
                string variableFullName = null;
                int currentIndex = 0;
                if (CoreUtils.IsValidVaraible(StepData.LoopCounter.CounterVariable))
                {
                    variableFullName = ModuleUtils.GetVariableFullName(StepData.LoopCounter.CounterVariable, StepData, Context.SessionId);
                }
                bool notCancelled = true;
                do
                {
                    if (null != variableFullName)
                    {
                        Context.VariableMapper.SetParamValue(variableFullName, StepData.LoopCounter.CounterVariable, currentIndex);
                    }
                    _invokeStepAction.Invoke(forceInvoke);
                    currentIndex++;
                    notCancelled = forceInvoke || !Context.Cancellation.IsCancellationRequested;
                } while (currentIndex < StepData.LoopCounter.MaxValue && notCancelled);
            }
            else
            {
                _invokeStepAction.Invoke(forceInvoke);
            }
        }

        // 单次执行序列
        protected abstract void InvokeStepSingleTime(bool forceInvoke);

        #region 调用分支

        // 使能retry调用step，且失败后强制继续执行
        private void InvokeStepWithRetryAndForceContinue(bool forceInvoke)
        {
            try
            {
                InvokeStepWithRetry(forceInvoke);
            }
            catch (TaskFailedException ex)
            {
                this.Result = StepResult.Failed;
                RecordInvocationError(ex, ex.FailedType);
            }
            catch (TestflowAssertException ex)
            {
                this.Result = StepResult.Failed;
                RecordInvocationError(ex, FailedType.AssertionFailed);
            }
            catch (TargetInvocationException ex)
            {
                this.Result = StepResult.Error;
                RecordInvocationError(ex.InnerException, FailedType.TargetError);
            }
        }

        // 使能retry调用step
        private void InvokeStepWithRetry(bool forceInvoke)
        {
            int maxRetry = StepData.RetryCounter.MaxRetryTimes;
            string retryVar = null;
            if (CoreUtils.IsValidVaraible(StepData.RetryCounter.CounterVariable))
            {
                retryVar = ModuleUtils.GetVariableFullName(StepData.RetryCounter.CounterVariable, StepData, Context.SessionId);
            }
            int retryTimes = -1;
            ApplicationException exception = null;
            do
            {
                retryTimes++;
                if (null != retryVar)
                {
                    Context.VariableMapper.SetParamValue(retryVar, StepData.RetryCounter.CounterVariable, retryTimes);
                }
                try
                {
                    InvokeStepSingleTime(forceInvoke);
                    exception = null;
                    // 成功执行一次后返回
                    break;
                }
                catch (TaskFailedException ex)
                {
                    this.Result = StepResult.Failed;
                    exception = ex;
                    RecordInvocationError(ex, ex.FailedType);
                }
                catch (TestflowAssertException ex)
                {
                    this.Result = StepResult.Failed;
                    exception = ex;
                    RecordInvocationError(ex, FailedType.AssertionFailed);
                }
                catch (TargetInvocationException ex)
                {
                    exception = ex;
                    RecordTargetInvocationError(ex);
                }
            } while (retryTimes < maxRetry);
            if (null != exception)
            {
                throw exception;
            }
        }

        // 执行step，即使失败也继续执行，如果失败，写入runtimeStatus
        private void InvokeStepAndForceContinue(bool forceInvoke)
        {
            try
            {
                InvokeStepSingleTime(forceInvoke);
            }
            catch (TaskFailedException ex)
            {
                this.Result = StepResult.Failed;
                RecordInvocationError(ex, ex.FailedType);
            }
            catch (TestflowAssertException ex)
            {
                this.Result = StepResult.Failed;
                RecordInvocationError(ex, FailedType.AssertionFailed);
            }
            catch (TargetInvocationException ex)
            {
                RecordTargetInvocationError(ex);
            }
        }

        // 跳过step
        private void InvokeStepWithSkip(bool forceInvoke)
        {
            this.Result = StepResult.Skip;
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, $"Sequence step <{this.GetStack()}> skipped.");
            // 如果当前step被标记为记录状态，则返回状态信息
            if (null != StepData && StepData.RecordStatus)
            {
                RecordRuntimeStatus();
            }
        }

        // 执行step，即使失败也继续执行。如果失败写入RuntimeStatus
        private void InvokeStepAndForcePass(bool forceInvoke)
        {
            try
            {
                InvokeStepSingleTime(forceInvoke);
            }
            catch (TaskFailedException ex)
            {
                this.Result = StepResult.Pass;
                Context.LogSession.Print(LogLevel.Warn, Context.SessionId, $"Sequence step <{this.GetStack()}> failed but force pass.");
                RecordInvocationError(ex, ex.FailedType);
            }
            catch (TestflowAssertException ex)
            {
                this.Result = StepResult.Pass;
                Context.LogSession.Print(LogLevel.Warn, Context.SessionId, $"Sequence step <{this.GetStack()}> failed but force pass.");
                RecordInvocationError(ex, FailedType.AssertionFailed);
            }
            catch (TargetInvocationException ex)
            {
                this.Result = StepResult.Pass;
                Context.LogSession.Print(LogLevel.Warn, Context.SessionId, $"Sequence step <{this.GetStack()}> failed but force pass.");
                RecordInvocationError(ex.InnerException, FailedType.TargetError);
            }
        }

        // 执行step且强制失败
        private void InvokeStepAndForceFailed(bool forceInvoke)
        {
            InvokeStepSingleTime(forceInvoke);
            this.Result = StepResult.Failed;
            Context.LogSession.Print(LogLevel.Warn, Context.SessionId, $"Sequence step <{this.GetStack()}> passed but force failed.");
            throw new TaskFailedException(SequenceIndex, FailedType.ForceFailed);
        }
        
        #endregion

        private void RecordTargetInvocationError(TargetInvocationException ex)
        {
            Exception innerException = ex.InnerException;
            if (innerException is TaskFailedException)
            {
                this.Result = StepResult.Failed;
                RecordInvocationError(innerException, ((TaskFailedException) innerException).FailedType);
            }
            else if (innerException is TestflowAssertException)
            {
                this.Result = StepResult.Failed;
                RecordInvocationError(innerException, FailedType.AssertionFailed);
            }
            else
            {
                this.Result = StepResult.Error;
                RecordInvocationError(innerException, FailedType.TargetError);
            }
        }

        protected void RecordRuntimeStatus()
        {
            SequenceStatusInfo statusInfo = new SequenceStatusInfo(SequenceIndex, this.GetStack(), StatusReportType.Record, Result);
            // 更新watch变量值
            statusInfo.WatchDatas = Context.VariableMapper.GetWatchDataValues(StepData);
            Context.StatusQueue.Enqueue(statusInfo);
        }

        private void RecordInvocationError(Exception ex, FailedType failedType)
        {
            FailedInfo failedInfo = new FailedInfo(ex, failedType);
            SequenceStatusInfo statusInfo = new SequenceStatusInfo(SequenceIndex, this.GetStack(), StatusReportType.Record, Result, failedInfo);
            // 一旦失败，需要记录WatchData
            statusInfo.WatchDatas = Context.VariableMapper.GetWatchDataValues(StepData);
            Context.StatusQueue.Enqueue(statusInfo);
            Context.LogSession.Print(LogLevel.Error, Context.SessionId, ex.Message);
        }

//        protected abstract void InvokeStep(bool forceInvoke);
    }
}