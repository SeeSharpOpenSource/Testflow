using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 序列测试生成信息
    /// </summary>
    public interface ISequenceGenerationInfo
    {
        /// <summary>
        /// 测试生成状态信息
        /// </summary>
        IList<GenerationStatus> Status { get; }

        /// <summary>
        /// 事件对应的Sequence索引号
        /// </summary>
        IList<int> SequenceIndex { get; }
    }
}