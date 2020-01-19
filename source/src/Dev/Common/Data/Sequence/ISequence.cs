using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Testflow.Data.Expression;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 保存单个测试序列信息的数据结构
    /// </summary>
    public interface ISequence: ISequenceFlowContainer
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

        /// <summary>
        /// 运行时行为，正常运行/跳过/强制失败/强制成功
        /// </summary>
        RunBehavior Behavior { get; set; }
    }
}