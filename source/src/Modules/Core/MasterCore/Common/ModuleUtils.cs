using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.Data.Sequence;
using Testflow.MasterCore.EventData;
using Testflow.MasterCore.StatusManage.StatePersistance;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Utility.I18nUtil;

namespace Testflow.MasterCore.Common
{
    internal static class ModuleUtils
    {

        public static TDataType GetDeleage<TDataType>(Delegate action) where TDataType : class
        {
            TDataType delegateAction = action as TDataType;
            if (null == delegateAction)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowInternalException(ModuleErrorCode.IncorrectDelegate,
                    i18N.GetFStr("IncorrectDelegate", action.GetType().Name));
            }
            return delegateAction;
        }

        public static TDataType GetParamValue<TDataType>(object[] eventParams, int index) where TDataType : class
        {
            TDataType paramValueObject = null;
            if (eventParams.Length > index && null == (paramValueObject = eventParams[index] as TDataType))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowInternalException(ModuleErrorCode.IncorrectParamType,
                    i18N.GetFStr("IncorrectParamType", typeof(TDataType).Name));
            }

            return paramValueObject;
        }

        public static int GetSessionId(ITestProject testProject, ISequenceGroup sequenceGroup)
        {
            if (null == testProject)
            {
                return 0;
            }
            return testProject.SequenceGroups.IndexOf(sequenceGroup);
        }

        public static void StopThreadWork(Thread thread)
        {
            if (null != thread && ThreadState.Running == thread.ThreadState && !thread.Join(Constants.OperationTimeout))
            {
                thread.Abort();
            }
        }

        public static string GetHashValue(string hashSource, Encoding encoding)
        {
            StringBuilder hashValue = new StringBuilder(256 * 2 / 8);
            char[] decimalStrs = new char[]
            {
                '0', '1', '2', '3', '4', '5', '6', '7',
                '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
            };

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] sourceBytes = encoding.GetBytes(hashSource);
                byte[] valueBytes = sha256.ComputeHash(sourceBytes);
                foreach (byte byteData in valueBytes)
                {
                    hashValue.Append(decimalStrs[byteData >> 4]).Append(decimalStrs[byteData & 0x0F]);
                }
            }
            return hashValue.ToString();
        }

        public static string GetRuntimeHash(Encoding encoding)
        {
            string hostName = Environment.MachineName;
            string systemVersion = Environment.OSVersion.VersionString;
            string mac = string.Empty;
            using (ManagementObjectSearcher query =
                new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration"))
            {
                ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementObject mo in queryCollection)
                {
                    if (mo["IPEnabled"].ToString() == "True")
                        mac = mo["MacAddress"].ToString();
                }
            }
            string runtimeInfo = $"{hostName}/{systemVersion}/{mac}/{DateTime.Now}";

            return GetHashValue(runtimeInfo, encoding);
        }

        public static string WatchDataToString(IDictionary<string, string> watchData)
        {
            if (null == watchData || watchData.Count == 0)
            {
                return string.Empty;
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                // TODO
            };
            return JsonConvert.SerializeObject(watchData, settings);
        }

        public static Dictionary<string, string> StrToWatchData(string watchDataStr)
        {
            if (string.IsNullOrWhiteSpace(watchDataStr))
            {
                return new Dictionary<string, string>(1);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                // TODO
            };
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(watchDataStr, settings);
        }

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