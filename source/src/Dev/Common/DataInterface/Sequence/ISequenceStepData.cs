using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 保存测试序列中单个步骤的数据结构
    /// </summary>
    public interface ISequenceStepData : ICloneable
    {
        /// <summary>
        /// 保存子步骤，如果不包含则为空或null
        /// </summary>
        IList<ISequenceStepData> SubSteps { get; set; }

        /// <summary>
        /// 步骤在当前序列的索引
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 当前步骤的Function，如果该步骤包含子步骤，该参数为null
        /// </summary>
        IFunctionData Function { get; set; }

        /// <summary>
        /// 是否包含子步骤
        /// </summary>
        [XmlIgnore]
        bool HasSubSteps { get; }
        
        /// <summary>
        /// 循环计数器
        /// </summary>
        ICounter LoopCounter { get; set; }

        /// <summary>
        /// 重试计数器
        /// </summary>
        IRetryCounter RetryCounter { get; set; }
    }
}