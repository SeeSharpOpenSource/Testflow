using System;

namespace Testflow.Common
{
    /// <summary>
    /// Testflow的异常基类
    /// </summary>
    [Serializable]
    public class TestflowException : ApplicationException
    {
        /// <summary>
        /// 异常码
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// 创建TestflowException的实例
        /// </summary>
        /// <param name="errorCode">异常码</param>
        /// <param name="message">错误信息</param>
        public TestflowException(int errorCode, string message) : base(message)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// 创建TestflowException的实例
        /// </summary>
        /// <param name="errorCode">异常码</param>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">内部异常</param>
        public TestflowException(int errorCode, string message, Exception innerException) : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }
    }
}