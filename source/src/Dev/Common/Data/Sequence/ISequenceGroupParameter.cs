using System;
using System.Collections.Generic;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 测试序列组参数配置
    /// </summary>
    public interface ISequenceGroupParameter : ISequenceDataContainer
    {
        /// <summary>
        /// 参数类的名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 参数类的描述信息
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 序列参数信息
        /// </summary>
        ISequenceParameterInfo Info { get; set; }

        /// <summary>
        /// 序列组内的变量值
        /// </summary>
        IList<IVariableInitValue> VariableValues { get; set; }

            /// <summary>
        /// Setup模块对应的参数配置
        /// </summary>
        ISequenceParameter SetUpParameters { get; set; }

        /// <summary>
        /// 所有序列对应的参数
        /// </summary>
        IList<ISequenceParameter> SequenceParameters { get; set; }

        /// <summary>
        /// Setup模块对应的参数配置
        /// </summary>
        ISequenceParameter TearDownParameters { get; set; }
        
    }
}