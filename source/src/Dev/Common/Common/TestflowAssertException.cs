using System;

namespace Testflow.Common
{
    /// <summary>
    /// Testflow的断言异常
    /// </summary>
    [Serializable]
    public class TestflowAssertException : TestflowException
    {
        public TestflowAssertException(int errorCode, string message) : base(errorCode, message)
        {
        }

        public TestflowAssertException(int errorCode, string message, Exception innerException) : base(errorCode, message, innerException)
        {
        }
    }
}