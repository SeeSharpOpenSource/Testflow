namespace Testflow.Runtime
{
    public class RuntimeDelegate
    {
        /// <summary>
        /// 状态数据获取后的委托
        /// </summary>
        /// <param name="statusInfo">获取的运行时状态信息</param>
        public delegate void StatusReceivedAction(IRuntimeStatusInfo statusInfo, ICallStack stack);

        /// <summary>
        /// 执行结束后的委托
        /// </summary>
        /// <param name="statistics">执行结束后的统计信息</param>
        public delegate void TestSessionOverAction(ITestResultCollection statistics, ICallStack stack);
    }
}