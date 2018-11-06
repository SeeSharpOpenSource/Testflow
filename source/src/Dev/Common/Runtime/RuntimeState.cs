namespace Testflow.Runtime
{
    /// <summary>
    /// 运行时会话的状态
    /// </summary>
    public enum RuntimeState
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        Idle,

        /// <summary>
        /// 运行状态
        /// </summary>
        Running,

        /// <summary>
        /// 阻塞状态
        /// </summary>
        Blocked,

        /// <summary>
        /// 调试阻塞状态
        /// </summary>
        DebugBlocked,

        /// <summary>
        /// 运行正常结束
        /// </summary>
        Stop,

        /// <summary>
        /// 发生异常
        /// </summary>
        Error,

        /// <summary>
        /// 申请中止
        /// </summary>
        AbortRequested,

        /// <summary>
        /// 已中止状态
        /// </summary>
        Abort,
    }
}