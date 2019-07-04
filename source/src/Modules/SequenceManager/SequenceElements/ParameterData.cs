using System;
using System.Runtime.Serialization;
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
            this.ParameterType = ParameterType.NotAvailable;
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

        #region 序列化声明及反序列化构造

        public ParameterData(SerializationInfo info, StreamingContext context)
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