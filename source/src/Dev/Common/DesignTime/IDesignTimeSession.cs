using System.Collections.Generic;
using Testflow.Common;
using Testflow.DataInterface;
using Testflow.DataInterface.Sequence;

namespace Testflow.DesignTime
{
    /// <summary>
    /// 设计时控制类
    /// </summary>
    public interface IDesignTimeSession : IEntityComponent
    {
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

        /// <summary>
        /// 添加某个序列到测试序列组
        /// </summary>
        /// <param name="sequence">待添加的测试序列</param>
        /// <param name="index">被插入的位置</param>
        void AddSequence(ISequence sequence, int index);

        /// <summary>
        /// 添加某个序列到测试序列组
        /// </summary>
        /// <param name="sequenceName">测试序列名称</param>
        /// <param name="description">测试序列的描述信息</param>
        /// <param name="index">被插入的位置</param>
        void AddSequence(string sequenceName, string description, int index);

        /// <summary>
        /// 添加测试步骤到上级节点
        /// </summary>
        /// <param name="parent">上级节点</param>
        /// <param name="stepData">待添加的测试步骤</param>
        /// <param name="index">待插入的位置</param>
        void AddSequenceStep(ISequenceFlowContainer parent, ISequenceStep stepData, int index);

        /// <summary>
        /// 添加多个测试步骤到上级节点
        /// </summary>
        /// <param name="parent">上级节点</param>
        /// <param name="stepDatas">待添加的测试步骤</param>
        /// <param name="index">待插入的位置</param>
        void AddSequenceStep(ISequenceFlowContainer parent, IList<ISequenceStep> stepDatas, int index);

        /// <summary>
        /// 添加测试步骤到上级节点
        /// </summary>
        /// <param name="parent">上级节点</param>
        /// <param name="functionData">待添加的方法</param>
        /// <param name="description">当前个步骤的描述信息</param>
        /// <param name="index">待插入的位置</param>
        void AddSequenceStep(ISequenceFlowContainer parent, IFunctionData functionData, string description, int index);

        /// <summary>
        /// 添加变量声明
        /// </summary>
        /// <param name="parent">添加变量的上级节点</param>
        /// <param name="variableName">变量名称</param>
        /// <param name="value">变量的值</param>
        /// <param name="index">待插入的位置</param>
        void AddVariable(ISequenceFlowContainer parent, string variableName, string value, int index);

        /// <summary>
        /// 添加变量声明
        /// </summary>
        /// <param name="parent">添加变量的上级节点</param>
        /// <param name="variable">变量实例</param>
        /// <param name="index">待插入的位置</param>
        void AddVariable(ISequenceFlowContainer parent, IVariable variable, int index);

        /// <summary>
        /// 配置参数值
        /// </summary>
        /// <param name="variable">待配置的变量</param>
        /// <param name="value">配置的变量值</param>
        void SetArgumentValue(ISequenceStep variable, string value);

        /// <summary>
        /// 配置参数值
        /// </summary>
        /// <param name="variableName">变量名称</param>
        /// <param name="value">配置的变量值</param>
        void SetArgumentValue(string variableName, string value);

        /// <summary>
        /// 保存配置集到文件
        /// </summary>
        /// <param name="filePath"></param>
        void Save(string filePath);
    }
}