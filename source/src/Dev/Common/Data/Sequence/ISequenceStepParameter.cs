using System.Collections.Generic;

namespace Testflow.Data.Sequence
{
    public interface ISequenceStepParameter : ISequenceDataContainer
    {
        /// <summary>
        /// 在当前层级的索引
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 所有子Step在当前Sequence的参数，如果没有子Step则为null
        /// </summary>
        IList<ISequenceStepParameter> SubStepParameters { get; set; }

        /// <summary>
        /// 参数配置，如果该Step包含子步骤则该项为null
        /// </summary>
        IParameterDataCollection Parameters { get; set; }
    }
}