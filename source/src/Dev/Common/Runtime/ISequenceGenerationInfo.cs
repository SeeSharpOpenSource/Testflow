using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 序列测试生成信息
    /// </summary>
    public interface ISequenceGenerationInfo : ICloneableClass<ISequenceGenerationInfo>
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        int Session { get; }

        /// <summary>
        /// 测试生成状态信息
        /// </summary>
        ISerializableMap<int, GenerationStatus> SequenceStatus { get; }

        /// <summary>
        /// Sequence整体的生成状态
        /// </summary>
        GenerationStatus Status { get; set; }
    }
}