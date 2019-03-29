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
        NoRecod,

        /// <summary>
        /// 跳过步骤
        /// </summary>
        Skip,

        /// <summary>
        /// Step执行成功
        /// </summary>
        Pass,

        /// <summary>
        /// Step执行失败
        /// </summary>
        Failed
    }
}