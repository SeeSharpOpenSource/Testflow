using System.Collections.Generic;
using Testflow.DataInterface.Sequence;

namespace Testflow.Runtime
{
    public interface ICallStack
    {
        /// <summary>
        /// 被调用测试工程
        /// </summary>
        ITestProject TestProject { get; }

        /// <summary>
        /// 被调用测试组
        /// </summary>
        ISequenceGroup SequenceGroup { get; }

        /// <summary>
        /// 被调用测试序列
        /// </summary>
        ISequence Sequence { get; }

        /// <summary>
        /// 序列步骤调用堆栈
        /// </summary>
        Stack<ISequenceStep> StepStack { get; }
    }
}