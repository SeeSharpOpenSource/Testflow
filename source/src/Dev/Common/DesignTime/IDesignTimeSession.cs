using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;

namespace Testflow.DesignTime
{
    /// <summary>
    /// 设计时控制类
    /// </summary>
    public interface IDesignTimeSession : IEntityComponent
    {
        /// <summary>
        /// 当前设计时的会话ID
        /// </summary> 
        long SessionId { get; set; }

        /// <summary>
        /// 当前设计时的上下文信息
        /// </summary>
        IDesigntimeContext Context { get; }

        /// <summary>
        /// 获取匹配当前类型的所有变量列表
        /// </summary>
        /// <param name="type">需要匹配的类型数据</param>
        /// <returns></returns>
        IVariableCollection GetFittedVariables(ITypeData type);

        #region Sequence　Edit
        
        /// <summary>
        /// 为测试序列组添加参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数的类型信息</param>
        /// <returns>添加完成的Argument</returns>
        IArgument AddArgument(string name, ITypeData type);

        /// <summary>
        /// 删除测试序列组的参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>被删除的参数</returns>
        IArgument RemoveArgument(string name);

        /// <summary>
        /// 添加某个序列到测试序列组
        /// </summary>
        /// <param name="sequence">待添加的测试序列</param>
        /// <param name="index">被插入的位置</param>
        /// <returns>被添加的Sequence</returns>
        ISequence AddSequence(ISequence sequence, int index);

        /// <summary>
        /// 添加某个序列到测试序列组
        /// </summary>
        /// <param name="sequenceName">测试序列名称</param>
        /// <param name="description">测试序列的描述信息</param>
        /// <param name="index">被插入的位置</param>
        /// <returns>被添加的Sequence</returns>
        ISequence AddSequence(string sequenceName, string description, int index);

        /// <summary>
        /// 添加测试步骤到上级节点
        /// </summary>
        /// <param name="parent">上级节点</param>
        /// <param name="stepData">待添加的测试步骤</param>
        /// <param name="index">待插入的位置</param>
        /// <returns>添加完成后的ISequenceStep</returns>
        ISequenceStep AddSequenceStep(ISequenceFlowContainer parent, ISequenceStep stepData, int index);

        /// <summary>
        /// 添加多个测试步骤到上级节点
        /// </summary>
        /// <param name="parent">上级节点</param>
        /// <param name="stepDatas">待添加的测试步骤</param>
        /// <param name="index">待插入的位置</param>
        /// <returns>添加完成后的ISequenceStep</returns>
        ISequenceStep AddSequenceStep(ISequenceFlowContainer parent, IList<ISequenceStep> stepDatas, int index);

        /// <summary>
        /// 添加空白的Step到上级节点，该节点可以添加子Step序列
        /// </summary>
        /// <param name="parent">上级节点</param>
        /// <param name="description">Step的描述信息</param>
        /// <param name="index">待插入的位置</param>
        /// <returns>添加完成后的ISequenceStep</returns>
        ISequenceStep AddSequenceStep(ISequenceFlowContainer parent, string description, int index);

        /// <summary>
        /// 添加测试步骤到上级节点
        /// </summary>
        /// <param name="parent">上级节点</param>
        /// <param name="functionData">待添加的方法</param>
        /// <param name="description">当前个步骤的描述信息</param>
        /// <param name="index">待插入的位置</param>
        /// <returns>添加完成后的ISequenceStep</returns>
        ISequenceStep AddSequenceStep(ISequenceFlowContainer parent, IFunctionData functionData, string description, int index);

        /// <summary>
        /// 添加变量声明
        /// </summary>
        /// <param name="parent">添加变量的上级节点</param>
        /// <param name="variableName">变量名称</param>
        /// <param name="value">变量的值</param>
        /// <param name="index">待插入的位置</param>
        /// <returns>添加完成后的Variable</returns>
        IVariable AddVariable(ISequenceFlowContainer parent, string variableName, string value, int index);

        /// <summary>
        /// 添加变量声明
        /// </summary>
        /// <param name="parent">添加变量的上级节点</param>
        /// <param name="variable">变量实例</param>
        /// <param name="index">待插入的位置</param>
        /// <returns>添加完成后的Variable</returns>
        IVariable AddVariable(ISequenceFlowContainer parent, IVariable variable, int index);

        /// <summary>
        /// 删除变量声明
        /// </summary>
        /// <param name="parent">删除变量的上级节点</param>
        /// <param name="variable">变量实例</param>
        /// <returns>被删除的变量</returns>
        IVariable RemoveVariable(ISequenceFlowContainer parent, IVariable variable);

        /// <summary>
        /// 删除变量声明
        /// </summary>
        /// <param name="parent">删除变量的上级节点</param>
        /// <param name="variableName">变量名称</param>
        /// <returns>被删除的变量</returns>
        IVariable RemoveVariable(ISequenceFlowContainer parent, string variableName);

        /// <summary>
        /// 配置变量值
        /// </summary>
        /// <param name="variable">待配置的变量</param>
        /// <param name="value">配置的变量值</param>
        void SetVariableValue(IVariable variable, string value);

        /// <summary>
        /// 配置变量值
        /// </summary>
        /// <param name="variableName">变量名称</param>
        /// <param name="value">配置的变量值</param>
        void SetVariableValue(string variableName, string value);

        /// <summary>
        /// 配置参数值
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="sequenceIndex">序列的索引</param>
        /// <param name="indexes">当前序列所在的Step的索引序列，如果是多层step，分别为从上层到下层的索引</param>
        void SetParameterValue(string parameterName, string value, int sequenceIndex, params int[] indexes);

        /// <summary>
        /// 配置参数值
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="sequence">该参数所在序列的Step</param>
        void SetParameterValue(string parameterName, string value, ISequenceStep sequence);

        /// <summary>
        /// 配置Step的实例变量
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="sequenceIndex">序列的索引</param>
        /// <param name="indexes">当前序列所在的Step的索引序列，如果是多层step，分别为从上层到下层的索引</param>
        void SetInstance(string variableName, int sequenceIndex, params int[] indexes);

        /// <summary>
        /// 配置Step的实例变量
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="sequence">该参数所在序列的Step</param>
        void SetInstance(string variableName, ISequenceStep sequence);

        /// <summary>
        /// 配置Step的返回变量
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="sequenceIndex">序列的索引</param>
        /// <param name="indexes">当前序列所在的Step的索引序列，如果是多层step，分别为从上层到下层的索引</param>
        void SetReturn(string variableName, int sequenceIndex, params int[] indexes);

        /// <summary>
        /// 配置Step的Return变量
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="sequence">该参数所在序列的Step</param>
        void SetReturn(string variableName, ISequenceStep sequence);

        /// <summary>
        /// 为指定序列步骤添加循环计数器
        /// </summary>
        /// <param name="sequenceStep">待添加循环计数器的序列步骤</param>
        /// <param name="maxCount">最大计数</param>
        /// <returns>添加的循环计数器</returns>
        ILoopCounter AddLoopCounter(ISequenceStep sequenceStep, int maxCount);

        /// <summary>
        /// 为指定序列步骤添加循环计数器
        /// </summary>
        /// <param name="sequenceStep">待添加循环计数器的序列步骤</param>
        /// <param name="maxCount">最大计数</param>
        /// <returns>添加的重试计数器</returns>
        IRetryCounter AddRetryCounter(ISequenceStep sequenceStep, int maxCount);

        /// <summary>
        /// 修改Counter的计数
        /// </summary>
        /// <param name="counter">待修改的Counter</param>
        /// <param name="maxCount">最大计数值</param>
        /// <param name="enabled">是否使能</param>
        void ModifyCounter(ILoopCounter counter, int maxCount, bool enabled);

        /// <summary>
        /// 修改Counter的计数
        /// </summary>
        /// <param name="counter">待修改的Counter</param>
        /// <param name="maxCount">最大计数值</param>
        /// <param name="enabled">是否使能</param>
        void ModifyCounter(IRetryCounter counter, int maxCount, bool enabled);

        /// <summary>
        /// 删除指定序列步骤的循环计数器
        /// </summary>
        /// <param name="sequenceStep">待删除循环计数器的序列步骤</param>
        /// <returns>被删除的循环计数器</returns>
        ILoopCounter RemoveLoopCounter(ISequenceStep sequenceStep);

        /// <summary>
        /// 删除指定序列步骤的循重试计数器
        /// </summary>
        /// <param name="sequenceStep">待删除重试计数器的序列步骤</param>
        /// <returns>被删除的重试计数器</returns>
        IRetryCounter RemoveRetryCounter(ISequenceStep sequenceStep);
        
        /// <summary>
        /// 根据索引号
        /// </summary>
        /// <param name="sequenceIndex">Sequence的索引号</param>
        /// <param name="stepIndex">Step的索引号</param>
        /// <returns></returns>
        ISequenceStep GetSequenceStep(int sequenceIndex, params int[] stepIndex);
        
        #endregion
        
    }
}