namespace Testflow.Data.Sequence
{
    /// <summary>
    /// Step的类型
    /// </summary>
    public enum SequenceStepType
    {
        /// <summary>
        /// 执行的步骤
        /// </summary>
        Execution = 0,

        /// <summary>
        /// 条件分支
        /// </summary>
        ConditionBlock = 1,

        /// <summary>
        /// Try-Finally块
        /// </summary>
        TryFinallyBlock = 2,

        /// <summary>
        /// 多Socket块
        /// </summary>
        MultiSocketBlock = 3,

        /// <summary>
        /// 序列调用步骤
        /// </summary>
        SequenceCall = 4,
    }
}