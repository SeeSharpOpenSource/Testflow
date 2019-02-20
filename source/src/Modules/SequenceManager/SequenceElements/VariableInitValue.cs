using System;
using Testflow.Data.Sequence;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class VariableInitValue : IVariableInitValue
    {
        public VariableInitValue()
        {
            this.Name = string.Empty;
            this.Value = string.Empty;
        }

        public string Name { get; set; }
        public string Value { get; set; }


        public ISequenceDataContainer Clone()
        {
            VariableInitValue variableInitValue = new VariableInitValue()
            {
                Name = this.Name,
                Value = this.Value
            };
            return variableInitValue;
        }

        public void Initialize(ISequenceFlowContainer parent)
        {
            // ignore
        }
    }
}