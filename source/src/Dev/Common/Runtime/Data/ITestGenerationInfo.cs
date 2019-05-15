using System.Collections.Generic;
using Testflow.Usr;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 测试生成信息
    /// </summary>
    public interface ITestGenerationInfo : ICloneableClass<ITestGenerationInfo>
    {
        /// <summary>
        /// 测试生成状态信息
        /// </summary>
        IList<ISessionGenerationInfo> GenerationInfos { get; }

        /// <summary>
        /// TestProject根节点的生成信息
        /// </summary>
        ISessionGenerationInfo RootGenerationInfo { get; }
    }
}