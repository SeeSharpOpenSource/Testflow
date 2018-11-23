using System;

namespace Testflow.Modules
{
    /// <summary>
    /// 控制接口基类
    /// </summary>
    public interface IController : IDisposable
    {
        /// <summary>
        /// 每个模块的配置数据
        /// </summary>
        IModuleConfigData ConfigData { get; set; }

        /// <summary>
        /// 初始化控制组件
        /// </summary>
        void Initialize();

        /// <summary>
        /// 适用该模块的参数配置
        /// </summary>
        /// <param name="configData">配置数据</param>
        void ApplyConfig(IModuleConfigData configData);
    }
}