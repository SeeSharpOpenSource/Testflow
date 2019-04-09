using System;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 会话状态数据
    /// </summary>
    public class RuntimeStatusData
    {
        /// <summary>
        /// 运行时哈希值
        /// </summary>
        public string RuntimeHash { get; set; }

        /// <summary>
        /// 序列会话ID
        /// </summary>
        public int Session { get; set; }

        /// <summary>
        /// 序列索引号
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 状态索引
        /// </summary>
        public long StatusIndex { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 开始执行的时间
        /// </summary>
        public double ElapsedTime { get; set; }

        /// <summary>
        /// Step执行结果
        /// </summary>
        public StepResult Result { get; set; }

        /// <summary>
        /// 当前堆栈信息
        /// </summary>
        public string Stack { get; set; }

        /// <summary>
        /// 监控的变量值
        /// </summary>
        public string WatchData { get; set; }

    }
}