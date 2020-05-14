using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    [RuntimeSerializeIgnore]
    public class SequenceGroupInfo : ISequenceGroupInfo
    {
        public SequenceGroupInfo()
        {
            this._hash = string.Empty;
            this.CreationTime = DateTime.Now;
            this.ModifiedTime = DateTime.Now;
            this.SequenceGroupFile = string.Empty;
            this.SequenceParamFile = string.Empty;
            this.Modified = false;
            this.OperationPanelInfo = new OperationPanelInfo();
            this.Platform = RunnerPlatform.Default;
        }

        public string Version { get; set; }

        [RuntimeSerializeIgnore]
        public RunnerPlatform Platform { get; set; }

        private string _hash;

        public string Hash
        {
            get { return string.IsNullOrWhiteSpace(_hash) ? string.Empty : _hash; }
            set { this._hash = value; } 
        }
        public DateTime CreationTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string SequenceGroupFile { get; set; }
        public string SequenceParamFile { get; set; }
        public IOperationPanelInfo OperationPanelInfo { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        public bool Modified { get; set; }
        public ISequenceGroupInfo Clone()
        {
            SequenceGroupInfo sequenceGroupInfo = new SequenceGroupInfo()
            {
                Version = this.Version,
                Platform = this.Platform,
                _hash = string.Empty,
                CreationTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                SequenceGroupFile = string.Empty,
                SequenceParamFile = string.Empty,
                Modified = false
            };
            return sequenceGroupInfo;
        }

        #region 序列化声明及反序列化构造

        public SequenceGroupInfo(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillDeserializationInfo(info, this, this.GetType());
            if (null == OperationPanelInfo)
            {
                this.OperationPanelInfo = new OperationPanelInfo();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillSerializationInfo(info, this);
        }

        #endregion
    }
}