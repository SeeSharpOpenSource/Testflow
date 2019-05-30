using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using Testflow.Usr;
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
        private PlatformLogSession _platformLogSession;
        private readonly I18N _i18N;
        private Messenger _messenger;
        private readonly TestflowContext _context;
        private readonly TestflowRunner _testflowInst;

        private static LogService _inst = null;
        private static readonly object _instLock = new object();

        /// <summary>
        /// 创建日志服务实例
        /// </summary>
        public LogService()
        {
            if (null != _inst)
            {
                I18N i18N = I18N.GetInstance(Constants.I18NName);
                throw new TestflowRuntimeException(CommonErrorCode.InternalError, i18N.GetStr("InstAlreadyExist"));
            }
            lock (_instLock)
            {
                Thread.MemoryBarrier();
                if (null != _inst)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18NName);
                    throw new TestflowRuntimeException(CommonErrorCode.InternalError, i18N.GetStr("InstAlreadyExist"));
                }
                I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "i18n_logger_zh", "i18n_logger_en")
                {
                    Name = Constants.I18NName
                };
                _i18N = I18N.GetInstance(i18NOption);
                _testflowInst = TestflowRunner.GetInstance();
                _context = _testflowInst.Context;
                _inst = this;
            }
        }

        public IModuleConfigData ConfigData { get; set; }

        public void RuntimeInitialize()
        {
            if (null == _platformLogSession)
            {
                this._platformLogSession = new PlatformLogSession(_messenger);
            }
            MessengerOption option = new MessengerOption(Constants.LogQueueName, typeof(LogMessage));
            _messenger = Messenger.GetMessenger(option);
        }

        public void DesigntimeInitialize()
        {
            _messenger?.Dispose();
            if (null == _platformLogSession)
            {
                this._platformLogSession = new PlatformLogSession(_messenger);
            }
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            // TODO to implement
        }

        public LogLevel LogLevel { get; set; }

        public void Print(LogLevel logLevel, int sessionId, string message)
        {
            _platformLogSession.Print(logLevel, sessionId, message);
        }

        public void Print(LogLevel logLevel, int sessionId, Exception exception, string message)
        {
            _platformLogSession.Print(logLevel, sessionId, exception, message);
        }

        public LogLevel RuntimeLogLevel { get; set; }

        public void Print(LogLevel logLevel, int sessionId, int sequenceIndex, string message)
        {
            _platformLogSession.Print(logLevel, Constants.DesigntimeSessionId, message);
        }

        public void Print(LogLevel logLevel, int sessionId, int sequenceIndex, Exception exception, string message = "")
        {
            _platformLogSession.Print(logLevel, Constants.DesigntimeSessionId, exception, message);
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
            _platformLogSession?.Dispose();
            if (null != _messenger)
            {
                _messenger.Clear();
                Messenger.DestroyMessenger(Constants.LogQueueName);
            }
            LogManager.Shutdown();
        }
    }
}