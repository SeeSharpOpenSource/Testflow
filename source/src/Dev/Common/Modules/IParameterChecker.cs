using System.Collections.Generic;
using Testflow.Data.Description;
using Testflow.Data.Sequence;

namespace Testflow.Modules
{
    /// <summary>
    /// 参数检查模块
    /// </summary>
    public interface IParameterChecker : IController
    {
        /// <summary>
        /// 校验TestProject内模块的参数配置正确性
        /// </summary>
        /// <param name="components">设计时加载的组件映射集合</param>
        /// <param name="testProject">待校验的序列工程</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckParameters(IDictionary<string, IComInterfaceDescription> components, ITestProject testProject);

        /// <summary>
        /// 校验SequenceGroup内模块的参数配置正确性
        /// </summary>
        /// <param name="components">设计时加载的组件映射集合</param>
        /// <param name="testProject">待校验的序列组</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckParameters(IDictionary<string, IComInterfaceDescription> components, ISequenceGroup testProject);
    }
}