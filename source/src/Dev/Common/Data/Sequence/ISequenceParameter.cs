using System.Collections.Generic;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 每个Sequence的参数配置
    /// </summary>
    public interface ISequenceParameter : ISequenceDataContainer
    {
        /// <summary>
        /// 该参数Sequence在当前SequenceGroup的索引号
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 所有Step的Parameters
        /// </summary>
        IList<ISequenceStepParameter> StepParameters { get; set; }

        /// <summary>
        /// 序列内的变量值
        /// </summary>
        IList<IVariableInitValue> VariableValues { get; set; }
    }
    
}