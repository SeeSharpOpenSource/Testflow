using System.Collections.Generic;

namespace Testflow.Runtime
{
    /// <summary>
    /// 测试生成信息
    /// </summary>
    public interface ITestGenerationInfo
    {
        /// <summary>
        /// 测试生成状态信息
        /// </summary>
        IDictionary<int, ISequenceGenerationInfo> GenerationInfos { get; }

        /// <summary>
        /// 事件对应的SequenceGroup索引号
        /// </summary>
        IList<int> SequenceGroupIndex { get; }
    }
}