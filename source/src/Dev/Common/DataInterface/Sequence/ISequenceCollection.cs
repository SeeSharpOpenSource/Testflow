using System;
using System.Collections.Generic;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// IsequeceData的列表容器
    /// </summary>
    public interface ISequenceCollection : IList<ISequence>, ICloneable
    {
         
    }
}