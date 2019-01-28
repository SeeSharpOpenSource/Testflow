namespace Testflow.Common
{
    /// <summary>
    /// Testflow异常码定义
    /// </summary>
    public static class TestflowErrorCode
    {
        /// <summary>
        /// 内部异常
        /// </summary>
        public const int InternalError = -100;

        /// <summary>
        /// 非法操作
        /// </summary>
        public const int InvalidOperation = -101;

        /// <summary>
        /// 日志队列初始化失败
        /// </summary>
        public const int LogQueueInitFailed = -1000;

        /// <summary>
        /// 非法日志参数
        /// </summary>
        public const int InvalidLogArgument = -1001;

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