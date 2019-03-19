﻿using System.Collections.Generic;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Data
{
    public class SequenceGenerationInfo : ISequenceGenerationInfo, ICloneableClass<ISequenceGenerationInfo>
    {
        public IList<GenerationStatus> Status { get; }

        public IList<int> SequenceIndex { get; }

        public SequenceGenerationInfo()
        {
            this.Status = new List<GenerationStatus>(CoreConstants.DefaultSequenceCapacaity);
            this.SequenceIndex = new List<int>(CoreConstants.DefaultSequenceCapacaity);
        }

        public ISequenceGenerationInfo Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}