using Testflow.Usr;

namespace Testflow.Modules
{
    /// <summary>
    /// 参数配置
    /// </summary>
    public interface IModuleConfigData : IPropertyExtendable
    {
         /// <summary>
         /// 配置模型的版本号
         /// </summary>
        string Version { get; set; }

        /// <summary>
        /// 配置对象的名称，等于对应模块接口的类名
        /// </summary>
        string Name { get; set; }
    }
}