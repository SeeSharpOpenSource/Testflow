using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.Debug
{
    public interface IDebugSession
    {
        IDebugContext Context { get; }

        IList<ISequenceStep> UnreachedBreakPoints { get; }
        
        ISequenceStep CurrentStep { get; }

        


    }
}