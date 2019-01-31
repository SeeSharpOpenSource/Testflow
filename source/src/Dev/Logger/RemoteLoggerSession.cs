using System;
using Testflow.Common;
using Testflow.Log;
using Testflow.Utility.MessageUtil;

namespace Testflow.Logger
{
    /// <summary>
    /// 远程日志会话
    /// </summary>
    public class RemoteLoggerSession : ILogSession
    {
        private static RemoteLoggerSession _inst;
        private static object _instLock = new object();
        private readonly Messenger _messenger;
        private readonly LogLevel _logLevel;
        private FormatterType _formatter;
        private readonly Type[] _targetType;

        private RemoteLoggerSession(int sessionId, LogLevel logLevel, FormatterType formatter)
        {
            _logLevel = logLevel;
            this.SessionId = sessionId;
            _formatter = formatter;
            this._targetType = new Type[] {typeof(LogMessage)};
            MessengerOption messengerOption = new MessengerOption(Constants.LogQueueName);
            _messenger = Messenger.GetMessenger(messengerOption);
        }

        /// <summary>
        /// 获取RemobeLoggerSession的实例
        /// </summary>
        public static RemoteLoggerSession GetInstance(int sessionId, LogLevel logLevel, FormatterType formatter)
        {
            if (null != _inst && _inst.SessionId == sessionId)
            {
                return _inst;
            }
            lock (_instLock)
            {
                if (null != _inst && sessionId == _inst.SessionId)
                {
                    return _inst;
                }
                if (null == _inst)
                {
                    _inst = new RemoteLoggerSession(sessionId, logLevel, formatter);
                    return _inst;
                }
            }
            return null;
        }

        public int SessionId { get; }

        void ILogSession.Print(LogLevel logLevel, int sequenceIndex, string message)
        {
            if (logLevel < this._logLevel)
            {
                return;
            }
            LogMessage logMessage = new LogMessage(SessionId, sequenceIndex, logLevel, message);
            _messenger.Send(logMessage, _formatter, _targetType);
        }

        void ILogSession.Print(LogLevel logLevel, int sequenceIndex, Exception exception, string message = "")
        {
            if (logLevel < this._logLevel)
            {
                return;
            }
            LogMessage logMessage = new LogMessage(SessionId, sequenceIndex, logLevel, exception);
            _messenger.Send(logMessage, _formatter, _targetType);
        }
    }
}