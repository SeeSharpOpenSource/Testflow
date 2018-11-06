using System.Xml.Serialization;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 重试计数器
    /// </summary>
    public interface IRetryCounter
    {
        /// <summary>
        /// 计数器名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 计数器当前重试次数
        /// </summary>
        [XmlIgnore]
        int RetryTimes { get; }

        /// <summary>
        /// 计数器的最大重试次数
        /// </summary>
        int MaxRetryTimes { get; set; }

        /// <summary>
        /// 索引器增加计数，如果已经达到上限则抛出异常
        /// </summary>
        void Flip();

        /// <summary>
        /// Retry功能是否使能
        /// </summary>
        [XmlIgnore]
        bool RetryEnabled { get; }

        /// <summary>
        /// 清除计数器
        /// </summary>
        void Clear();
    }
}