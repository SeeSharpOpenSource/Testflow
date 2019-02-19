using System;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    internal class SequenceGroupLocationInfo
    {
        public SequenceGroupLocationInfo()
        {
            this.Name = string.Empty;
            this.SequenceFilePath = string.Empty;
            this.ParameterFilePath = string.Empty;
        }

        public string Name { get; set; }

        public string SequenceFilePath { get; set; }

        public string ParameterFilePath { get; set; }
    }
}