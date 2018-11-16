using System;
using System.Collections.Generic;
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
        ICallStack DebugStack { get; }

        /// <summary>
        /// 从起始或者上个断点到当前执行结束的事件
        /// </summary>
        TimeSpan ElapsedTime { get; }

        /// <summary>
        /// 变量调试信息
        /// </summary>
        Dictionary<string, object> VariableDebugInfo { get; }
    }
}