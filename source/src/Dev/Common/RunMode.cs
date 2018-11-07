namespace Testflow
{
    /// <summary>
    /// Testflow的运行模式
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// 完整运行，启用所有模块
        /// </summary>
        Full = 0,

        /// <summary>
        /// 最小运行模式，只启动运行相关模块
        /// </summary>
        Minimal = 1
    }
}