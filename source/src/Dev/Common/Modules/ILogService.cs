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
        /// 以指定级别打印日志
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="context">运行时上下文</param>
        /// <param name="sequenceIndex">序列索引</param>
        /// <param name="message">信息</param>
        void Print(LogLevel logLevel, IRuntimeContext context, int sequenceIndex, string message);

        /// <summary>
        /// 以指定级别打印异常信息
        /// </summary>
        /// <param name="logLevel">日志级别</param>
        /// <param name="context">运行时上下文</param>
        /// <param name="sequenceIndex">序列索引</param>
        /// <param name="exception">待打印的异常</param>
        void Print(LogLevel logLevel, IRuntimeContext context, int sequenceIndex, Exception exception);
    }
}