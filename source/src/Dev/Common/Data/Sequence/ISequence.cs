using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 保存单个测试序列信息的数据结构
    /// </summary>
    public interface ISequence: ISequenceFlowContainer, ICloneable
    {
        /// <summary>
        /// 测试序列在当前序列组的索引。Setup的Index为0，Teardown的Index为最后一个
        /// </summary>
        int Index { get; set; }

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