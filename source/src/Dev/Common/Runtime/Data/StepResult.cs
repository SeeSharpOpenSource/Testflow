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
        /// Step执行失败
        /// </summary>
        Failed = 3,

        /// <summary>
        /// 被终止
        /// </summary>
        Abort = 4,

        /// <summary>
        /// 超时
        /// </summary>
        Timeout = 5,

        /// <summary>
        /// 序列结束
        /// </summary>
        Over = 6,

        /// <summary>
        /// 出现错误
        /// </summary>
        Error = 7
    }
}