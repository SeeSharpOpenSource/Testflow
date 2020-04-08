using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class Variable : IVariable
    {
        public Variable()
        {
            this.Name = string.Empty;
            this.Type = null;
            this.TypeIndex = Constants.UnverifiedTypeIndex;
            this.Description = string.Empty;
            this.LogRecordLevel = RecordLevel.None;
            this.ReportRecordLevel = RecordLevel.None;
            this.OIRecordLevel = RecordLevel.None;
            this.Value = string.Empty;
            this.Parent = null;
            this.AutoType = true;
        }

        public string Name { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public ITypeData Type { get; set; }

        public int TypeIndex { get; set; }
        public VariableType VariableType { get; set; }
        [RuntimeSerializeIgnore]
        public string Description { get; set; }
        public RecordLevel LogRecordLevel { get; set; }
        public RecordLevel ReportRecordLevel { get; set; }
        public RecordLevel OIRecordLevel { get; set; }
        public bool AutoType { get; set; }
        
        [XmlIgnore]
        [SerializationIgnore]
        public string Value { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public ISequenceFlowContainer Parent { get; set; }

        public void Initialize(ISequenceFlowContainer parent)
        {
            this.Parent = parent;
        }

        public ISequenceFlowContainer Clone()
        {
            Variable variable = new Variable()
            {
                Name = this.Name,
                Description = this.Description,
                LogRecordLevel = this.LogRecordLevel,
                ReportRecordLevel = this.ReportRecordLevel,
                OIRecordLevel = this.OIRecordLevel,
                Parent = null,
                Type = this.Type,
                TypeIndex = Constants.UnverifiedTypeIndex,
                VariableType = this.VariableType,
                AutoType = this.AutoType,
                Value = this.Value
            };
            return variable;
        }

        #region 序列化声明及反序列化构造

        public Variable(SerializationInfo info, StreamingContext context)
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