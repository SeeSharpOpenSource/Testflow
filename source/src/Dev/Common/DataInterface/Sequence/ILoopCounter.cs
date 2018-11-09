using System.Xml.Serialization;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// Testflow的计数器
    /// </summary>
    public interface ILoopCounter
    {
        /// <summary>
        /// 计数器迭代变量名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 计数器当前计数值
        /// </summary>
        [XmlIgnore]
        int Value { get; }

        /// <summary>
        /// 计数器的最大计数值
        /// </summary>
        int MaxValue { get; set; }

        /// <summary>
        /// 索引器增加计数，如果达到上限则返回true，否则返回false
        /// </summary>
        bool Flip();

        /// <summary>
        /// Counter是否被使用
        /// </summary>
        [XmlIgnore]
        bool CounterEnabled { get; }

        /// <summary>
        /// 清除计数器
        /// </summary>
        void Clear();
    }
}
