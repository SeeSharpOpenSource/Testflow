using System;
using System.Runtime.Serialization;

namespace Testflow.Usr
{
    /// <summary>
    /// Testflow运行时异常
    /// </summary>
    [Serializable]
    public class TestflowRuntimeException : TestflowException
    {
        /// <summary>
        /// 创建TestflowRuntimeException的实例
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误信息</param>
        public TestflowRuntimeException(int errorCode, string message) : base(errorCode, message)
        {
        }

        /// <summary>
        /// 创建TestflowRuntimeException的实例
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">内部异常</param>
        public TestflowRuntimeException(int errorCode, string message, Exception innerException) : base(errorCode, message, innerException)
        {
        }

        /// <summary>
        /// 由外部代码创建的运行时异常
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">内部异常</param>
        public TestflowRuntimeException(string message, Exception innerException) : base(CommonErrorCode.ExternalError, message, innerException)
        {
        }

        /// <summary>
        /// 由外部代码创建的运行时异常
        /// </summary>
        /// <param name="message">错误信息</param>
        public TestflowRuntimeException(string message) : base(CommonErrorCode.ExternalError, message)
        {
        }

        public TestflowRuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}