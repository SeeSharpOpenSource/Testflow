using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.FlowControl;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class SequenceTaskEntity
    {
        private readonly ISequence _sequence;
        private readonly SlaveContext _context;

        private StepTaskEntityBase _stepEntityRoot;
        public int RootCoroutineId { get; private set; }

        public SequenceTaskEntity(ISequence sequence, SlaveContext context)
        {
            this._sequence = sequence;
            this._context = context;
            this.State = RuntimeState.Idle;
            this.RootCoroutineId = -1;
        }

        public int Index => _sequence.Index;

        private int _runtimeState;

        /// <summary>
        /// 全局状态。配置规则：哪里最早获知全局状态变更就在哪里更新。
        /// </summary>
        public RuntimeState State
        {
            get { return (RuntimeState)_runtimeState; }
            set
            {
                // 如果当前状态大于等于待更新状态则不执行。因为在一次运行的实例中，状态的迁移是单向的。
                if ((int)value <= _runtimeState)
                {
                    return;
                }
                Thread.VolatileWrite(ref _runtimeState, (int)value);
            }
        }

        public void Generate(int startCoroutineId)
        {
            this.RootCoroutineId = startCoroutineId;
            this.State = RuntimeState.TestGen;
            _stepEntityRoot = ModuleUtils.CreateStepModelChain(_sequence.Steps, _context, _sequence.Index);
            if (null == _stepEntityRoot)
            {
                return;
            }

            StepTaskEntityBase stepEntity = _stepEntityRoot;
            do
            {
                stepEntity.Generate(ref startCoroutineId);
            } while (null != (stepEntity = stepEntity.NextStep));

            this.State = RuntimeState.StartIdle;
            // 添加当前根节点到stepModel管理中
            StepTaskEntityBase.AddSequenceEntrance(_stepEntityRoot);
        }

        /// <summary>
        /// 调用序列的函数
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        public void Invoke(bool forceInvoke = false)
        {
            FailedInfo failedInfo = null;
            StepResult lastStepResult = StepResult.NotAvailable;
            StatusReportType finalReportType = StatusReportType.Failed;
            try
            {
                this.State = RuntimeState.Running;
                SequenceStatusInfo startStatusInfo = new SequenceStatusInfo(Index, _stepEntityRoot.GetStack(),
                    StatusReportType.Start, StepResult.NotAvailable)
                {
                    ExecutionTime = DateTime.Now,
                    ExecutionTicks = -1,
                    CoroutineId = RootCoroutineId
                };
                _context.StatusQueue.Enqueue(startStatusInfo);

                StepTaskEntityBase stepEntity = _stepEntityRoot;
                bool notCancelled = true;
                do
                {
                    stepEntity.Invoke(forceInvoke);
                    notCancelled = forceInvoke || !_context.Cancellation.IsCancellationRequested;
                } while (null != (stepEntity = stepEntity.NextStep) && notCancelled);
                SetResultState(out lastStepResult, out finalReportType, out failedInfo);
            }
            catch (TargetInvocationException ex)
            {
                // 停止失败的step的计时
                StepTaskEntityBase currentStep = StepTaskEntityBase.GetCurrentStep(Index, RootCoroutineId);
                currentStep?.EndTiming();
                FillFinalExceptionReportInfo(ex.InnerException, out finalReportType, out lastStepResult, out failedInfo);
                // 如果抛出TargetInvokcationException到当前位置则说明内部没有发送错误事件
                if (null != currentStep && currentStep.BreakIfFailed)
                {
                    currentStep.SetStatusAndSendErrorEvent(lastStepResult, failedInfo);
                }
            }
            catch (TestflowLoopBreakException ex)
            {
                // 停止失败的step的计时
                StepTaskEntityBase currentStep = StepTaskEntityBase.GetCurrentStep(Index, RootCoroutineId);
                currentStep?.EndTiming();
                FillFinalExceptionReportInfo(ex.InnerException, out finalReportType, out lastStepResult, out failedInfo);
                // 如果抛出TargetInvokcationException到当前位置则说明内部没有发送错误事件
                if (null != currentStep && currentStep.BreakIfFailed)
                {
                    currentStep.SetStatusAndSendErrorEvent(lastStepResult, failedInfo);
                }
            }
            catch (Exception ex)
            {
                // 停止失败的step的计时
                StepTaskEntityBase currentStep = StepTaskEntityBase.GetCurrentStep(Index, RootCoroutineId);
                currentStep?.EndTiming();
                FillFinalExceptionReportInfo(ex, out finalReportType, out lastStepResult, out failedInfo);
                // 如果抛出Exception到当前位置则说明内部没有发送错误事件
                if (null != currentStep && currentStep.BreakIfFailed)
                {
                    currentStep.SetStatusAndSendErrorEvent(lastStepResult, failedInfo);
                }
            }
            finally
            {
                StepTaskEntityBase currentStep = StepTaskEntityBase.GetCurrentStep(Index, RootCoroutineId);
                // 发送结束事件，包括所有的ReturnData信息
                SequenceStatusInfo overStatusInfo = new SequenceStatusInfo(Index, currentStep.GetStack(),
                    finalReportType, StepResult.Over, failedInfo)
                {
                    ExecutionTime = DateTime.Now,
                    CoroutineId = RootCoroutineId,
                    ExecutionTicks = 0
                };
                overStatusInfo.WatchDatas = _context.VariableMapper.GetReturnDataValues(_sequence);
                this._context.StatusQueue.Enqueue(overStatusInfo);

                _context.VariableMapper.ClearSequenceVariables(_sequence);
                this._stepEntityRoot = null;
                // 将失败步骤职责链以后的step标记为null
                currentStep.NextStep = null;
            }
        }

        private void FillFinalExceptionReportInfo(Exception ex, out StatusReportType finalReportType,
            out StepResult lastStepResult, out FailedInfo failedInfo)
        {
            if (ex is TaskFailedException)
            {
                this.State = RuntimeState.Failed;
                finalReportType = StatusReportType.Failed;
                lastStepResult = StepResult.Failed;
                failedInfo = new FailedInfo(ex, ((TaskFailedException) ex).FailedType);
                _context.LogSession.Print(LogLevel.Info, Index, "Step force failed.");
            }
            else if (ex is TestflowAssertException)
            {
                this.State = RuntimeState.Failed;
                finalReportType = StatusReportType.Failed;
                lastStepResult = StepResult.Failed;
                failedInfo = new FailedInfo(ex, FailedType.AssertionFailed);
                _context.LogSession.Print(LogLevel.Error, Index, "Assert exception catched.");
            }
            else if (ex is ThreadAbortException)
            {
                this.State = RuntimeState.Abort;
                finalReportType = StatusReportType.Error;
                lastStepResult = StepResult.Abort;
                failedInfo = new FailedInfo(ex, FailedType.Abort);
                _context.LogSession.Print(LogLevel.Warn, Index, $"Sequence {Index} execution aborted");
            }
            else if (ex is TestflowException)
            {
                this.State = RuntimeState.Error;
                finalReportType = StatusReportType.Error;
                lastStepResult = StepResult.Error;
                failedInfo = new FailedInfo(ex, FailedType.RuntimeError);
                _context.LogSession.Print(LogLevel.Error, Index, ex, "Inner exception catched.");
            }
            else
            {
                this.State = RuntimeState.Error;
                finalReportType = StatusReportType.Error;
                lastStepResult = StepResult.Error;
                failedInfo = new FailedInfo(ex, FailedType.RuntimeError);
                _context.LogSession.Print(LogLevel.Error, Index, ex, "Runtime exception catched.");
            }
//            else if (ex is TargetInvocationException)
//            {
//                this.State = RuntimeState.Failed;
//                finalReportType = StatusReportType.Failed;
//                lastStepResult = StepResult.Failed;
//                failedInfo = new FailedInfo(ex.InnerException, FailedType.TargetError);
//                _context.LogSession.Print(LogLevel.Error, Index, ex, "Invocation exception catched.");
//            }
        }

        private void SetResultState(out StepResult lastStepResult, out StatusReportType finalReportType, 
            out FailedInfo failedInfo)
        {
            StepTaskEntityBase lastStep = StepTaskEntityBase.GetCurrentStep(this.Index, RootCoroutineId);
            lastStepResult = lastStep.Result;
            failedInfo = null;
            switch (lastStepResult)
            {
                case StepResult.Skip:
                case StepResult.Pass:
                    this.State = RuntimeState.Success;
                    finalReportType = StatusReportType.Over;
                    break;
                case StepResult.Failed:
                    this.State = RuntimeState.Failed;
                    finalReportType = StatusReportType.Failed;
                    break;
                case StepResult.Abort:
                    this.State = RuntimeState.Abort;
                    finalReportType = StatusReportType.Error;
                    failedInfo = new FailedInfo("Sequence aborted", FailedType.Abort);
                    _context.LogSession.Print(LogLevel.Warn, Index, $"Sequence {Index} execution aborted");
                    break;
                case StepResult.Timeout:
                    this.State = RuntimeState.Timeout;
                    finalReportType = StatusReportType.Error;
                    break;
                case StepResult.Error:
                    this.State = RuntimeState.Error;
                    finalReportType = StatusReportType.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void FillStatusInfo(StatusMessage message)
        {
            // 如果是外部调用且该序列已经执行结束或者未开始或者message中已经有了当前序列的信息，则说明该序列在前面的消息中已经标记结束，直接返回。
            if (message.InterestedSequence.Contains(this.Index) || this.State > RuntimeState.AbortRequested || this.State == RuntimeState.StartIdle)
            {
                return;
            }
            message.SequenceStates.Add(this.State);
            StepTaskEntityBase currentStep = StepTaskEntityBase.GetCurrentStep(Index, RootCoroutineId);
            currentStep.FillStatusInfo(message);
        }

        public void FillStatusInfo(StatusMessage message, string errorInfo)
        {
            // 如果是外部调用且该序列已经执行结束或者message中已经有了当前序列的信息，则说明该序列在前面的消息中已经标记结束，直接返回。
            if (message.InterestedSequence.Contains(this.Index) || this.State > RuntimeState.AbortRequested)
            {
                return;
            }
            message.Stacks.Add(StepTaskEntityBase.GetCurrentStep(Index, RootCoroutineId).GetStack());
            message.SequenceStates.Add(this.State);
            message.Results.Add(StepResult.NotAvailable);
        }
    }
}