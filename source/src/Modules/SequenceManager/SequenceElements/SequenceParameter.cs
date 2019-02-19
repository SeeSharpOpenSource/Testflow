using System;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    internal class SequenceParameter : ISequenceParameter
    {
        public SequenceParameter()
        {
            this.Index = Constants.UnverifiedIndex;
            this.StepParameters = null;
            this.VariableValues = null;
        }
        public int Index { get; set; }
        public IList<ISequenceStepParameter> StepParameters { get; set; }
        public IList<IVariableInitValue> VariableValues { get; set; }

        public void Initialize(ISequenceFlowContainer flowContainer)
        {
            ISequence sequence = flowContainer as ISequence;
            this.Index = sequence.Index;

            this.StepParameters = new SequenceStepParameterCollection();
            foreach (ISequenceStep sequenceStep in sequence.Steps)
            {
                SequenceStepParameter stepParameter = new SequenceStepParameter();
                stepParameter.Initialize(sequenceStep);
                this.StepParameters.Add(stepParameter);
            }

            this.VariableValues = new VariableInitValueCollection();
            foreach (IVariable variable in sequence.Variables)
            {
                VariableInitValue initValue = new VariableInitValue();
                initValue.Initialize(variable);
                this.VariableValues.Add(initValue);
            }
        }

        public ISequenceDataContainer Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}