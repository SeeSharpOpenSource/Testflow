namespace Testflow.SlaveCore.Data
{
    public enum StatusReportType
    {
        /// <summary>
        /// 状态记录
        /// </summary>
        Record,
        /// <summary>
        /// 调试命中
        /// </summary>
        DebugHitted,
        /// <summary>
        /// 执行失败
        /// </summary>
        Failed,
        /// <summary>
        /// 执行错误
        /// </summary>
        Error,

        /// <summary>
        /// 序列结束
        /// </summary>
        Over
    }
}