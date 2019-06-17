using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 调用堆栈
    /// </summary>
    public interface ICallStack
    {
        /// <summary>
        /// 会话索引号
        /// </summary>
        int Session { get; }

        /// <summary>
        /// 被调用测试序列
        /// </summary>
        int Sequence { get; }

        /// <summary>
        /// 序列步骤调用堆栈
        /// </summary>
        IList<int> StepStack { get; }
    }
}