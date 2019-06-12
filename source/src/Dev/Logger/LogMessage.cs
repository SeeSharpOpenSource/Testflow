using System;
using System.Runtime.Serialization;
using Testflow.Usr;
using Testflow.Utility.MessageUtil;

namespace Testflow.Logger
{
    /// <summary>
    /// 日志消息
    /// </summary>
    public class LogMessage : IMessage
    {
        /// <summary>
        /// 创建空白的日志消息实例
        /// </summary>
        public LogMessage()
        {
            this.Id = -1;
            this.Msg = null;
            this.Ex = null;
            this.Level = (int) LogLevel.Trace;
            this.Time = DateTime.Now;
        }

        /// <summary>
        /// 使用消息初始化日志消息
        /// </summary>
        public LogMessage(int sessionId, LogLevel logLevel, string message)
        {
            this.Id = sessionId;
            this.Msg = message;
            this.Level = (int) logLevel;
            this.Ex = null;
            this.Time = DateTime.Now;
        }

        /// <summary>
        /// 使用异常初始化日志消息
        /// </summary>
        public LogMessage(int sessionId, LogLevel logLevel, Exception exception)
        {
            this.Id = sessionId;
            this.Ex = exception;
            this.Level = (int) logLevel;
            this.Msg = null;
            this.Time = DateTime.Now;
        }

        /// <summary>
        /// 会话ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public Exception Ex { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 全局唯一的索引号
        /// </summary>
        public long Index { get; set; }

        public LogMessage(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            Level = info.GetInt32("Level");
            Msg = info.GetString("Msg");
            Ex = (Exception) info.GetValue("Ex", typeof(Exception));
            Time = (DateTime) info.GetValue("Time", typeof(DateTime));
            Index = info.GetInt32("Index");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Level", Level);
            info.AddValue("Msg", Msg);
            info.AddValue("Ex", Ex);
            info.AddValue("Time", Time);
            info.AddValue("Index", Index);
        }
    }
}