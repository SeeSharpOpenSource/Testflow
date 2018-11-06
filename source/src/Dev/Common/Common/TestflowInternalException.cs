using System;

namespace Testflow.Common
{
    /// <summary>
    /// Testflow内部异常
    /// </summary>
    [Serializable]
    public class TestflowInternalException : TestflowException
    {
        public TestflowInternalException(int errorCode, string message) : base(errorCode, message)
        {
        }

        public TestflowInternalException(int errorCode, string message, Exception innerException) : base(errorCode, message, innerException)
        {
        }
    }
}