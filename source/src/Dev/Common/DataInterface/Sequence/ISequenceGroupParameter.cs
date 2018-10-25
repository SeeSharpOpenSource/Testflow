using System;
using System.Collections.Generic;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISequenceGroupParameter : ICloneable
    {
        /// <summary>
        /// 序列参数信息
        /// </summary>
        ISequenceParameterInfo Info { get; set; }

        /// <summary>
        /// 所有序列对应的参数
        /// </summary>
        IList<ISequenceParameter> SequenceParameters { get; set; }

        /// <summary>
        /// 更新所有签名信息
        /// </summary>
        void RefreshSignature();
    }
}