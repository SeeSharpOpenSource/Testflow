using System;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 会话状态数据
    /// </summary>
    public class SessionResultData
    {
        /// <summary>
        /// 运行时实例的哈希
        /// </summary>
        public string RuntimeHash { get; set; }

        /// <summary>
        /// 序列组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 序列组的描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 序列组的会话ID
        /// </summary>
        public int Session { get; set; }

        /// <summary>
        /// 序列的哈希
        /// </summary>
        public string SequenceHash { get; set; }

        /// <summary>
        /// 开始执行时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束执行时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 测试耗时
        /// </summary>
        public double ElapsedTime { get; set; }
    }
}