using System;

namespace Testflow.Usr
{
    /// <summary>
    /// Testflow内部异常
    /// </summary>
    [Serializable]
    public class TestflowInternalException : TestflowException
    {
        /// <summary>
        /// 创建TestflowInternalException的实例
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误信息</param>
        public TestflowInternalException(int errorCode, string message) : base(errorCode, message)
        {
        }

        /// <summary>
        /// 创建TestflowInternalException的实例
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">内部异常</param>
        public TestflowInternalException(int errorCode, string message, Exception innerException) : base(errorCode, message, innerException)
        {
        }
    }
}