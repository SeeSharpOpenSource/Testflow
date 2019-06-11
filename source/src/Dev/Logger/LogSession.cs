using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.Logger
{
    /// <summary>
    /// 日志会话
    /// </summary>
    public abstract class LogSession : IDisposable
    {
        protected ILog Logger;
        private string _loggerName;

        /// <summary>
        /// 日志会话
        /// </summary>
        public LogSession(int session, string loggerName)
        {
            SessionId = session;
            _loggerName = loggerName;
        }

        protected ILoggerRepository Repository;

        private LogLevel _logLevel;
        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set
            {
                log4net.Repository.Hierarchy.Logger rootLogger = (log4net.Repository.Hierarchy.Logger) Logger.Logger;
                switch (value)
                {
                    case LogLevel.Trace:
                        rootLogger.Level = Level.Trace;
                        break;
                    case LogLevel.Debug:
                        rootLogger.Level = Level.Debug;
                        break;
                    case LogLevel.Info:
                        rootLogger.Level = Level.Info;
                        break;
                    case LogLevel.Warn:
                        rootLogger.Level = Level.Warn;
                        break;
                    case LogLevel.Error:
                        rootLogger.Level = Level.Error;
                        break;
                    case LogLevel.Fatal:
                        rootLogger.Level = Level.Fatal;
                        break;
                    case LogLevel.Off:
                        rootLogger.Level = Level.Off;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                _logLevel = value;
                ((Hierarchy)LogManager.GetRepository()).RaiseConfigurationChanged(EventArgs.Empty);
            }
        }

        public int SessionId { get; }

        public virtual void Print(LogLevel logLevel, int sessionId, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Logger.Debug(message);
                    break;
                case LogLevel.Info:
                    Logger.Info(message);
                    break;
                case LogLevel.Warn:
                    Logger.Warn(message);
                    break;
                case LogLevel.Error:
                    Logger.Error(message);
                    break;
                case LogLevel.Fatal:
                    Logger.Fatal(message);
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18NName);
                    throw new TestflowRuntimeException(ModuleErrorCode.InvalidLogArgument, i18N.GetStr("InvalidLogArgument"));
                    break;
            }
        }

        public virtual void Print(LogLevel logLevel, int sessionId, Exception exception, string message = "")
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Logger.Debug(message, exception);
                    break;
                case LogLevel.Info:
                    Logger.Info(message, exception);
                    break;
                case LogLevel.Warn:
                    Logger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    Logger.Error(message, exception);
                    break;
                case LogLevel.Fatal:
                    Logger.Fatal(message, exception);
                    break;
                default:
                    I18N i18N = I18N.GetInstance(Constants.I18NName);
                    throw new TestflowRuntimeException(ModuleErrorCode.InvalidLogArgument, i18N.GetStr("InvalidLogArgument"));
                    break;
            }
        }

        public virtual void Dispose()
        {
            LoggerManager.ShutdownRepository(_loggerName);
        }
    }
}