using System.Collections.Generic;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Data
{
    public class TestGenerationInfo : ITestGenerationInfo, ICloneableClass<ITestGenerationInfo>
    {
        public IList<ISequenceGenerationInfo> GenerationInfos { get; }

        public IList<int> SequenceGroupIndex { get; }

        public TestGenerationInfo()
        {
            this.GenerationInfos = new List<ISequenceGenerationInfo>(CoreConstants.DefaultSequenceCapacaity);
            this.SequenceGroupIndex = new List<int>(CoreConstants.DefaultSequenceCapacaity);
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