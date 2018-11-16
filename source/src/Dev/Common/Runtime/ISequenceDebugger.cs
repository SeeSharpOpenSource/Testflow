using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 单个测试序列组的调试会话
    /// </summary>
    public interface ISequenceDebugger
    {
        

        /// <summary>
        /// 未命中的断点步骤
        /// </summary>
        IList<ISequenceStep> UnreachedBreakPoints { get; }
        
        /// <summary>
        /// 当前断点序列步骤
        /// </summary>
        ISequenceStep CurrentStep { get; }

        
    }
}