using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceStep : ISequenceStep
    {
        public SequenceStep(SequenceStepType stepType)
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
            this.RecordStatus = false;
            this.StepType = SequenceStepType.Execution;
            this.StepType = stepType;
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

        public SequenceStepType StepType { get; }

        [RuntimeSerializeIgnore]
        public string Tag { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public bool HasSubSteps => (null != SubSteps && SubSteps.Count > 0);

        public bool BreakIfFailed { get; set; }
        public bool RecordStatus { get; set; }

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
            ModuleUtils.SetElementName(this, sequence.Steps);
        }

        private void InitializeSubStep(ISequenceFlowContainer parent)
        {
            ISequenceStep sequence = parent as ISequenceStep;
            this.Parent = parent;
            ModuleUtils.SetElementName(this, sequence.SubSteps);
        }

        ISequenceFlowContainer ICloneableClass<ISequenceFlowContainer>.Clone()
        {
            SequenceStepCollection subStepCollection = null;
            if (null != this.SubSteps)
            {
                subStepCollection = new SequenceStepCollection();
                ModuleUtils.CloneFlowCollection(SubSteps, subStepCollection);
            }

            SequenceStep sequenceStep = new SequenceStep(this.StepType)
            {
                Name = this.Name + Constants.CopyPostfix,
                Description = this.Description,
                Parent = null,
                SubSteps = subStepCollection,
                Index = Constants.UnverifiedIndex,
                Function = this.Function?.Clone(),
                BreakIfFailed = this.BreakIfFailed,
                Behavior = this.Behavior,
                RecordStatus = this.RecordStatus,
                LoopCounter = this.LoopCounter?.Clone(),
                RetryCounter = this.RetryCounter?.Clone(),
            };
            return sequenceStep;
        }

        #region 序列化声明及反序列化构造

        public SequenceStep(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillDeserializationInfo(info, this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillSerializationInfo(info, this);
        }

        #endregion
    }
}