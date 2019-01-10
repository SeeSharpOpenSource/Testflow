using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Common;
using Testflow.Modules;
using Testflow.Runtime;

namespace Testflow.Logger
{
    /// <summary>
    /// 日志服务
    /// </summary>
    public class LogService : ILogService
    {
        private readonly Dictionary<int, ILogSession> _logStreams;

        public LogService()
        {
            _logStreams = new Dictionary<int, ILogSession>(Constants.DefaultLogStreamSize);
        }

        public IModuleConfigData ConfigData { get; set; }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            throw new NotImplementedException();
        }

        public LogLevel LogLevel { get; set; }
        public void Print(LogLevel logLevel, int sequenceIndex, string message)
        {
            throw new NotImplementedException();
        }

        public void Print(LogLevel logLevel, int sequenceIndex, Exception exception)
        {
            throw new NotImplementedException();
        }

        public LogLevel RuntimeLogLevel { get; set; }
        public ILogSession GetLogSession(IRuntimeSession session)
        {
            throw new NotImplementedException();
        }

        public void Print(LogLevel logLevel, int sessionId, int sequenceIndex, string message)
        {
            throw new NotImplementedException();
        }

        public void Print(LogLevel logLevel, int sessionId, int sequenceIndex, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void DestroyLogStream(int sessionId)
        {
            throw new NotImplementedException();
        }

        public void DestroyLogStream()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}