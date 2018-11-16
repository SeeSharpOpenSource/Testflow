using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 单个序列的断点信息
    /// </summary>
    public interface IBreakPointsInfo
    {
        /// <summary>
        /// 待调试的序列
        /// </summary>
        ISequence DebugSequence { get; set; }

        /// <summary>
        /// 当前序列的所有断点信息
        /// </summary>
        IList<ISequenceStep> BreakPoints { get; set; }
    }
}