using Testflow.Common;
using Testflow.Data.Sequence;

namespace Testflow.Modules
{
    /// <summary>
    /// 序列持久化模块
    /// </summary>
    public interface ISequenceSerializer : IController
    {
        /// <summary>
        /// 序列化测试工程
        /// </summary>
        /// <param name="project">待序列化的工程</param>
        /// <param name="target">序列化的目标</param>
        /// <param name="param">额外参数</param>
        void Serialize(ITestProject project, SerializationTarget target, params string[] param);

        /// <summary>
        /// 序列化测试序列组
        /// </summary>
        /// <param name="sequenceGroup">待序列化的测试序列组</param>
        /// <param name="target">序列化的目标</param>
        /// <param name="param">额外参数</param>
        void Serialize(ISequenceGroup sequenceGroup, SerializationTarget target, params string[] param);

        /// <summary>
        /// 反序列化测试工程
        /// </summary>
        /// <param name="source">反序列化的源</param>
        /// <param name="param">额外参数</param>
        ITestProject Deserialize(SerializationTarget source, params string[] param);

        /// <summary>
        /// 反序列化测试序列组
        /// </summary>
        /// <param name="source">反序列化的源</param>
        /// <param name="param">额外参数</param>
        ISequenceGroup Serialize(SerializationTarget source, params string[] param);
    }
}