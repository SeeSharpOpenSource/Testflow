using System;
using System.Runtime.Serialization;
using System.Text;
using Testflow.CoreCommon.Common;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.CoreCommon.Data
{
    public class SequenceFailedInfo : ISequenceFailedInfo, ISerializable
    {
        private const string Delim = "$#_#$";

        public static string GetFailedStr(Exception exception, FailedType failedType)
        {
            StringBuilder failedStr = new StringBuilder(400);
            return failedStr.Append(failedType).Append(Delim).Append(exception.Message).Append(Delim)
                .Append(exception.Source).Append(Delim).Append(exception.StackTrace).Append(Delim)
                .Append(exception.GetType().Name).ToString();
        }

        public FailedType Type { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionType { get; set; }

        public SequenceFailedInfo(string failedInfo, FailedType type)
        {
            Type = type;
            this.Message = failedInfo;
            this.Source = string.Empty;
            this.StackTrace = string.Empty;
            this.ExceptionType = string.Empty;
        }

        public SequenceFailedInfo(Exception exception, FailedType failedType)
        {
            this.Message = exception.Message;
            this.Type = failedType;
            this.Source = exception.Source;
            this.StackTrace = exception.StackTrace;
            Type exceptionType = typeof(Exception);
            this.ExceptionType = $"{exceptionType.Namespace}.{exceptionType.Name}";
        }
        
        public SequenceFailedInfo(string failedStr)
        {
            string[] failedInfoDelim = failedStr.Split(Delim.ToCharArray());
            Type = (FailedType)Enum.Parse(typeof(FailedType), failedInfoDelim[0]);
            this.Message = failedInfoDelim[1];
            this.Source = failedInfoDelim[2];
            this.StackTrace = failedInfoDelim[3];
            this.ExceptionType = failedInfoDelim[4];
        }

        public override string ToString()
        {
            StringBuilder failedStr = new StringBuilder(400);
            return failedStr.Append(Type).Append(Delim).Append(Message).Append(Delim).Append(Source)
                .Append(Delim).Append(StackTrace).Append(Delim).Append(ExceptionType).ToString();
        }

        public SequenceFailedInfo(SerializationInfo info, StreamingContext context)
        {
            this.Type = (FailedType) info.GetValue("Type", typeof (FailedType));
            this.Message = (string)info.GetValue("Message", typeof(string));
            this.ExceptionType = (string)info.GetValue("ExceptionType", typeof(string));
            this.StackTrace = (string)info.GetValue("StackTrace", typeof(string));
            this.Source = (string)info.GetValue("Source", typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type", Type);
            info.AddValue("Message", Message);
            info.AddValue("ExceptionType", ExceptionType);
            info.AddValue("StackTrace", StackTrace);
            info.AddValue("Source", Source);
        }
    }
}