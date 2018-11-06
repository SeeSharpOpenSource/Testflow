using System;

namespace Testflow.Common
{
    /// <summary>
    /// Testflow运行时异常
    /// </summary>
    [Serializable]
    public class TestflowRuntimeException : TestflowException
    {
        public TestflowRuntimeException(int errorCode, string message) : base(errorCode, message)
        {
        }

        public TestflowRuntimeException(int errorCode, string message, Exception innerException) : base(errorCode, message, innerException)
        {
        }
    }
}