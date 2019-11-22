namespace Testflow.Data
{
    /// <summary>
    /// 中间数据的保存级别
    /// </summary>
    public enum RecordLevel
    {
        /// <summary>
        /// 不保存
        /// </summary>
        None = 0,

        /// <summary>
        /// 保存最终结果
        /// </summary>
        FinalResult = 1,

        /// <summary>
        /// 保存被变更的每个过程的值
        /// </summary>
        Trace = 2,

        /// <summary>
        /// 无论任何step返回状态，只要该变量可用则返回值
        /// </summary>
        FullTrace = 3
    }
}