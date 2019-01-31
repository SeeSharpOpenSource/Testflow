using System;
using log4net;
using Testflow.Common;
using Testflow.Log;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.MessageUtil;

namespace Testflow.Logger
{
    /// <summary>
    /// 日志会话
    /// </summary>
    public class LocalLogSession : ILogSession, IMessageConsumer, IDisposable
    {
        private readonly ILog _logger;

        /// <summary>
        /// 创建日志会话实例
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        public LocalLogSession(int sessionId)
        {
            SessionId = sessionId;
            SessionId = sessionId;
            _logger = LogManager.GetLogger(Constants.PlatformLogName);
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
            if (null != logMessage.Ex)
            {
                Print((LogLevel) logMessage.Level, logMessage.SeqId, logMessage.Ex, logMessage.Msg);
            }
            else
            {
                Print((LogLevel)logMessage.Level, logMessage.SeqId, logMessage.Msg);
            }
        }

        public void Print(LogLevel logLevel, int sequenceIndex, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    _logger.Debug(message);
                    break;
                case LogLevel.Info:
                    _logger.Info(message);
                    break;
                case LogLevel.Warn:
                    _logger.Warn(message);
                    break;
                case LogLevel.Error:
                    _logger.Error(message);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message);
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18NName);
                    throw new TestflowRuntimeException(TestflowErrorCode.InvalidLogArgument, i18N.GetStr("InvalidLogArgument"));
                    break;
            }
        }

        public void Print(LogLevel logLevel, int sequenceIndex, Exception exception, string message = "")
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    _logger.Debug(message, exception);
                    break;
                case LogLevel.Info:
                    _logger.Info(message, exception);
                    break;
                case LogLevel.Warn:
                    _logger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    _logger.Error(message, exception);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message, exception);
                    break;                             
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18NName);
                    throw new TestflowRuntimeException(TestflowErrorCode.InvalidLogArgument, i18N.GetStr("InvalidLogArgument"));
                    break;
            }
        }

        public void Dispose()
        {
            // ignore
        }
    }
}