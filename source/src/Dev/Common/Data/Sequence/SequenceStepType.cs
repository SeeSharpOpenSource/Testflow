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
        /// 条件表达语句
        /// </summary>
        ConditionStatement = 2,

        /// <summary>
        /// Try-Finally块
        /// </summary>
        TryFinallyBlock = 3,

        /// <summary>
        /// 条件循环块
        /// </summary>
        ConditionLoop = 4,

        /// <summary>
        /// 序列调用步骤
        /// </summary>
        SequenceCall = 5,

        /// <summary>
        /// 跳转
        /// </summary>
        Goto = 6,

        /// <summary>
        /// 多线程块
        /// </summary>
        MultiThreadBlock = 7,

        /// <summary>
        /// 定时调用块
        /// </summary>
        TimerBlock = 8,

        /// <summary>
        /// 批处理块
        /// </summary>
        BatchBlock = 9
    }
}