namespace Testflow.SlaveCore.Data
{
    public enum StatusReportType
    {
        /// <summary>
        /// 序列开始执行
        /// </summary>
        Start = 0,

        /// <summary>
        /// 状态记录
        /// </summary>
        Record = 1,

        /// <summary>
        /// 调试命中
        /// </summary>
        DebugHitted = 2,

        /// <summary>
        /// 同步请求
        /// </summary>
        SyncRequested = 3,

        /// <summary>
        /// 执行失败
        /// </summary>
        Failed = 4,

        /// <summary>
        /// 序列结束
        /// </summary>
        Over = 5,

        /// <summary>
        /// 执行错误
        /// </summary>
        Error = 6
    }
}