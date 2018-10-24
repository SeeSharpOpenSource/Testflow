using System;

namespace Testflow
{
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