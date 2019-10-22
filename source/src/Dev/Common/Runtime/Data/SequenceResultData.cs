using System;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 序列执行状态数据
    /// </summary>
    public class SequenceResultData
    {
        /// <summary>
        /// 运行时实例哈希
        /// </summary>
        public string RuntimeHash { get; set; }

        /// <summary>
        /// 序列名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 序列描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 运行时会话id
        /// </summary>
        public int Session { get; set; }

        /// <summary>
        /// 序列Id
        /// </summary>
        public int SequenceIndex { get; set; }

        /// <summary>
        /// 序列执行结果
        /// </summary>
        public RuntimeState Result { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 根协程的逻辑ID
        /// </summary>
        public int CoroutineId { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public double ElapsedTime { get; set; }

        /// <summary>
        /// 失败信息
        /// </summary>
        public IFailedInfo FailInfo { get; set; }

        /// <summary>
        /// 失败时的堆栈信息
        /// </summary>
        public string FailStack { get; set; }
    }
}