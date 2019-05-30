using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Logger;
using Testflow.Usr;

namespace Testflow.LoggerTest
{
    [TestClass]
    public class RemoteLoggerTest
    {
        private RemoteLoggerSession _logger;

        [TestInitialize]
         public void Initialize()
        {
            _logger = new RemoteLoggerSession("TestInstance", "SessionName", 0, LogLevel.Warn);
        }

        [TestMethod]
        public void RemoteLogTest()
        {
            Exception exception;
            try
            {
                throw new ApplicationException("This is an exception.");
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.AreEqual(LogLevel.Warn, _logger.LogLevel);

            // 需要查看${TESTFLOW_HOME}/log/platform目录下的打印信息是否正确
            // 忽略
            _logger.Print(LogLevel.Debug, CommonConst.PlatformLogSession, "This is Debug message.");
            
            // 记录
            _logger.Print(LogLevel.Info, CommonConst.PlatformLogSession, "This is Info message.");
            // 记录
            _logger.Print(LogLevel.Warn, CommonConst.PlatformLogSession, "This is Warn message.");
            // 记录
            _logger.Print(LogLevel.Error, CommonConst.PlatformLogSession, "This is Error message.");
            // 记录
            _logger.Print(LogLevel.Error, CommonConst.PlatformLogSession, exception, "This is debug exception message");
            // 记录
            _logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, "This is Fatal message.");
        }
    }
}