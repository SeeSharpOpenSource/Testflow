using Testflow.Usr;

namespace Testflow.Logger
{
    /// <summary>
    /// 模块异常码
    /// </summary>
    public static class ModuleErrorCode
    {
        /// <summary>
        /// 日志队列初始化失败
        /// </summary>
        public const int LogQueueInitFailed = 1 | CommonErrorCode.LogErrorMask;

        /// <summary>
        /// 非法日志参数
        /// </summary>
        public const int InvalidLogArgument = 2 | CommonErrorCode.LogErrorMask;
    }
}