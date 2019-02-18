using System;
using System.Linq;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class Sequence : ISequence
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
        public string Name { get; set; }
        public string Description { get; set; }
        public ISequenceFlowContainer Parent { get; set; }
        

        public int Index { get; set; }

        public IVariableCollection Variables { get; set; }

        public ISequenceStepCollection Steps { get; set; }

        public RunBehavior Behavior { get; set; }

        public void Initialize(ISequenceFlowContainer parent)
        {
            SequenceGroup sequenceGroup = this.Parent as SequenceGroup;
            string[] existNames = existNames = (from sequence in sequenceGroup.Sequences
                where !ReferenceEquals(sequence, this)
                select sequence.Name).ToArray();
            if (!Common.Utility.IsValidName(this.Name))
            {
                this.Name = Common.Utility.GetDefaultName(existNames, Constants.SequenceNameFormat, 0);
            }
            this.Description = string.Empty;
            this.Parent = parent;
            this.Variables = new VariableCollection();
            this.Steps = new SequenceStepCollection();
        }

        ISequenceFlowContainer ICloneableClass<ISequenceFlowContainer>.Clone()
        {
            VariableCollection variables = new VariableCollection();
            Common.Utility.CloneFlowCollection(this.Variables, variables);

            SequenceStepCollection stepCollection = new SequenceStepCollection();
            Common.Utility.CloneFlowCollection(this.Steps, stepCollection);

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
    }
}