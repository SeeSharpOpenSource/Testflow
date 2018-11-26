using System;
using System.Collections.Generic;
using Testflow.Common;
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
        /// CPU使用事件，单位为ms
        /// </summary>
        ulong ElapsedTime { get; }

        /// <summary>
        /// 变量调试信息
        /// </summary>
        ISerializableMap<string, object> VariableDebugInfo { get; }
    }
}