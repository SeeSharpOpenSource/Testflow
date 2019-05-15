using Testflow.Usr;

namespace Testflow.Runtime
{
    /// <summary>
    /// 运行器配置接口
    /// </summary>
    public interface IRuntimeConfiguration : IPropertyExtendable
    {
        /// <summary>
        /// 运行时类型，运行/调试
        /// </summary>
        RuntimeType Type { get; set; }

        /// <summary>
        /// 状态更新周期，单位为ms
        /// </summary>
        int StatusTransCycle { get; set; }
    }
}