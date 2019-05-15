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
    internal class Sequence : ISequence
    {
        public Sequence()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Parent = null;
            this.Index = Constants.UnverifiedIndex;
            this.Variables = new VariableCollection();
            this.Steps = new SequenceStepCollection();
            this.Behavior = RunBehavior.Normal;
        }
        [RuntimeSerializeIgnore]
        public string Name { get; set; }
        [RuntimeSerializeIgnore]
        public string Description { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public ISequenceFlowContainer Parent { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        public int Index { get; set; }

        [RuntimeType(typeof(VariableCollection))]
        public IVariableCollection Variables { get; set; }

        [RuntimeType(typeof(SequenceStepCollection))]
        public ISequenceStepCollection Steps { get; set; }

        public RunBehavior Behavior { get; set; }

        public void Initialize(ISequenceFlowContainer parent)
        {
            SequenceGroup sequenceGroup = parent as SequenceGroup;
            this.Description = string.Empty;
            this.Parent = parent;
            this.Variables = new VariableCollection();
            this.Steps = new SequenceStepCollection();
            ModuleUtils.SetElementName(this, sequenceGroup.Sequences);
        }

        ISequenceFlowContainer ICloneableClass<ISequenceFlowContainer>.Clone()
        {
            VariableCollection variables = new VariableCollection();
            ModuleUtils.CloneFlowCollection(this.Variables, variables);

            SequenceStepCollection stepCollection = new SequenceStepCollection();
            ModuleUtils.CloneFlowCollection(this.Steps, stepCollection);

            Sequence sequence = new Sequence()
            {
                Name = this.Name + Constants.CopyPostfix,
                Description = this.Description,
                Parent = null,
                Index = Constants.UnverifiedIndex,
                Variables = variables,
                Steps = stepCollection,
                Behavior = this.Behavior
            };
            return sequence;
        }

        #region 序列化声明及反序列化构造

        public Sequence(SerializationInfo info, StreamingContext context)
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