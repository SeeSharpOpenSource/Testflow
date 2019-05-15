using log4net.Appender;
using log4net.Core;

namespace Testflow.Logger.Appender
{
    /// <summary>
    /// 使用消息队列传输的Appender
    /// </summary>
    public class MsgQueueAppender : IAppender
    {
        public void Close()
        {
            throw new System.NotImplementedException();
        }

        public void DoAppend(LoggingEvent loggingEvent)
        {
        }

        public string Name { get; set; }
    }
}