using System;
using System.Collections.Generic;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Runner.Model
{
    internal abstract class StepTaskEntityBase
    {
        public static StepTaskEntityBase GetStepModel(ISequenceStep stepData, SlaveContext context)
        {
            if (stepData.HasSubSteps)
            {
                return new StepExecutionEntity(stepData, context);
            }
            switch (stepData.Function.Type)
            {
                case FunctionType.Constructor:
                case FunctionType.InstanceFunction:
                case FunctionType.StaticFunction:
                    return new StepExecutionEntity(stepData, context);
                    break;
                case FunctionType.Assertion:
                    return new StepAssertEntity(stepData, context);
                    break;
                case FunctionType.CallBack:
                    return new StepCallBackEntity(stepData, context);
                    break;
                default:
                    throw new InvalidOperationException();
                    break;
            }
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

        public StepResult Result { get; protected set; }
        public int SequenceIndex { get; }

        protected StepTaskEntityBase(ISequenceStep step, SlaveContext context)
        {
            this.Context = context;
            this.StepData = step;
            this.Result = StepResult.NotAvailable;
            this.SequenceIndex = step.Index;
        }

        public CallStack GetStack()
        {
            return CallStack.GetStack(StepData);
        }

        public abstract void GenerateInvokeInfo();

        public abstract void InitializeParamsValues();

        // 该方法只有在某个Sequence没有关键信息上报时使用。
        /// <summary>
        /// 当指定时间内该序列没有额外信息到达时传递运行时状态的信息
        /// </summary>
        public virtual void FillStatusInfo(StatusMessage statusMessage)
        {
            statusMessage.Stacks.Add(GetStack());
            statusMessage.Results.Add(this.Result);
        }

        public abstract void Invoke();
    }
}