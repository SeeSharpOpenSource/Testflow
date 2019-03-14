using System.Collections.Generic;
using Testflow.Common;
using Testflow.EngineCore.Common;
using Testflow.Runtime;

namespace Testflow.EngineCore.Data
{
    public class TestGenerationInfo : ITestGenerationInfo, ICloneableClass<ITestGenerationInfo>
    {
        public IList<ISequenceGenerationInfo> GenerationInfos { get; }

        public IList<int> SequenceGroupIndex { get; }

        public TestGenerationInfo()
        {
            this.GenerationInfos = new List<ISequenceGenerationInfo>(Constants.DefaultSequenceCapacaity);
            this.SequenceGroupIndex = new List<int>(Constants.DefaultSequenceCapacaity);
        }

        private TestGenerationInfo(ITestGenerationInfo testGeneration)
        {
            this.GenerationInfos = new List<ISequenceGenerationInfo>();
        }

        public ITestGenerationInfo Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}