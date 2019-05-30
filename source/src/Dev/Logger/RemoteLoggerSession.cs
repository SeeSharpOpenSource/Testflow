using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using log4net.Appender;
using log4net.Core;
using Testflow.Usr;
using Testflow.Utility.MessageUtil;

namespace Testflow.Logger
{
    /// <summary>
    /// 远程日志会话
    /// </summary>
    public class RemoteLoggerSession : LogSession
    {
        //        private readonly Messenger _messenger;
        /// <summary>
        /// 创建远程日志会话
        /// </summary>
        /// <param name="instanceName">运行实例名称</param>
        /// <param name="sessionName">运行会话名称</param>
        /// <param name="sessionId">会话ID</param>
        /// <param name="logLevel">日志级别</param>
        /// <exception cref="TestflowRuntimeException"></exception>
        public RemoteLoggerSession(string instanceName, string sessionName, int sessionId, LogLevel logLevel) : base(sessionId, Constants.SlaveLogName)
        {
            
            //            Type[] _targetType = new Type[] {typeof(LogMessage)};
            //            MessengerOption messengerOption = new MessengerOption(Constants.LogQueueName);
            //            _messenger = Messenger.GetMessenger(messengerOption);
            string testflowHome = Environment.GetEnvironmentVariable(CommonConst.EnvironmentVariable);
            if (string.IsNullOrWhiteSpace(testflowHome))
            {
                testflowHome = "..";
            }
            char dirSeparator = Path.DirectorySeparatorChar;
            string logPath = GetSlaveLogPath(instanceName, sessionName, testflowHome);
            string configFilePath = $"{testflowHome}{dirSeparator}{Constants.SlaveConfFile}";

            try
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(configFilePath));
                Repository = LogManager.GetRepository();
                IAppender[] appenders = Repository.GetAppenders();
                RollingFileAppender appender = appenders.First(item => item.Name.Equals(Constants.SlaveAppender)) as RollingFileAppender;
                string originalLogFile = appender.File;

                appender.File = logPath;
                appender.ActivateOptions();
                if (File.Exists(originalLogFile))
                {
                    File.Delete(originalLogFile);
                }
                Logger = LogManager.GetLogger(Constants.PlatformLogName);
                LogLevel = logLevel;
            }
            catch (LogException ex)
            {
                TestflowRuntimeException exception = new TestflowRuntimeException(ModuleErrorCode.LogQueueInitFailed,
                    ex.Message, ex);
                throw exception;
            }
        }

        private string GetSlaveLogPath(string instanceName, string sessionName, string testflowHome)
        {
            char dirSeparator = Path.DirectorySeparatorChar;
            string logFileName = string.Format(Constants.SlaveLogNameFormat, SessionId, sessionName);
            StringBuilder logPath = new StringBuilder(200);
            logPath.Append(testflowHome).Append(dirSeparator).Append(Constants.SlaveLogDir).Append(dirSeparator)
                .Append(instanceName).Append(dirSeparator).Append(logFileName).Append(Constants.LogFilePostfix);
            // 如果不存在该文件说明原来没有执行过当前instance，直接返回默认路径
            if (!File.Exists(logPath.ToString()))
            {
                return logPath.ToString();
            }
            int removeLength = 1 + logFileName.Length + Constants.LogFilePostfix.Length;
            logPath.Remove(logPath.Length - removeLength, removeLength);
            int index = 1;
            string newInstanceDir = instanceName;
            do
            {
                logPath.Remove(logPath.Length - newInstanceDir.Length, newInstanceDir.Length);
                newInstanceDir = $"{instanceName}_{index++}";
                logPath.Append(newInstanceDir);
            } while (Directory.Exists(logPath.ToString()));
            return logPath.Append(dirSeparator).Append(logFileName).Append(Constants.LogFilePostfix).ToString();
        }
    }
}