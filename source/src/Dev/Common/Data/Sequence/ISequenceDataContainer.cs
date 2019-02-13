using System;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 序列数据的父接口
    /// </summary>
    public interface ISequenceDataContainer : ISequenceElement
    {
        /// <summary>
        /// 克隆一个序列数据
        /// </summary>
        ISequenceDataContainer Clone();
    }
}