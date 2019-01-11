using System;
using System.Collections.Generic;
using System.Linq;
using Testflow.Common;
using Testflow.Log;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Utility.I18nUtil;
using Testflow.Utility.MessageUtil;

namespace Testflow.Logger
{
    /// <summary>
    /// 日志服务
    /// </summary>
    public class LogService : ILogService
    {
        private readonly Dictionary<int, LogSession> _runtimeLogSessions;
        private LogSession _designtimeLogSession;
        private readonly I18N _i18N;
        private Messenger _messenger;

        /// <summary>
        /// 创建日志服务实例
        /// </summary>
        public LogService()
        {
            _runtimeLogSessions = new Dictionary<int, LogSession>(Constants.DefaultLogStreamSize);
            I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "Resources.i18n_logger_zh", "Resources.i18n_logger_en")
            {
                Name = Constants.I18NName
            };
            _i18N = I18N.GetInstance(i18NOption);
        }

        IModuleConfigData IController.ConfigData { get; set; }

        void IController.RuntimeInitialize()
        {
            MessengerOption option = new MessengerOption(Constants.LogQueueName, null);
            try
            {
                _messenger = Messenger.GetMessenger(option);
            }
            catch (TestflowRuntimeException ex)
            {
                if (ex.ErrorCode == TestflowErrorCode.MessengerRuntimeError)
                {
                    _messenger = Messenger.CreateMessenger(option);
                }
                else
                {
                    throw;
                }
            }
            _messenger.Initialize(_runtimeLogSessions.Values.ToArray());
            foreach (LogSession logSession in _runtimeLogSessions.Values)
            {
                logSession.Dispose();
            }
            _runtimeLogSessions.Clear();
        }

        void IController.DesigntimeInitialize()
        {
            _designtimeLogSession = new LogSession(Constants.DesigntimeSessionId);
        }

        void IController.ApplyConfig(IModuleConfigData configData)
        {
            throw new NotImplementedException();
        }

        LogLevel ILogService.LogLevel { get; set; }

        void ILogService.Print(LogLevel logLevel, int sequenceIndex, string message)
        {
            throw new NotImplementedException();
        }

        void ILogService.Print(LogLevel logLevel, int sequenceIndex, Exception exception)
        {
            throw new NotImplementedException();
        }

        LogLevel ILogService.RuntimeLogLevel { get; set; }

        ILogSession ILogService.GetLogSession(IRuntimeSession session)
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

        /// <summary>
        /// 停止运行时日志
        /// </summary>
        public void StopRuntimeLogging()
        {
            _messenger.Dispose();
            _messenger = null;
        }

        void IDisposable.Dispose()
        {
            _designtimeLogSession?.Dispose();
            foreach (LogSession logSession in _runtimeLogSessions.Values)
            {
                logSession.Dispose();
            }
            _runtimeLogSessions.Clear();
            Messenger.DestroyMessenger(Constants.LogQueueName);
        }
    }
}