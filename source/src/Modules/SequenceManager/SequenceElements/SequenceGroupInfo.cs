using System;
using Testflow.Data.Sequence;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceGroupInfo : ISequenceGroupInfo
    {
        public SequenceGroupInfo()
        {
            SequenceManager sequenceManager = SequenceManager.GetInstance();
            this.Version = sequenceManager.Version;
            this._hash = string.Empty;
            this.CreationTime = DateTime.Now;
            this.ModifiedTime = DateTime.Now;
            this.SequenceGroupFile = string.Empty;
            this.SequenceParamFile = string.Empty;
            this.Modified = false;
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
    }
}