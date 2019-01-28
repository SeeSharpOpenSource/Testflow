namespace Testflow
{
    /// <summary>
    /// Testflow平台的状态信息
    /// </summary>
    public enum TestflowStates
    {
        /// <summary>
        /// 不可用状态，未初始化或已释放
        /// </summary>
        Unavailable = 0,

        /// <summary>
        /// 设计时
        /// </summary>
        Designtime = 1,

        /// <summary>
        /// 运行时设计
        /// </summary>
        RuntimeDesign = 2,

        /// <summary>
        /// 运行时调试状态
        /// </summary>
        RuntimeDebug = 3,

        /// <summary>
        /// 运行时状态
        /// </summary>
        Runtime = 4,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 5
    }
}