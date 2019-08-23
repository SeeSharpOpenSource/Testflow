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
        /// <param name="overwriteType">检测到null类型的变量时，是否覆盖</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckParameters(ITestProject testProject, bool overwriteType);

        /// <summary>
        /// 校验SequenceGroup内模块的参数配置正确性
        /// </summary>
        /// <param name="sequenceGroup">待校验的序列组</param>
        /// /// <param name="overwriteType">检测到null类型的变量时，是否覆盖</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup, bool overwriteType);

        /// <summary>
        /// 校验Sequence模块的参数配置正确性
        /// </summary>
        /// <param name="sequenceGroup">待校验的序列组</param>
        /// <param name="sequence">待校验的序列</param>
        /// /// <param name="overwriteType">检测到null类型的变量时，是否覆盖</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckParameters(ISequenceGroup sequenceGroup, ISequence sequence, bool overwriteType);

        /// <summary>
        /// 检查一行参数
        /// </summary>
        /// <param name="function">参数所在的function</param>
        /// <param name="index">参数index</param>
        /// <param name="arr">变量可能存在于这些ISequenceFlowContainer</param>
        /// /// <param name="overwriteType">检测到null类型的变量时，是否覆盖</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IWarningInfo CheckParameterData(IFunctionData function, int index, ISequenceFlowContainer[] arr, bool overwriteType);

        /// <summary>
        /// 校验Step模块的参数配置正确性
        /// </summary>
        /// <param name="Step">待校验的步骤</param>
        /// <param name="arr">变量可能存在于这些ISequenceFlowContainer</param>
        /// /// <param name="overwriteType">检测到null类型的变量时，是否覆盖</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IList<IWarningInfo> CheckStep(ISequenceStep Step, ISequenceFlowContainer[] arr, bool overwriteType);

        /// <summary>
        /// 校验某个Step的Return的参数配置正确性
        /// </summary>
        /// <param name="Step">所在Step</param>
        /// <param name="arr">变量可能存在于这些ISequenceFlowContainer</param>
        /// /// <param name="overwriteType">检测到null类型的变量时，是否覆盖</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IWarningInfo CheckReturn(ISequenceStep Step, ISequenceFlowContainer[] arr, bool overwriteType);

        /// <summary>
        /// 校验某个Step的Instance的参数配置正确性
        /// </summary>
        /// <param name="Step">所在Step</param>
        /// <param name="arr">变量可能存在于这些ISequenceFlowContainer</param>
        /// /// <param name="overwriteType">检测到null类型的变量时，是否覆盖</param>
        /// <returns>检查过程中出现的告警信息</returns>
        IWarningInfo CheckInstance(ISequenceStep Step, ISequenceFlowContainer[] arr, bool overwriteType);


        /// <summary>
        /// 校验某个变量和属性构成的字符串对应的类型是否和待校验类型一致
        /// </summary>
        /// <param name="parent">该次所在的SequenceGroup或TestProject</param>
        /// <param name="variableString">变量的字符串，样式类似于varname.property1.property2</param>
        /// <param name="checkType">待检查是否匹配的类型</param>
        /// /// <param name="overwriteType">检测到null类型的变量时，是否覆盖</param>
        /// <returns>如果错误返回错误信息，如果正确返回true</returns>
        IWarningInfo CheckPropertyType(ISequenceFlowContainer parent, string variableString, ITypeData checkType, bool overwriteType);

        /// <summary>
        /// 校验VariableType.Value的某个变量值是否与类型相符
        /// </summary>
        /// <param name="parent">改变量所在TestProject或SequenceGroup或Sequence</param>
        /// <param name="name">变量名字</param>
        /// <returns></returns>
        IWarningInfo CheckVariableValue(ISequenceFlowContainer parent, string name);
    }
}