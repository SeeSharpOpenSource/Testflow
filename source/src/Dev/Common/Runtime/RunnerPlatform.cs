namespace Testflow.Runtime
{
    /// <summary>
    /// 测试的运行平台
    /// </summary>
    public enum RunnerPlatform
    {
        /// <summary>
        /// 使用TestFlow应用的运行平台
        /// </summary>
        Default = 0,

        /// <summary>
        /// 32位运行平台
        /// </summary>
        X86 = 1,

        /// <summary>
        /// 64位运行平台
        /// </summary>
        X64 = 2
    }
}