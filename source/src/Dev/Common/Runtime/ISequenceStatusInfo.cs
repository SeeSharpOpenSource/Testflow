using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 保存一个测试序列组运行时单个监视点的综合状态信息，提供给外部模块使用。
    /// </summary>
    public interface ISequenceStatusInfo : IPropertyExtendable
    {
        /// <summary>
        /// 正在执行的序列
        /// </summary>
        IList<ISequence> RunningSequence { get; }

        /// <summary>
        /// 已成功执行的序列
        /// </summary>
        IList<ISequence> SuccessSequence { get; }

        /// <summary>
        /// 已失败的序列
        /// </summary>
        IList<ISequence> FailedSequence { get; }

        /// <summary>
        /// 待执行状态的序列
        /// </summary>
        IList<ISequence> IdleSequence { get; }
    }
}