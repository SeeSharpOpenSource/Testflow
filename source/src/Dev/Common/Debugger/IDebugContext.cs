using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.Debug
{
    /// <summary>
    /// 调试上下文
    /// </summary>
    public interface IDebugContext
    {
        /// <summary>
        /// 被调试的TestProject
        /// </summary>
        ITestProject TestProject { get; }

        /// <summary>
        /// 所有被调试序列组的集合
        /// </summary>
        ISequenceGroup DebugSequenceGroup { get; }


        ISequence DebugSequence { get; }

        /// <summary>
        /// 所有被打断点的Step
        /// </summary>
        IList<ISequenceStep> BreakPoints { get; }
    }
}