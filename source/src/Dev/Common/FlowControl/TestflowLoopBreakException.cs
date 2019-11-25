using Testflow.Usr;

namespace Testflow.FlowControl
{
    /// <summary>
    /// 跳出当前循环异常
    /// </summary>
    public class TestflowLoopBreakException : TestflowException
    {
        /// <summary>
        /// 是否停止循环：true，停止循环后续执行；false，停止当前循环，进入下个循环
        /// </summary>
        public bool BreakLoop { get; }

        /// <summary>
        /// 创建当前异常的实例
        /// </summary>
        /// <param name="breakLoop">是否停止循环：true，停止循环后续执行；false，停止当前循环，进入下个循环</param>
        /// <param name="breakTime">跳出循环次数，默认为1</param>
        public TestflowLoopBreakException(bool breakLoop, int breakTime = 1) : base(CommonErrorCode.FlowControl, "Flow Control")
        {
            this.BreakLoop = breakLoop;
        }
    }
}