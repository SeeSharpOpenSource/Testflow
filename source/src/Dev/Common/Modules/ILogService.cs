using System;
using Testflow.Usr;
using Testflow.Runtime;

namespace Testflow.Modules
{
    /// <summary>
    /// 日志服务模块
    /// </summary>
    public interface ILogService : IController
    {
        #region 平台日志接口

        /// <summary>
        /// 平台日志级别
        /// </summary>
        LogLevel LogLevel { get; set; }

        /// <summary>
        /// 以指定级别打印日志
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="sessionId">序列索引</param>
        /// <param name="message">信息</param>
        void Print(LogLevel logLevel, int sessionId, string message);

        /// <summary>
        /// 以指定级别打印异常信息
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="sessionId">序列索引</param>
        /// <param name="exception">待打印的异常</param>
        /// <param name="message">日志信息</param>
        void Print(LogLevel logLevel, int sessionId, Exception exception, string message = "");

        #endregion

        /// <summary>
        /// 运行时的日志级别
        /// </summary>
        LogLevel RuntimeLogLevel { get; set; }

        /// <summary>
        /// 在框架中以指定级别向某个运行时会话的日志流打印日志
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="sessionId">运行时上下文的ID</param>
        /// <param name="sequenceIndex">序列索引</param>
        /// <param name="message">信息</param>
        void Print(LogLevel logLevel, int sessionId, int sequenceIndex, string message);

        /// <summary>
        /// 在框架中以指定级别向某个运行时会话的日志流打印日志
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="sessionId">运行时上下文的ID</param>
        /// <param name="sequenceIndex">序列索引</param>
        /// <param name="exception">待打印的异常</param>
        /// <param name="message"></param>
        void Print(LogLevel logLevel, int sessionId, int sequenceIndex, Exception exception, string message = "");
    }
}