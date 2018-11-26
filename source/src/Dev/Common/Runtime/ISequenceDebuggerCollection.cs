using System.Collections.Generic;
using Testflow.Common;

namespace Testflow.Runtime
{
    /// <summary>
    /// 序列调试器的集合
    /// </summary>
    public interface ISequenceDebuggerCollection : ISerializableMap<int, ISequenceDebugger>
    {
         
    }
}