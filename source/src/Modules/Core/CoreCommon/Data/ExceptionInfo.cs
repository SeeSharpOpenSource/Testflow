using System;
using System.Runtime.Serialization;

namespace Testflow.CoreCommon.Data
{
    public class ExceptionInfo : ISerializable
    {
        public string Message { get; }
        public string ExceptionType { get; }
        public string StackTrace { get; }
        public string Source { get; }

        public ExceptionInfo(Exception exception)
        {
            this.Message = exception.Message;
            Type exceptionType = exception.GetType();
            this.ExceptionType = $"{exceptionType.Namespace}.{exceptionType.Name}";
            this.StackTrace = exception.StackTrace;
            this.Source = exception.Source;
        }

        public ExceptionInfo(SerializationInfo info, StreamingContext context)
        {
            this.Message = (string) info.GetValue("Message", typeof (string));
            this.ExceptionType = (string)info.GetValue("ExceptionType", typeof(string));
            this.StackTrace = (string)info.GetValue("StackTrace", typeof(string));
            this.Source = (string)info.GetValue("Source", typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Message", Message);
            info.AddValue("ExceptionType", ExceptionType);
            info.AddValue("StackTrace", StackTrace);
            info.AddValue("Source", Source);
        }

        public override string ToString()
        {
            string newLine = Environment.NewLine;
            return
                $"ExceptionType:{ExceptionType}{newLine}Source:{Source}{newLine}Message:{Message}{newLine}StackTrace:{StackTrace}";
        }
    }
}