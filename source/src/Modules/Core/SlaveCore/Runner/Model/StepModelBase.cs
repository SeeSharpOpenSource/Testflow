using System;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Runner.Model
{
    internal abstract class StepModelBase
    {
        public static StepModelBase GetStepModel(ISequenceStep stepData, SlaveContext context)
        {
            if (stepData.HasSubSteps)
            {
                return new StepExecutionModel(stepData, context);
            }
            switch (stepData.Function.Type)
            {
                case FunctionType.Constructor:
                case FunctionType.InstanceFunction:
                case FunctionType.StaticFunction:
                    return new StepExecutionModel(stepData, context);
                    break;
                case FunctionType.Assertion:
                    return new StepAssertModel(stepData, context);
                    break;
                case FunctionType.CallBack:
                    return new StepCallBackModel(stepData, context);
                    break;
                default:
                    throw new InvalidProgramException();
                    break;
            }
        }

        public StepModelBase NextStep { get; set; }

        protected readonly SlaveContext Context;
        protected readonly ISequenceStep StepData;

        public StepResult Result { get; private set; }
        public int SequenceIndex { get; }

        protected StepModelBase(ISequenceStep step, SlaveContext context)
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

        public abstract void FillStatusInfo(StatusMessage statusMessage);

        public abstract void Invoke();
    }
}