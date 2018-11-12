using System;
using System.Collections.Generic;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// IsequeceData的列表容器
    /// </summary>
    public interface ISequenceCollection : IList<ISequence>, ICloneable
    {
         
    }
}