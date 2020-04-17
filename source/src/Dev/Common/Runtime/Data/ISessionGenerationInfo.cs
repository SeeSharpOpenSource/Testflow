using Testflow.Usr;

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

        /// <summary>
        /// 发生生成错误的Step的堆栈
        /// </summary>
        ICallStack ErrorStack { get; set; }

        /// <summary>
        /// 生成错误的错误信息
        /// </summary>
        string ErrorInfo { get; set; }
    }
}