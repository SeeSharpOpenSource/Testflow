using System;
using System.Collections.Generic;
using Testflow.Usr;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 调试信息
    /// </summary>
    public interface IDebugInformation
    {
        /// <summary>
        /// 断点命中信息
        /// </summary>
        ICallStack BreakPoint { get; }

        /// <summary>
        /// 变量调试信息
        /// </summary>
        IDictionary<IVariable, string> WatchDatas { get; }
    }
}