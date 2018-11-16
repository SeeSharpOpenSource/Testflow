using System;
using Testflow.Data.Sequence;

namespace Testflow.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntityComponent : IDisposable
    {
        /// <summary>
        /// 初始化组件
        /// </summary>
        void Initialize();
    }
}