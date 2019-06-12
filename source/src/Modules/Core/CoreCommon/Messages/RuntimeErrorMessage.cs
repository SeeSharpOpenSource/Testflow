using System;
using System.Runtime.Serialization;
using Testflow.CoreCommon.Common;

namespace Testflow.CoreCommon.Messages
{
    [Serializable]
    public class RuntimeErrorMessage : MessageBase
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string Source { get; set; }

        public RuntimeErrorMessage(int id, Exception exception) : 
            base($"{exception.GetType().Namespace}.{exception.GetType().Name}", id, MessageType.RuntimeError)
        {
            this.Message = exception.Message;
            this.StackTrace = exception.StackTrace;
            this.Source = exception.Source;
        }

        public RuntimeErrorMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Message = (string) info.GetValue("Message", typeof(string));
            this.StackTrace = (string) info.GetValue("StackTrace", typeof(string));
            this.Source = (string) info.GetValue("Source", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Message", Message);
            info.AddValue("StackTrace", StackTrace);
            info.AddValue("Source", Source);
        }
    }
}
