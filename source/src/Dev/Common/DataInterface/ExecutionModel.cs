namespace Testflow.DataInterface
{
    /// <summary>
    /// 执行模型，顺序执行/并行执行
    /// </summary>
    public enum ExecutionModel
    {
        /// <summary>
        /// 顺序执行
        /// </summary>
        SequentialExecution = 0,

        /// <summary>
        /// 并行执行
        /// </summary>
        ParallelExecution = 1
    }
}