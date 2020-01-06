namespace Testflow.SlaveCore.Coroutine
{
    /// <summary>
    /// 协程运行状态
    /// </summary>
    internal enum CoroutineState
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        Running = 1,

        /// <summary>
        /// 阻塞
        /// </summary>
        Blocked = 2,

        /// <summary>
        /// 结束
        /// </summary>
        Over = 3
    }
}