using System;
using Testflow.Data.Sequence;

namespace Testflow.Usr
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