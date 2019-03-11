using System.Collections.Generic;
using System.Xml.Serialization;
using Testflow.EngineCore.Common;
using Testflow.Runtime;

namespace Testflow.EngineCore.Data.GenerationData
{
    public class SequenceGenerationInfo : ISequenceGenerationInfo
    {
        [XmlIgnore]
        [MQIgnore]
        public IList<GenerationStatus> Status => RawStatus;

        [XmlIgnore]
        [MQIgnore]
        public IList<int> SequenceIndex => RawSeqIndex;

        public SerializableList<GenerationStatus> RawStatus { get; set; }
        public SerializableList<int> RawSeqIndex { get; set; }
    }
}