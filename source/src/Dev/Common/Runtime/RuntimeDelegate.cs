using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public class RuntimeDelegate
    {
        /// <summary>
        /// 状态数据获取后的委托
        /// </summary>
        /// <param name="statusInfo">获取的运行时状态信息</param>
        /// <param name="stack">调用堆栈</param>
        public delegate void StatusReceivedAction(IRuntimeStatusInfo statusInfo, ICallStack stack);

        /// <summary>
        /// 测试序列组执行结束后的委托
        /// </summary>
        /// <param name="statistics">执行结束后的统计信息</param>
        /// <param name="sequenceGroup">执行结束的sequenceGroup对象</param>
        public delegate void TestSessionOverAction(ITestResultCollection statistics, ISequenceGroup sequenceGroup);

        /// <summary>
        /// 断点命中后的委托
        /// </summary>
        public delegate void BreakPointHittedAction(ISequenceDebugger debugger, IDebugInformation information);

        /// <summary>
        /// 测试工程所有项目执行结束后的委托
        /// </summary>
        /// <param name="statistics">执行结束后的统计信息</param>
        public delegate void TestProjectOverAction(IList<ITestResultCollection> statistics);
    }
}