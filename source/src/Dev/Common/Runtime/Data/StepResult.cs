namespace Testflow.Runtime.Data
{
    /// <summary>
    /// step执行结果
    /// </summary>
    public enum StepResult
    {
        /// <summary>
        /// 结果未记录
        /// </summary>
        NotAvailable = -1,

        /// <summary>
        /// 跳过步骤
        /// </summary>
        Skip = 1,

        /// <summary>
        /// Step执行成功
        /// </summary>
        Pass = 2,

        /// <summary>
        /// Step重试执行失败，该失败状态不会影响最终的成功失败统计
        /// </summary>
        RetryFailed = 3,

        /// <summary>
        /// Step执行失败
        /// </summary>
        Failed = 4,

        /// <summary>
        /// 被终止
        /// </summary>
        Abort = 5,

        /// <summary>
        /// 超时
        /// </summary>
        Timeout = 6,

        /// <summary>
        /// 序列结束
        /// </summary>
        Over = 7,

        /// <summary>
        /// 出现错误
        /// </summary>
        Error = 8
    }
}