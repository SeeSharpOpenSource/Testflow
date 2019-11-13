using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Data;
using Testflow.Usr;
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
            this.Behavior = RunBehavior.Normal;
            this.LoopCounter = null;
            this.RetryCounter = null;
            this.RecordStatus = false;
            this.StepType = SequenceStepType.Execution;
            this.Tag = string.Empty;
            this.AssertFailedAction = FailedAction.Terminate;
            this.InvokeErrorAction = FailedAction.Terminate;
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

        public SequenceStepType StepType { get; set; }

        [RuntimeSerializeIgnore]
        public string Tag { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public bool HasSubSteps => (null != SubSteps && SubSteps.Count > 0);

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        [Obsolete]
        public bool BreakIfFailed {
            get { return AssertFailedAction == FailedAction.Terminate; }
            set
            {
                // 为了兼容原始版本，如果BreakIfFailed为False，则用户手动修改过，应该将Assert和Invoke失败后的行为全部修改为Continue。
                if (value == false && (AssertFailedAction == FailedAction.Terminate || 
                    InvokeErrorAction == FailedAction.Terminate))
                {
                    this.AssertFailedAction = FailedAction.Continue;
                    this.InvokeErrorAction = FailedAction.Continue;
                }
            }
        }
        public bool RecordStatus { get; set; }

        public FailedAction AssertFailedAction { get; set; }
        public FailedAction InvokeErrorAction { get; set; }

        public RunBehavior Behavior { get; set; }

        [RuntimeType(typeof (LoopCounter))]
        public ILoopCounter LoopCounter { get; set; }

        [RuntimeType(typeof (RetryCounter))]
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

            SequenceStep sequenceStep = new SequenceStep()
            {
                Name = this.Name + Constants.CopyPostfix,
                Description = this.Description,
                Parent = null,
                SubSteps = subStepCollection,
                Index = Constants.UnverifiedIndex,
                Function = this.Function?.Clone(),
                Behavior = this.Behavior,
                RecordStatus = this.RecordStatus,
                StepType = this.StepType,
                AssertFailedAction = this.AssertFailedAction,
                InvokeErrorAction = this.InvokeErrorAction,
                LoopCounter = this.LoopCounter?.Clone(),
                RetryCounter = this.RetryCounter?.Clone(),
                Tag = this.Tag
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