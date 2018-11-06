using System;

namespace Testflow.Common
{
    /// <summary>
    /// Testflow的异常基类
    /// </summary>
    [Serializable]
    public class TestflowException : ApplicationException
    {
        public int ErrorCode { get; }

        public TestflowException(int errorCode, string message) : base(message)
        {
            this.ErrorCode = errorCode;
        }

        public TestflowException(int errorCode, string message, Exception innerException) : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }
    }
}