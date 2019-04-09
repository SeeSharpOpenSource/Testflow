using System;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 性能统计信息
    /// </summary>
    public class PerformanceStatus
    {
         /// <summary>
         /// 运行时哈希
         /// </summary>
        public string RuntimeHash { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        public int Session { get; set; }

        /// <summary>
        /// 当前会话的性能数据索引
        /// </summary>
        public long Index { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 内存占用
        /// </summary>
        public long MemoryUsed { get; set; }

        /// <summary>
        /// 内存占用
        /// </summary>
        public long MemoryAllocated { get; set; }

        /// <summary>
        /// CPU使用事件，单位为ms
        /// </summary>
        public ulong ProcessorTime { get; set; }
    }
}