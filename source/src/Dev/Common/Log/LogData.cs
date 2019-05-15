using System;
using System.Dynamic;
using Testflow.Usr;

namespace Testflow.Log
{
    /// <summary>
    /// 日志信息
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 序列索引号
        /// </summary>
        public int SeqIndex { get; set; }
    }
}