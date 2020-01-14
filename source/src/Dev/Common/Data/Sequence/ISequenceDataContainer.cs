using System;
using Testflow.Usr;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 序列数据的父接口
    /// </summary>
    public interface ISequenceDataContainer : ISequenceElement, ICloneableClass<ISequenceDataContainer>
    {
        /// <summary>
        /// 使用所属序列对象初始化
        /// </summary>
        void Initialize(ISequenceFlowContainer parent);
    }
}