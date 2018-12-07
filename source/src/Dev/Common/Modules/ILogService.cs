using System;
using Testflow.Common;
using Testflow.Runtime;

namespace Testflow.Modules
{
    /// <summary>
    /// 日志服务模块
    /// </summary>
    public interface ILogService : IController
    {
        /// <summary>
        /// 当前的日志级别
        /// </summary>
        LogLevel RecordLevel { get; set; }

        /// <summary>
        /// 初始化日志会话
        /// </summary>
        /// <param name="sessionId">日志会话ID</param>
        void InitLogSession(int sessionId);

        /// <summary>
        /// 以指定级别打印日志
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="sessionId">运行时上下文的ID</param>
        /// <param name="sequenceIndex">序列索引</param>
        /// <param name="message">信息</param>
        void Print(LogLevel logLevel, int sessionId, int sequenceIndex, string message);

        /// <summary>
        /// 以指定级别打印异常信息
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="sessionId">运行时上下文的ID</param>
        /// <param name="sequenceIndex">序列索引</param>
        /// <param name="exception">待打印的异常</param>
        void Print(LogLevel logLevel, int sessionId, int sequenceIndex, Exception exception);

        /// <summary>
        /// 销毁日志会话
        /// </summary>
        /// <param name="sessionId">日志会话ID</param>
        void DestroyLogSession(int sessionId);
    }
}