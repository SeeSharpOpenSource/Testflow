using System.Collections.Generic;
using Testflow.Common;
using Testflow.MasterCore.Common;
using Testflow.Runtime;

namespace Testflow.MasterCore.Data
{
    public class SequenceGenerationInfo : ISequenceGenerationInfo, ICloneableClass<ISequenceGenerationInfo>
    {
        public IList<GenerationStatus> Status { get; }

        public IList<int> SequenceIndex { get; }

        public SequenceGenerationInfo()
        {
            this.Status = new List<GenerationStatus>(Constants.DefaultSequenceCapacaity);
            this.SequenceIndex = new List<int>(Constants.DefaultSequenceCapacaity);
        }

        public ISequenceGenerationInfo Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}