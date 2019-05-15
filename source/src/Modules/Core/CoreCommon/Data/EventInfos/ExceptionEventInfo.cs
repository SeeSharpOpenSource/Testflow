using System;
using Testflow.Usr;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class ExceptionEventInfo : EventInfoBase
    {
        public string Message { get; }
        public string ExceptionType { get; }
        public string StackTrace { get; }
        public string Source { get; }

        public ExceptionEventInfo(Exception ex) : base(CommonConst.PlatformSession, EventType.Exception, DateTime.Now)
        {
            this.Message = ex.Message;
            this.ExceptionType = $"{ex.GetType().Namespace}.{ex.GetType().Name}";
            this.StackTrace = ex.StackTrace;
            this.Source = ex.Source;
        }

        public ExceptionEventInfo(RuntimeErrorMessage message) : base(message.Id, EventType.Exception, message.Time)
        {
            this.Message = message.Message;
            this.ExceptionType = message.Name;
            this.StackTrace = message.StackTrace;
            this.Source = message.Source;
        }
    }
}