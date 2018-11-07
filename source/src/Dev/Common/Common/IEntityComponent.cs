using System;

namespace Testflow.Common
{
    public interface IEntityComponent : IDisposable
    {
        /// <summary>
        /// 初始化组件
        /// </summary>
        void Initialize();
    }
}