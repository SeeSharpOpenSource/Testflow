using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Testflow.Common;

namespace Testflow.Runtime
{
    /// <summary>
    /// 保存一个测试序列组运行时单个监视点的即时状态信息，运行引擎内部使用。
    /// </summary>
    public interface IRuntimeStatusInfo : IPropertyExtendable
    {
        /// <summary>
        /// 所在的运行时会话
        /// </summary>
        IRuntimeSession Session { get; }

        /// <summary>
        /// 运行时的ID
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// 当前会话的开始时间
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// 实际运行时间
        /// </summary>
        TimeSpan ElapsedTime { get; }

        /// <summary>
        /// 总运行时间
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// 内存占用
        /// </summary>
        long MemoryUsed { get; }

        /// <summary>
        /// 内存占用
        /// </summary>
        long MemoryAllocated { get; }

        /// <summary>
        /// CPU使用事件
        /// </summary>
        TimeSpan ProcessorTime { get; }

        /// <summary>
        /// 运行时状态
        /// </summary>
        RuntimeState State { get; }

        /// <summary>
        /// 调用堆栈
        /// </summary>
        ICallStack CallStack { get; }

        /// <summary>
        /// 变量的事实取值
        /// </summary>
        Dictionary<string, object> VariableValues { get; }
    }
}