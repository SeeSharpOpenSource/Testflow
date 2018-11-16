using System.Collections.Generic;

namespace Testflow.Runtime
{
    /// <summary>
    /// 序列调试器的集合
    /// </summary>
    public interface ISequenceDebuggerCollection : IDictionary<int, ISequenceDebugger>
    {
         
    }
}