using System.Collections.Generic;
using Testflow.Usr;

namespace Testflow.Runtime
{
    /// <summary>
    /// 序列调试器的集合
    /// </summary>
    public interface ISequenceDebuggerCollection : ISerializableMap<int, IDebuggerHandle>
    {
         
    }
}