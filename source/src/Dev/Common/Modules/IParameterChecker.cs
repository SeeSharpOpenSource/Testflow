using System.Collections.Generic;
using Testflow.Data;
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
        /// <param name="testProject">待校验的序列工程</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckParameters(ITestProject testProject);

        /// <summary>
        /// 校验SequenceGroup内模块的参数配置正确性
        /// </summary>
        /// <param name="sequenceGroup">待校验的序列组</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup);

        /// <summary>
        /// 校验Sequence模块的参数配置正确性
        /// </summary>
        /// <param name="sequenceGroup">待校验的序列组</param>
        /// <param name="sequence">待校验的序列</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup, ISequence sequence);

        /// <summary>
        /// 校验某个变量和属性构成的字符串对应的类型是否和待校验类型一致
        /// </summary>
        /// <param name="parent">该次所在的SequenceGroup或TestProject</param>
        /// <param name="variableString">变量的字符串，样式类似于varname.property1.property2</param>
        /// <param name="checkType">待检查是否匹配的类型</param>
        /// <returns>如果错误返回错误信息，如果正确返回true</returns>
        IWarningInfo CheckPropertyType(ISequenceFlowContainer parent, string variableString, ITypeData checkType);
    }
}