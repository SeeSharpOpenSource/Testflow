namespace Testflow.Data
{
    /// <summary>
    /// Step执行失败后的行为
    /// </summary>
    public enum FailedAction
    {
        /// <summary>
        /// 序列执行终止
        /// </summary>
        Terminate = 0,

        /// <summary>
        /// 序列继续运行
        /// </summary>
        Continue = 1,

        /// <summary>
        /// 跳出循环，只在n层上级Step为Loop的情况下使能
        /// </summary>
        BreakLoop = 2,

        /// <summary>
        /// 跳出当前运行并进入下次循环，只在n层上级Step为Loop的情况下使能
        /// </summary>
        NextLoop = 3
    }
}