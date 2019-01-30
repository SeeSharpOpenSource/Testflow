using System;
using log4net;
using log4net.Config;
using Testflow.Common;
using Testflow.Log;
using Testflow.Modules;
using Testflow.Utility.MessageUtil;

namespace Testflow.Logger
{
    /// <summary>
    /// 日志会话
    /// </summary>
    public class LogSession : ILogSession, IMessageConsumer, IDisposable
    {
        private readonly ILogAppender _logAppender;

        /// <summary>
        /// 创建日志会话实例
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        public LogSession(int sessionId)
        {
            SessionId = sessionId;
        }

        /// <summary>
        /// 会话ID
        /// </summary>
        public int SessionId { get; set; }

        public void Handle(IMessage message)
        {
            LogMessage logMessage = message as LogMessage;
            if (null == logMessage)
            {
                return;
            }
        }

        public void Print(LogLevel logLevel, int sequenceIndex, string message)
        {
            throw new NotImplementedException();
        }

        public void Print(LogLevel logLevel, int sequenceIndex, Exception exception, string message = "")
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}