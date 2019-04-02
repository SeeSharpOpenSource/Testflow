using Testflow.Common;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 序列测试生成信息
    /// </summary>
    public interface ISessionGenerationInfo : ICloneableClass<ISessionGenerationInfo>
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