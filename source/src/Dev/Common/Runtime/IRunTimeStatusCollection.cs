using System;
using System.Collections.Generic;

namespace Testflow.Runtime
{
    /// <summary>
    /// 运行时状态集合
    /// </summary>
    public interface IRuntimeStatusCollection : IDictionary<int, IRuntimeStatusInfo>
    {
         
    }
}