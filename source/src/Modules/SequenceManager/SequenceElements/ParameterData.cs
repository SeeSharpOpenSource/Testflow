using System;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class ParameterData : IParameterData
    {
        public ParameterData()
        {
            this.Index = Constants.UnverifiedIndex;
            this.Value = string.Empty;
            this.ParameterType = ParameterType.Value;
        }

        public int Index { get; set; }
        public string Value { get; set; }
        public ParameterType ParameterType { get; set; }

        public IParameterData Clone()
        {
            ParameterData parameterData = new ParameterData()
            {
                Index = this.Index,
                Value = this.Value,
                ParameterType = this.ParameterType
            };
            return parameterData;
        }
    }
}