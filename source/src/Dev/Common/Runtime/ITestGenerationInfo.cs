using System.Collections.Generic;
using Testflow.Common;

namespace Testflow.Runtime
{
    /// <summary>
    /// 测试生成信息
    /// </summary>
    public interface ITestGenerationInfo : ICloneableClass<ITestGenerationInfo>
    {
        /// <summary>
        /// 测试生成状态信息
        /// </summary>
        IList<ISequenceGenerationInfo> GenerationInfos { get; }

        /// <summary>
        /// TestProject根节点的生成信息
        /// </summary>
        ISequenceGenerationInfo RootGenerationInfo { get; }
    }
}