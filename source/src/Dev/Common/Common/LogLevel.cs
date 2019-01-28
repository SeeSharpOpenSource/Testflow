namespace Testflow.Common
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 最低级别
        /// </summary>
        Trace = 0,

        /// <summary>
        /// 调试级别
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 消息级别
        /// </summary>
        Info = 2,

        /// <summary>
        /// 警告级别
        /// </summary>
        Warn =3,

        /// <summary>
        /// 错误级别
        /// </summary>
        Error = 4,

        /// <summary>
        /// 严重错误级别
        /// </summary>
        Fatal = 5, 

        /// <summary>
        /// 关闭日志
        /// </summary>
        Off = 6
    }
}