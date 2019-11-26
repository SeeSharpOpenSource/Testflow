namespace Testflow.SlaveCore.Debugger
{
    /// <summary>
    /// 协程运行状态
    /// </summary>
    internal enum CoroutineState
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle,

        /// <summary>
        /// 运行中
        /// </summary>
        Running,

        /// <summary>
        /// 阻塞
        /// </summary>
        Blocked,

        /// <summary>
        /// 结束
        /// </summary>
        Over
    }
}