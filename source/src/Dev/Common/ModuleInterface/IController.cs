using System;

namespace Testflow.ModuleInterface
{
    /// <summary>
    /// 控制接口基类
    /// </summary>
    public interface IController : IDisposable
    {
        /// <summary>
        /// 初始化控制组件
        /// </summary>
        void Initialize();
    }
}