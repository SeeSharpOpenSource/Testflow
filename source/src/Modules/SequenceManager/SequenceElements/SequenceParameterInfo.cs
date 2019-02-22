using System;
using System.Xml.Serialization;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceParameterInfo : ISequenceParameterInfo
    {
        public SequenceParameterInfo()
        {
            this.Version = string.Empty;
            this.Hash = string.Empty;
            this.CreateTime = DateTime.Now;
            this.ModifiedTime = DateTime.Now;
            this.Path = string.Empty;
            this.Modified = true;
        }
        public string Version { get; set; }
        public string Hash { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime CreateTime { get; set; }
        public string Path { get; set; }
        [XmlIgnore]
        [SerializationIgnore]
        public bool Modified { get; set; }
    }
}