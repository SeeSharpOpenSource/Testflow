using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Testflow.CoreCommon.Common;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.CoreCommon.Data
{
    [Serializable]
    public class FailedInfo : IFailedInfo, ISerializable
    {
        private const string Delim = "$#_#$";
        public static string GetFailedStr(Exception exception, FailedType failedType, string errorCodeProperty)
        {
            StringBuilder failedStr = new StringBuilder(400);
            string stackTrace = exception.StackTrace;
            if (!string.IsNullOrWhiteSpace(stackTrace) && stackTrace.Contains("'"))
            {
                stackTrace = stackTrace.Replace("'", "*");
            }
            return failedStr.Append(failedType).Append(Delim).Append(exception.Message).Append(Delim)
                .Append(exception.Source).Append(Delim).Append(stackTrace).Append(Delim)
                .Append(exception.GetType().Name).Append(Delim).Append(exception.HResult).ToString();
        }

        public static string GetFailedStr(string message, FailedType failedType)
        {
            StringBuilder failedStr = new StringBuilder(400);
            return failedStr.Append(failedType).Append(Delim).Append(message).Append(Delim)
                .Append(string.Empty).Append(Delim).Append(string.Empty).Append(Delim)
                .Append(string.Empty).Append(Delim).Append(0).ToString();
        }

        public FailedType Type { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionType { get; set; }
        public int ErrorCode { get; set; }

        public FailedInfo(string failedInfo, FailedType type)
        {
            Type = type;
            this.Message = failedInfo;
            this.Source = string.Empty;
            this.StackTrace = string.Empty;
            this.ExceptionType = string.Empty;
            this.ErrorCode = 0;
        }

        public FailedInfo(Exception exception, FailedType failedType)
        {
            this.Message = exception.Message;
            this.Type = failedType;
            this.Source = exception.Source;
            string stackTrace = exception.StackTrace;
            if (!string.IsNullOrWhiteSpace(stackTrace) && stackTrace.Contains("'"))
            {
                stackTrace = stackTrace.Replace("'", "*");
            }
            this.StackTrace = stackTrace;
            Type exceptionType = typeof(Exception);
            this.ExceptionType = $"{exceptionType.Namespace}.{exceptionType.Name}";
            this.ErrorCode = exception.HResult;
        }
        
        public FailedInfo(string failedStr)
        {
            const string delimPattern = @"\$#_#\$";
            string[] failedInfoElems = Regex.Split(failedStr, delimPattern, RegexOptions.IgnoreCase);
            int index = 0;
            Type = (FailedType)Enum.Parse(typeof(FailedType), failedInfoElems[index]);
            index++;
            this.Message = failedInfoElems[index];
            index++;
            this.Source = failedInfoElems[index];
            index++;
            string stackTrace = failedInfoElems[index];
            if (!string.IsNullOrWhiteSpace(stackTrace) && stackTrace.Contains("'"))
            {
                stackTrace = stackTrace.Replace("'", "*");
            }
            this.StackTrace = stackTrace;
            index++;
            this.ExceptionType = failedInfoElems[index];
            index++;
            // 为了兼容原来不存在ErrorCode字段时的场景
            int errorCode;
            this.ErrorCode = index < failedInfoElems.Length && int.TryParse(failedInfoElems[index], out errorCode)
                ? errorCode
                : 0;
        }

        public override string ToString()
        {
            StringBuilder failedStr = new StringBuilder(1000);
            return failedStr.Append(Type).Append(Delim).Append(Message).Append(Delim).Append(Source)
                .Append(Delim).Append(StackTrace).Append(Delim).Append(ExceptionType).Append(Delim)
                .Append(ErrorCode).ToString();
        }

        public FailedInfo(SerializationInfo info, StreamingContext context)
        {
            this.Type = (FailedType) info.GetValue("Type", typeof (FailedType));
            this.Message = (string)info.GetValue("Message", typeof(string));
            this.ExceptionType = (string)info.GetValue("ExceptionType", typeof(string));
            this.StackTrace = (string)info.GetValue("StackTrace", typeof(string));
            this.Source = (string)info.GetValue("Source", typeof(string));
            this.ErrorCode = info.GetInt32("ErrorCode");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", Type);
            info.AddValue("Message", Message);
            info.AddValue("ExceptionType", ExceptionType);
            info.AddValue("StackTrace", StackTrace);
            info.AddValue("Source", Source);
            info.AddValue("ErrorCode", ErrorCode);
        }
    }
}