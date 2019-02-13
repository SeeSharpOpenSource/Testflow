using System;
using Testflow.Common;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 序列数据的父接口
    /// </summary>
    public interface ISequenceDataContainer : ISequenceElement, ICloneableClass<ISequenceDataContainer>
    {
    }
}