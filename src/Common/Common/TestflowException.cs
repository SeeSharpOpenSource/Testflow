using System;

namespace Testflow
{
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