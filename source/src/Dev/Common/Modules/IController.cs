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
        /// 运行时初始化控制组件
        /// </summary>
        void RuntimeInitialize();

        /// <summary>
        /// 设计时初始化控制组件
        /// </summary>
        void DesigntimeInitialize();

        /// <summary>
        /// 适用该模块的参数配置
        /// </summary>
        /// <param name="configData">配置数据</param>
        void ApplyConfig(IModuleConfigData configData);
    }
}