using System.Xml.Serialization;
using Testflow.Common;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// Testflow的计数器
    /// </summary>
    public interface ILoopCounter : ICloneableClass<ILoopCounter>
    {
        /// <summary>
        /// 计数器迭代变量名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 计数器的最大计数值
        /// </summary>
        int MaxValue { get; set; }

        /// <summary>
        /// Counter是否被使用
        /// </summary>
        bool CounterEnabled { get; set; }

        /// <summary>
        /// 循环变量，记录当前的计数值
        /// </summary>
        string CounterVariable { get; set; }
    }
}
