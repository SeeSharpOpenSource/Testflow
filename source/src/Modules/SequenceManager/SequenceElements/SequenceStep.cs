using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
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
        [RuntimeSerializeIgnore]
        public string Name { get; set; }
        [RuntimeSerializeIgnore]
        public string Description { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public ISequenceFlowContainer Parent { get; set; }

        [RuntimeType(typeof(SequenceStepCollection))]
        public ISequenceStepCollection SubSteps { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        public int Index { get; set; }

        [RuntimeType(typeof(FunctionData))]
        public IFunctionData Function { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public bool HasSubSteps => (null != SubSteps && SubSteps.Count > 0);

        public bool BreakIfFailed { get; set; }

        public RunBehavior Behavior { get; set; }

        [RuntimeType(typeof(LoopCounter))]
        public ILoopCounter LoopCounter { get; set; }

        [RuntimeType(typeof(RetryCounter))]
        public IRetryCounter RetryCounter { get; set; }

        public void Initialize(ISequenceFlowContainer parent)
        {
            if (parent is ISequence)
            {
                InitializeStep(parent);
            }
            else if (parent is ISequenceStep)
            {
                InitializeSubStep(parent);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void InitializeStep(ISequenceFlowContainer parent)
        {
            ISequence sequence = parent as ISequence;
            this.Parent = parent;
            Common.Utility.SetElementName(this, sequence.Steps);
        }

        private void InitializeSubStep(ISequenceFlowContainer parent)
        {
            ISequenceStep sequence = parent as ISequenceStep;
            this.Parent = parent;
            Common.Utility.SetElementName(this, sequence.SubSteps);
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillSerializationInfo(info, this);
        }
    }
}