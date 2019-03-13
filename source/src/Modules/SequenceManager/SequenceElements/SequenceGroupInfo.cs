using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Data.Sequence;
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
            this.Modified = true;
        }

        public string Version { get; set; }
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

        [XmlIgnore]
        [SerializationIgnore]
        public bool Modified { get; set; }
        public ISequenceGroupInfo Clone()
        {
            SequenceGroupInfo sequenceGroupInfo = new SequenceGroupInfo()
            {
                Version = this.Version,
                _hash = string.Empty,
                CreationTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                SequenceGroupFile = string.Empty,
                SequenceParamFile = string.Empty,
                Modified = false
            };
            return sequenceGroupInfo;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillSerializationInfo(info, this);
        }
    }
}