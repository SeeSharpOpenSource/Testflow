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
using Testflow.Utility.MessageUtil;

namespace Testflow.Logger
{
    /// <summary>
    /// 内部日志会话
    /// </summary>
    public class PlatformLogSession : LogSession
    {
        /// <summary>
        /// 内部日志会话
        /// </summary>
        public PlatformLogSession() : base(CommonConst.PlatformLogSession, Constants.PlatformLogName)
        {
            string testflowHome = Environment.GetEnvironmentVariable(CommonConst.EnvironmentVariable);
            if (string.IsNullOrWhiteSpace(testflowHome))
            {
                testflowHome = "..";
            }
            char dirSeparator = Path.DirectorySeparatorChar;
            string logPath = $"{testflowHome}{dirSeparator}{Constants.PlatformLogDir}";
            string configFilePath = $"{testflowHome}{dirSeparator}{Constants.PlatformConfFile}";

            try
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(configFilePath));
                Repository = LogManager.GetRepository();
                IAppender[] appenders = Repository.GetAppenders();
                RollingFileAppender appender = appenders.First(item => item.Name.Equals(Constants.RootAppender)) as RollingFileAppender;
                string originalLogFile = appender.File;

                appender.File = logPath;
                appender.ActivateOptions();
                if (File.Exists(originalLogFile))
                {
                    File.Delete(originalLogFile);
                }

                Logger = LogManager.GetLogger(Constants.PlatformLogName);
                SetOriginalLevel();
            }
            catch (LogException ex)
            {
                TestflowRuntimeException exception = new TestflowRuntimeException(ModuleErrorCode.LogQueueInitFailed,
                    ex.Message, ex);
                throw exception;
            }
        }

        private void SetOriginalLevel()
        {
            Level level = ((Hierarchy) Repository).Root.Level;
            if (level == Level.Trace)
            {
                this.LogLevel = LogLevel.Trace;
            }
            else if (level == Level.Debug)
            {
                this.LogLevel = LogLevel.Debug;
            }
            else if (level == Level.Warn)
            {
                this.LogLevel = LogLevel.Warn;
            }
            else if (level == Level.Info)
            {
                this.LogLevel = LogLevel.Info;
            }
            else if (level == Level.Error)
            {
                this.LogLevel = LogLevel.Error;
            }
            else if (level == Level.Fatal)
            {
                this.LogLevel = LogLevel.Fatal;
            }
            else if (level == Level.Off)
            {
                this.LogLevel = LogLevel.Off;
            }
        }

        public override void Print(LogLevel logLevel, int sessionId, Exception exception, string message = "")
        {
//            if (sessionId != CommonConst.PlatformLogSession)
//            {
//                if (logLevel >= this.LogLevel)
//                {
//                    LogMessage logMessage = new LogMessage(sessionId, logLevel, message);
//                    _messenger.Send(logMessage, FormatterType.Xml);
//                }
//                return;
//            }
            base.Print(logLevel, sessionId, exception, message);
        }

        public override void Print(LogLevel logLevel, int sessionId, string message)
        {
//            if (sessionId != CommonConst.PlatformLogSession)
//            {
//                if (logLevel >= this.LogLevel)
//                {
//                    LogMessage logMessage = new LogMessage(sessionId, logLevel, message);
//                    _messenger.Send(logMessage, FormatterType.Xml);
//                }
//                return;
//            }
            base.Print(logLevel, sessionId, message);
        }

        public override void Dispose()
        {
            IAppender[] appenders = Repository.GetAppenders();
            RollingFileAppender appender = appenders.First(item => item.Name.Equals(Constants.RootAppender)) as RollingFileAppender;
            string logFile = (null != appender) ? appender.File : string.Empty;
            base.Dispose();
            if (!string.IsNullOrWhiteSpace(logFile) && File.Exists(logFile))
            {
                FileInfo fileInfo = new FileInfo(logFile);
                if (fileInfo.Length == 0)
                {
                    File.Delete(logFile);
                }
            }
        }
    }
}