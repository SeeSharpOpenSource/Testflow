using System;
using Testflow.Common;
using Testflow.Modules;
using Testflow.Utility.I18nUtil;

namespace Testflow.EngineCore.Common
{
    internal static class ModuleUtility
    {
        public static void LogAndRaiseDataException(LogLevel level, string logInfo, int errorCode,
            Exception innerException, string message, params string[] param)
        {
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;

            string exMessage = (null == param || 0 == param.Length)
                ? i18N.GetStr(message)
                : i18N.GetFStr(message, param);

            if (null == innerException)
            {
                logService.Print(level, CommonConst.PlatformLogSession, logInfo);
                throw new TestflowDataException(errorCode, exMessage);
            }
            else
            {
                logService.Print(level, CommonConst.PlatformLogSession, innerException, logInfo);
                throw new TestflowDataException(errorCode, exMessage, innerException);
            }
        }

        public static void LogAndRaiseInternalException(LogLevel level, string logInfo, int errorCode,
            Exception innerException, string message, params string[] param)
        {
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;

            string exMessage = (null == param || 0 == param.Length)
                ? i18N.GetStr(message)
                : i18N.GetFStr(message, param);

            if (null == innerException)
            {
                logService.Print(level, CommonConst.PlatformLogSession, logInfo);
                throw new TestflowInternalException(errorCode, exMessage);
            }
            else
            {
                logService.Print(level, CommonConst.PlatformLogSession, innerException, logInfo);
                throw new TestflowInternalException(errorCode, exMessage, innerException);
            }
        }

        public static void LogAndRaiseRuntimeException(LogLevel level, string logInfo, int errorCode,
            Exception innerException, string message, params string[] param)
        {
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            ILogService logService = TestflowRunner.GetInstance().LogService;

            string exMessage = (null == param || 0 == param.Length)
                ? i18N.GetStr(message)
                : i18N.GetFStr(message, param);

            if (null == innerException)
            {
                logService.Print(level, CommonConst.PlatformLogSession, logInfo);
                throw new TestflowRuntimeException(errorCode, exMessage);
            }
            else
            {
                logService.Print(level, CommonConst.PlatformLogSession, innerException, logInfo);
                throw new TestflowRuntimeException(errorCode, exMessage, innerException);
            }
        }
    }
}