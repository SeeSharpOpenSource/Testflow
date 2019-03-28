using System.Collections.Generic;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Data
{
    public class SequenceGenerationInfo : ISequenceGenerationInfo, ICloneableClass<ISequenceGenerationInfo>
    {
        public IList<GenerationStatus> Status { get; }
        public IList<int> Session { get; }

        public SequenceGenerationInfo()
        {
            this.Status = new List<GenerationStatus>(CoreConstants.DefaultSequenceCapacaity);
        }

        public ISequenceGenerationInfo Clone()
        {
            throw new System.NotImplementedException();
        }

        ISerializableMap<int, GenerationStatus> ISequenceGenerationInfo.Status { get; }
    }
}