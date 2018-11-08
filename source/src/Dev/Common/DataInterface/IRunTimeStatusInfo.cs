using Testflow.Common;
using Testflow.Runtime;

namespace Testflow.DataInterface
{
    /// <summary>
    /// 保存一个测试序列组运行时单个监视点的即时状态信息，运行引擎内部使用。
    /// </summary>
    public interface IRuntimeStatusInfo : IPropertyExtendable
    {
        /// <summary>
        /// 所在的运行时会话
        /// </summary>
        IRuntimeSession Session { get; }

    }
}