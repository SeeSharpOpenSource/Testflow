using System;
using System.Collections.Generic;
using Testflow.Common;

namespace Testflow.Runtime
{
    /// <summary>
    /// 运行时状态集合
    /// </summary>
    public interface IRuntimeStatusCollection : ISerializableMap<int, IRuntimeStatusInfo>
    {
         
    }
}