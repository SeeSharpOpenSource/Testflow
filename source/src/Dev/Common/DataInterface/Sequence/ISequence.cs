using System;
using System.Collections.Generic;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 保存单个测试序列信息的数据结构
    /// </summary>
    public interface ISequence: ISequenceFlowContainer, ICloneable
    {
        /// <summary>
        /// 序列名称
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// 测试序列在当前序列组的索引
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 测试序列描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 序列的变量集合
        /// </summary>
        IVariableCollection Variables { get; set; }

        /// <summary>
        /// 测试的步骤集合
        /// </summary>
        ISequenceStepCollection Steps { get; set; }
    }
}