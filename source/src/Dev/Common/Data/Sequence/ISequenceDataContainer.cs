using System;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 序列容器容器的父接口
    /// </summary>
    public interface ISequenceDataContainer : ISequenceElement
    {
        /// <summary>
        /// 克隆一个复制数据
        /// </summary>
        ISequenceDataContainer Clone();
    }
}