using System;

namespace Testflow.Common
{
    /// <summary>
    /// Testflow的数据异常
    /// </summary>
    [Serializable]
    public class TestflowDataException : TestflowException
    {
        public TestflowDataException(int errorCode, string message) : base(errorCode, message)
        {
        }

        public TestflowDataException(int errorCode, string message, Exception innerException) : base(errorCode, message, innerException)
        {
        }
    }
}