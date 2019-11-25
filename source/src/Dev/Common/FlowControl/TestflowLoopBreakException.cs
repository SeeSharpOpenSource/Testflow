using System;
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

        private int _count;
        /// <summary>
        /// 循环计数，每跳出一层循环计数减1。计数为0时停止
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 创建当前异常的实例
        /// </summary>
        /// <param name="breakLoop">是否停止循环：true，停止循环后续执行；false，停止当前循环，进入下个循环</param>
        /// <param name="innerException">内部异常</param>
        /// <param name="breakTime">跳出循环次数，默认为1</param>
        public TestflowLoopBreakException(bool breakLoop, Exception innerException, int breakTime = 1) : 
            base(CommonErrorCode.FlowControl, "Flow Control", innerException)
        {
            this.BreakLoop = breakLoop;
            this._count = breakTime;
        }

        /// <summary>
        /// 减少一次Loop的计数
        /// </summary>
        public void CalcDown()
        {
            this._count--;
        }
    }
}