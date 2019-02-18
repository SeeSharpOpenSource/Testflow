using System;
using System.Linq;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceStep : ISequenceStep
    {
        public SequenceStep()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Parent = null;
            this.SubSteps = null;
            this.Index = Constants.UnverifiedIndex;
            this.Function = null;
            this.BreakIfFailed = true;
            this.Behavior = RunBehavior.Normal;
            this.LoopCounter = null;
            this.RetryCounter = null;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public ISequenceFlowContainer Parent { get; set; }

        public ISequenceStepCollection SubSteps { get; set; }

        public int Index { get; set; }

        public IFunctionData Function { get; set; }

        public bool HasSubSteps => (null != SubSteps && SubSteps.Count > 0);

        public bool BreakIfFailed { get; set; }

        public RunBehavior Behavior { get; set; }

        public ILoopCounter LoopCounter { get; set; }

        public IRetryCounter RetryCounter { get; set; }

        public void Initialize(ISequenceFlowContainer parent)
        {
            if (parent is ISequence)
            {
                InitializeSubStep(parent);
            }
            else if (parent is ISequenceStep)
            {
                InitializeStep(parent);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void InitializeStep(ISequenceFlowContainer parent)
        {
            ISequence sequence = parent as ISequence;
            string[] existNames = (from step in sequence.Steps where !ReferenceEquals(step, this) select step.Name).ToArray();
            if (Common.Utility.IsValidName(this.Name, existNames))
            {
                this.Name = Common.Utility.GetDefaultName(existNames, Constants.StepNameFormat, 0);
            }
            this.Parent = parent;
        }

        private void InitializeSubStep(ISequenceFlowContainer parent)
        {
            ISequenceStep sequence = parent as ISequenceStep;
            string[] existNames = (from step in sequence.SubSteps where !ReferenceEquals(step, this) select step.Name).ToArray();
            if (Common.Utility.IsValidName(this.Name, existNames))
            {
                this.Name = Common.Utility.GetDefaultName(existNames, Constants.StepNameFormat, 0);
            }
            this.Parent = parent;
        }

        ISequenceFlowContainer ICloneableClass<ISequenceFlowContainer>.Clone()
        {
            SequenceStepCollection subStepCollection = null;
            if (null != this.SubSteps)
            {
                subStepCollection = new SequenceStepCollection();
                Common.Utility.CloneFlowCollection(SubSteps, subStepCollection);
            }

            SequenceStep sequenceStep = new SequenceStep()
            {
                Name = this.Name + Constants.CopyPostfix,
                Description = this.Description,
                Parent = null,
                SubSteps = subStepCollection,
                Index = Constants.UnverifiedIndex,
                Function = this.Function?.Clone(),
                BreakIfFailed = this.BreakIfFailed,
                Behavior = this.Behavior,
                LoopCounter = this.LoopCounter?.Clone(),
                RetryCounter = this.RetryCounter?.Clone(),
            };
            return sequenceStep;
        }
    }
}