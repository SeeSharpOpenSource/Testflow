using System;
using Testflow.Common;
using Testflow.Modules;

namespace Testflow.Logger
{
    public class LogSession : ILogSession
    {
        private readonly ILogAppender _logAppender;

        public LogSession(int sessionId)
        {
            SessionId = sessionId;


        }

        public int SessionId { get; }

        public void Print(LogLevel logLevel, int sequenceIndex, string message)
        {
            throw new NotImplementedException();
        }

        public void Print(LogLevel logLevel, int sequenceIndex, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}