using System.Collections.Generic;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 序列单个步骤的参数配置
    /// </summary>
    public interface ISequenceStepParameter : ISequenceDataContainer
    {
        /// <summary>
        /// 在当前层级的索引
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 所有子Step在当前Sequence的参数，如果没有子Step则为空
        /// </summary>
        IList<ISequenceStepParameter> SubStepParameters { get; set; }

        /// <summary>
        /// 参数配置，如果该Step包含子步骤则该项为空
        /// </summary>
        IParameterDataCollection Parameters { get; set; }

        /// <summary>
        /// 方法所在类的实例，静态方法时为空
        /// </summary>
        string Instance { get; set; }

        /// <summary>
        /// 保存返回值的变量名
        /// </summary>
        string Return { get; set; }
    }
}