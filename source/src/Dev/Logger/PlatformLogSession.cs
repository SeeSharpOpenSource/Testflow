using System;
using log4net;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;
using Testflow.Log;

namespace Testflow.Logger
{
    /// <summary>
    /// 内部日志会话
    /// </summary>
    internal class PlatformLogSession : ILogSession, IDisposable
    {
        private readonly ILog _logger;

        /// <summary>
        /// 内部日志会话
        /// </summary>
        /// <param name="sessionId"></param>
        public PlatformLogSession(int sessionId)
        {
            SessionId = sessionId;
            _logger = LogManager.GetLogger(Constants.PlatformLogName);

            // TODO to implement
        }

        public int SessionId { get; }

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
                    throw new TestflowRuntimeException(ModuleErrorCode.InvalidLogArgument, i18N.GetStr("InvalidLogArgument"));
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
                    throw new TestflowRuntimeException(ModuleErrorCode.InvalidLogArgument, i18N.GetStr("InvalidLogArgument"));
                    break;
            }
        }

        public void Dispose()
        {
            // ignore
        }
    }
}