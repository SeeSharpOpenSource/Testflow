using System;
using Testflow.Common;

namespace Testflow.Runtime
{
    /// <summary>
    /// 性能结果
    /// </summary>
    public interface IPerformanceResult : IPropertyExtendable
    {
        /// <summary>
        /// CPU使用事件
        /// </summary>
        TimeSpan CpuTime { get; set; }

        /// <summary>
        /// 平均分配的内存
        /// </summary>
        long AverageAllocatedMemory { get; set; }

        /// <summary>
        /// 分配内存的最大值
        /// </summary>
        long MaxAllocatedMemory { get; set; }

        /// <summary>
        /// 平均使用的内存
        /// </summary>
        long AverageUsedMemory { get; set; }

        /// <summary>
        /// 最大使用的内存
        /// </summary>
        long MaxUsedMemory { get; set; }

    }
}