namespace Testflow.Common
{
    /// <summary>
    /// Testflow异常码定义
    /// </summary>
    public static class TestflowErrorCode
    {
        /// <summary>
        /// 日志队列初始化失败
        /// </summary>
        public const int LogQueueInitFailed = -1000;

        /// <summary>
        /// 国际化模块运行时异常
        /// </summary>
        public const int I18nRuntimeError = -1100;

        /// <summary>
        /// 信使运行时异常
        /// </summary>
        public const int MessengerRuntimeError = 1200;
    }
}