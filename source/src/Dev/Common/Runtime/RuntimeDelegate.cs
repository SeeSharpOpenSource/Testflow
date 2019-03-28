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
        /// 测试生成过程中的委托
        /// </summary>
        /// <param name="generationInfo">测试生成状态信息</param>
        public delegate void TestGenerationAction(ITestGenerationInfo generationInfo);

        /// <summary>
        /// 状态数据获取后的委托
        /// </summary>
        /// <param name="statusInfo">获取的运行时状态信息</param>
        public delegate void StatusReceivedAction(IRuntimeStatusInfo statusInfo);

        /// <summary>
        /// 测试序列组执行结束后的委托
        /// </summary>
        /// <param name="statistics">执行结束后的统计信息</param>
        public delegate void TestSessionOverAction(ITestResultCollection statistics);

        /// <summary>
        /// 断点命中后的委托
        /// </summary>
        public delegate void BreakPointHittedAction(IDebuggerHandle debuggerHandle, IDebugInformation information);

        /// <summary>
        /// 测试工程所有项目执行结束后的委托
        /// </summary>
        /// <param name="statistics">执行结束后的统计信息</param>
        public delegate void TestProjectOverAction(IList<ITestResultCollection> statistics);
    }
}