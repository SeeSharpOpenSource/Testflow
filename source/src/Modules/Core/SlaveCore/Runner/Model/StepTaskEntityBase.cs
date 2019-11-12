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
        // 块步骤类型集合。块集合中的Function可以为null或者定义在funcdefs的特殊类型接口
        private static HashSet<SequenceStepType> _blockStepTypes = new HashSet<SequenceStepType>();

        static StepTaskEntityBase()
        {
            _blockStepTypes.Add(SequenceStepType.ConditionBlock);
            _blockStepTypes.Add(SequenceStepType.TryFinallyBlock);
            _blockStepTypes.Add(SequenceStepType.MultiThreadBlock);
            _blockStepTypes.Add(SequenceStepType.TimerBlock);
            _blockStepTypes.Add(SequenceStepType.BatchBlock);
        }

        public static StepTaskEntityBase GetStepModel(ISequenceStep stepData, SlaveContext context, int sequenceIndex)
        {
            // 如果某个Step的Function为null，且不是块步骤类型，则为该Step分配空类型运行实体
            if (null == stepData.Function && !_blockStepTypes.Contains(stepData.StepType))
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

        private static readonly Dictionary<int, Dictionary<int, StepTaskEntityBase>> CurrentModel = 
            new Dictionary<int, Dictionary<int, StepTaskEntityBase>>(Constants.DefaultRuntimeSize);

        public static StepTaskEntityBase GetCurrentStep(int sequenceIndex, int coroutineId)
        {
//            return CurrentModel.ContainsKey(sequenceIndex) ? CurrentModel[sequenceIndex] : null;
            return CurrentModel[sequenceIndex][coroutineId];
        }

        public static void AddSequenceEntrance(StepTaskEntityBase stepModel)
        {
            if (!CurrentModel.ContainsKey(stepModel.SequenceIndex))
            {
                CurrentModel.Add(stepModel.SequenceIndex, 
                    new Dictionary<int, StepTaskEntityBase>(Constants.DefaultRuntimeSize));
            }
            CurrentModel[stepModel.SequenceIndex].Add(stepModel.CoroutineId, stepModel);
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
        public int CoroutineId { get; private set; }
        public bool BreakIfFailed { get; }

        protected StepTaskEntityBase(ISequenceStep step, SlaveContext context, int sequenceIndex)
        {
            this.Context = context;
            this.StepData = step;
            this.Result = StepResult.NotAvailable;
            this.SequenceIndex = sequenceIndex;
            this.Actuator = ActuatorBase.GetActuator(step, context, sequenceIndex);
            this.CoroutineId = -1;
            // 只有在断言失败和调用异常都配置为终止执行时，该步骤才会被判断为失败后终止
            if (null != StepData && StepData.HasSubSteps)
            {
                this.SubStepRoot = ModuleUtils.CreateSubStepModelChain(StepData.SubSteps, Context, sequenceIndex);
            }
            BreakIfFailed = (null == StepData) || (StepData.AssertFailedAction == FailedAction.Terminate &&
                                                      StepData.InvokeErrorAction == FailedAction.Terminate);
        }

        public virtual CallStack GetStack()
        {
            return CallStack.GetStack(Context.SessionId, StepData);
        }

        public virtual void Generate(ref int coroutineId)
        {
            this.CoroutineId = coroutineId;
            Actuator.Generate(coroutineId);
            // 只有在StepData的LoopCounter不为null，loop最大值大于1，并且Step类型不是ConditionLoop的情况下才会执行LoopCounter
            _hasLoopCounter = (StepData?.LoopCounter != null && StepData.LoopCounter.MaxValue > 1 && 
                StepData.StepType != SequenceStepType.ConditionLoop);
            if (StepData?.HasSubSteps ?? false)
            {
                StepTaskEntityBase subStepEntity = SubStepRoot;
                do
                {
                    subStepEntity.Generate(ref coroutineId);
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
            
            switch (StepData.Behavior)
            {
                case RunBehavior.Normal:
                    if (retryEnabled)
                    {
                        if (BreakIfFailed)
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
                        if (BreakIfFailed)
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
            SequenceStatusInfo statusInfo = new SequenceStatusInfo(SequenceIndex, this.GetStack(), StatusReportType.Record, Result, failedInfo)
            {
                ExecutionTime = Actuator.ExecutionTime,
                CoroutineId = this.CoroutineId,
                ExecutionTicks = Actuator.ExecutionTicks
            };
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
            CurrentModel[SequenceIndex][CoroutineId] = this;
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

        public void EndTiming()
        {
            Actuator.EndTiming();
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
                // 停止计时
                Actuator.EndTiming();
                this.Result = StepResult.Failed;
                // 如果InvokeErrorAction不是Continue，则抛出异常
                RecordInvocationError(ex, ex.FailedType);
                // 如果失败行为是终止，则抛出异常
                if (StepData.InvokeErrorAction == FailedAction.Terminate)
                {
                    throw;
                }
            }
            catch (TestflowAssertException ex)
            {
                // 停止计时
                Actuator.EndTiming();
                this.Result = StepResult.Failed;
                RecordInvocationError(ex, FailedType.AssertionFailed);
                // 如果失败行为是终止，则抛出异常
                if (StepData.AssertFailedAction == FailedAction.Terminate)
                {
                    throw;
                }
            }
            catch (TargetInvocationException ex)
            {
                // 停止计时
                Actuator.EndTiming();
                this.Result = StepResult.Error;
                RecordInvocationError(ex.InnerException, FailedType.TargetError);
                // 如果失败行为是终止，则抛出异常
                if (StepData.InvokeErrorAction == FailedAction.Terminate)
                {
                    throw;
                }
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
                    // 停止计时
                    Actuator.EndTiming();
                    this.Result = StepResult.Failed;
                    exception = ex;
                    RecordInvocationError(ex, ex.FailedType);
                }
                catch (TestflowAssertException ex)
                {
                    // 停止计时
                    Actuator.EndTiming();
                    this.Result = StepResult.Failed;
                    exception = ex;
                    RecordInvocationError(ex, FailedType.AssertionFailed);
                }
                catch (TargetInvocationException ex)
                {
                    // 停止计时
                    Actuator.EndTiming();
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
                // 停止计时
                Actuator.EndTiming();
                this.Result = StepResult.Failed;
                RecordInvocationError(ex, ex.FailedType);
                // 如果失败行为是终止，则抛出异常
                if (StepData.InvokeErrorAction == FailedAction.Terminate)
                {
                    throw;
                }
            }
            catch (TestflowAssertException ex)
            {
                // 停止计时
                Actuator.EndTiming();
                this.Result = StepResult.Failed;
                RecordInvocationError(ex, FailedType.AssertionFailed);
                // 如果失败行为是终止，则抛出异常
                if (StepData.AssertFailedAction == FailedAction.Terminate)
                {
                    throw;
                }
            }
            catch (TargetInvocationException ex)
            {
                // 停止计时
                Actuator.EndTiming();
                RecordTargetInvocationError(ex);
                // 如果失败行为是终止，则抛出异常
                if (StepData.InvokeErrorAction == FailedAction.Terminate)
                {
                    throw;
                }
            }
        }

        // 跳过step
        private void InvokeStepWithSkip(bool forceInvoke)
        {
            // 开始计时
            Actuator.StartTiming();
            // 停止计时
            Actuator.EndTiming();
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
                // 停止计时
                Actuator.EndTiming();
                this.Result = StepResult.Pass;
                Context.LogSession.Print(LogLevel.Warn, Context.SessionId, $"Sequence step <{this.GetStack()}> failed but force pass.");
                RecordInvocationError(ex, ex.FailedType);
            }
            catch (TestflowAssertException ex)
            {
                // 停止计时
                Actuator.EndTiming();
                this.Result = StepResult.Pass;
                Context.LogSession.Print(LogLevel.Warn, Context.SessionId, $"Sequence step <{this.GetStack()}> failed but force pass.");
                RecordInvocationError(ex, FailedType.AssertionFailed);
            }
            catch (TargetInvocationException ex)
            {
                // 停止计时
                Actuator.EndTiming();
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
            SequenceStatusInfo statusInfo = new SequenceStatusInfo(SequenceIndex, this.GetStack(), StatusReportType.Record, Result)
            {
                ExecutionTime = Actuator.ExecutionTime,
                ExecutionTicks = Actuator.ExecutionTicks,
                CoroutineId = this.CoroutineId
            };
            // 更新watch变量值
            statusInfo.WatchDatas = Context.VariableMapper.GetWatchDataValues(StepData);
            Context.StatusQueue.Enqueue(statusInfo);
        }

        private void RecordInvocationError(Exception ex, FailedType failedType)
        {
            FailedInfo failedInfo = new FailedInfo(ex, failedType);
            SequenceStatusInfo statusInfo = new SequenceStatusInfo(SequenceIndex, this.GetStack(),
                StatusReportType.Record, Result, failedInfo)
            {
                ExecutionTime = Actuator.ExecutionTime,
                ExecutionTicks = Actuator.ExecutionTicks,
                CoroutineId = this.CoroutineId,
                WatchDatas = Context.VariableMapper.GetWatchDataValues(StepData)
            };
            // 一旦失败，需要记录WatchData
            Context.StatusQueue.Enqueue(statusInfo);
            Context.LogSession.Print(LogLevel.Error, Context.SessionId, ex.Message);
        }
//        protected abstract void InvokeStep(bool forceInvoke);
    }
}