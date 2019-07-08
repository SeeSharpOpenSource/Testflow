using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Logger;
using Testflow.Usr;

namespace Testflow.LoggerTest
{
    [TestClass]
    public class LogServiceTest
    {
        private LogService _logService;

        [TestInitialize]
        public void Initialize()
        {
            _logService = new LogService();
            _logService.RuntimeInitialize();
        }

        [TestMethod]
        public void TestLogServiceWrite()
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

            Assert.AreEqual(LogLevel.Debug, _logService.LogLevel);

            // 需要查看${TESTFLOW_HOME}/log/platform目录下的打印信息是否正确
            // 忽略
            _logService.Print(LogLevel.Debug, CommonConst.PlatformLogSession, "This is Debug message.");
            // 忽略
            _logService.Print(LogLevel.Debug, CommonConst.PlatformLogSession, exception, "This is debug exception message");
            // 记录
            _logService.Print(LogLevel.Info, CommonConst.PlatformLogSession, "This is Info message.");
            // 记录
            _logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, "This is Warn message.");
            // 记录
            _logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, "This is Error message.");
            // 记录
            _logService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, "This is Fatal message.");

            _logService.LogLevel = LogLevel.Debug;

            // 记录
            _logService.Print(LogLevel.Debug, CommonConst.PlatformLogSession, "This is Debug message.");
            // 记录
            _logService.Print(LogLevel.Debug, CommonConst.PlatformLogSession, exception,
                "This is debug exception message");
            // 记录
            _logService.Print(LogLevel.Info, CommonConst.PlatformLogSession, "This is Info message.");
            // 记录
            _logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, "This is Warn message.");
            // 记录
            _logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, "This is Error message.");
            // 记录
            _logService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, "This is Fatal message.");
        }
    }
}