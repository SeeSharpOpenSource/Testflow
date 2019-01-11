using System;
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

        void IMessageConsumer.Handle(IMessage message)
        {
            throw new NotImplementedException();
        }

        void ILogSession.Print(LogLevel logLevel, int sequenceIndex, string message)
        {
            throw new NotImplementedException();
        }

        void ILogSession.Print(LogLevel logLevel, int sequenceIndex, Exception exception)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}