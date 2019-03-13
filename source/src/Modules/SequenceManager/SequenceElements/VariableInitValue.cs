using System;
using System.Runtime.Serialization;
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

        #region 序列化声明及反序列化构造

        public VariableInitValue(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillDeserializationInfo(info, this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillSerializationInfo(info, this);
        }

        #endregion
    }
}