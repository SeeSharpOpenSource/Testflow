using System.Collections.Generic;
using System.Xml.Serialization;
using Testflow.EngineCore.Common;
using Testflow.Runtime;

namespace Testflow.EngineCore.Data.GenerationData
{
    public class TestGenerationInfo : ITestGenerationInfo
    {
        [XmlIgnore]
        [MQIgnore]
        public IList<ISequenceGenerationInfo> GenerationInfos { get { return new List<ISequenceGenerationInfo>(RawGenInfos);} }

        [XmlIgnore]
        [MQIgnore]
        public IList<int> SequenceGroupIndex => RawSeqGroupIndex;

        public SerializableList<SequenceGenerationInfo> RawGenInfos { get; set; }
        public SerializableList<int> RawSeqGroupIndex { get; set; }
    }
}