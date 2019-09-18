using System;
using System.Collections.Generic;
using Testflow.Usr;
using Testflow.Data.Sequence;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 保存一个测试序列组运行时单个监视点的即时状态信息，运行引擎内部使用。
    /// </summary>
    public interface IRuntimeStatusInfo : IPropertyExtendable
    {
        /// <summary>
        /// 所在的运行时会话
        /// </summary>
        int SessionId { get; }

        /// <summary>
        /// 当前报告在当前序列信息发送的索引号
        /// </summary>
        ulong StatusIndex { get; set; }

        /// <summary>
        /// 开始生成的时间
        /// </summary>
        DateTime StartGenTime { get;  }

        /// <summary>
        /// 结束生成的时间
        /// </summary>
        DateTime EndGenTime { get; }

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
        DateTime CurrentTime { get; }

        /// <summary>
        /// 内存占用
        /// </summary>
        long MemoryUsed { get; }

        /// <summary>
        /// 内存占用
        /// </summary>
        long MemoryAllocated { get; }

        /// <summary>
        /// CPU使用事件，单位为ms
        /// </summary>
        double ProcessorTime { get; }

        /// <summary>
        /// 运行时状态
        /// </summary>
        RuntimeState State { get; }

        /// <summary>
        /// 所有序列执行的堆栈信息
        /// </summary>
        IList<ICallStack> CallStacks { get; }

        /// <summary>
        /// 所有序列的状态信息
        /// </summary>
        IList<RuntimeState> SequenceState { get; }

        /// <summary>
        /// 失败信息
        /// </summary>
        IDictionary<int, IFailedInfo> FailedInfos { get; set; }

        /// <summary>
        /// 被监视的变量数据
        /// </summary>
        IDictionary<IVariable, string> WatchDatas { get; set; }
    }
}