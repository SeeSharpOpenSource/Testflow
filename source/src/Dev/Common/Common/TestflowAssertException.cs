using System;

namespace Testflow.Common
{
    /// <summary>
    /// Testflow的断言异常
    /// </summary>
    [Serializable]
    public class TestflowAssertException : TestflowException
    {
        /// <summary>
        /// 创建TestflowAssertException的实例
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">异常信息</param>
        public TestflowAssertException(int errorCode, string message) : base(errorCode, message)
        {
        }

        /// <summary>
        /// 创建TestflowAssertException的实例
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">异常信息</param>
        /// <param name="innerException">内部异常</param>
        public TestflowAssertException(int errorCode, string message, Exception innerException) : base(errorCode, message, innerException)
        {
        }
    }
}