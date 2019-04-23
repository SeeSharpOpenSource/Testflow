﻿using System;
using System.Reflection;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
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

        public SequenceTaskEntity(ISequence sequence, SlaveContext context)
        {
            this._sequence = sequence;
            this._context = context;
            this.State = RuntimeState.Idle;
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

        public void Generate()
        {
            this.State = RuntimeState.TestGen;

            _stepEntityRoot = ModuleUtils.CreateStepModelChain(_sequence.Steps, _context);
            _stepEntityRoot.GenerateInvokeInfo();
            _stepEntityRoot.InitializeParamsValues();

            this.State = RuntimeState.StartIdle;
            // 添加当前根节点到stepModel管理中
            StepTaskEntityBase.AddSequenceEntrance(_stepEntityRoot);
        }

        public void Invoke()
        {
            try
            {
                this.State = RuntimeState.Running;
                SequenceStatusInfo startStatusInfo = new SequenceStatusInfo(Index, _stepEntityRoot.GetStack(), StatusReportType.Start);
                _context.StatusQueue.Enqueue(startStatusInfo);

                _stepEntityRoot.Invoke();

                this.State = RuntimeState.Over;

                SequenceStatusInfo overStatusInfo = new SequenceStatusInfo(Index,
                    StepTaskEntityBase.GetCurrentStep(Index).GetStack(), StatusReportType.Over);

                _context.StatusQueue.Enqueue(overStatusInfo);
            }
            catch (TestflowAssertException ex)
            {
                this.State = RuntimeState.Failed;
                SequenceStatusInfo errorStatusInfo = new SequenceStatusInfo(Index,
                    StepTaskEntityBase.GetCurrentStep(Index).GetStack(), StatusReportType.Failed, ex);
                this._context.StatusQueue.Enqueue(errorStatusInfo);
            }
            catch (TestflowException ex)
            {
                this.State = RuntimeState.Error;
                SequenceStatusInfo errorStatusInfo = new SequenceStatusInfo(Index,
                    StepTaskEntityBase.GetCurrentStep(Index).GetStack(), StatusReportType.Failed, ex);
                this._context.StatusQueue.Enqueue(errorStatusInfo);
            }
            catch (TargetInvocationException ex)
            {
                this.State = RuntimeState.Error;
                SequenceStatusInfo errorStatusInfo = new SequenceStatusInfo(Index,
                    StepTaskEntityBase.GetCurrentStep(Index).GetStack(), StatusReportType.Error, ex.InnerException);
                this._context.StatusQueue.Enqueue(errorStatusInfo);
            }
            catch (Exception ex)
            {
                this.State = RuntimeState.Error;
                SequenceStatusInfo errorStatusInfo = new SequenceStatusInfo(Index,
                    StepTaskEntityBase.GetCurrentStep(Index).GetStack(), StatusReportType.Error, ex);
                this._context.StatusQueue.Enqueue(errorStatusInfo);
            }

        }

        public void FillStatusInfo(StatusMessage message)
        {
            // 如果是外部调用且该序列已经执行结束或者message中已经有了当前序列的信息，则说明该序列在前面的消息中已经标记结束，直接返回。
            if (message.InterestedSequence.Contains(this.Index) || this.State > RuntimeState.AbortRequested)
            {
                return;
            }
            message.Stacks.Add(StepTaskEntityBase.GetCurrentStep(Index).GetStack());
            message.SequenceStates.Add(this.State);
            message.Results.Add(StepResult.NotAvailable);
        }

        public void FillStatusInfo(StatusMessage message, string errorInfo)
        {
            // 如果是外部调用且该序列已经执行结束或者message中已经有了当前序列的信息，则说明该序列在前面的消息中已经标记结束，直接返回。
            if (message.InterestedSequence.Contains(this.Index) || this.State > RuntimeState.AbortRequested)
            {
                return;
            }
            message.Stacks.Add(StepTaskEntityBase.GetCurrentStep(Index).GetStack());
            message.SequenceStates.Add(this.State);
            message.Results.Add(StepResult.NotAvailable);
            message.FailedInfo.Add(Index, errorInfo);
        }
    }
}