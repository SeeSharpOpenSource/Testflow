namespace Testflow.Logger
{
    internal static class Constants
    {
        public const int DesigntimeSessionId = -1;
        public const int DefaultLogStreamSize = 8;
        public const string I18NName = "logger";
        public const string LogQueueName = @".\testflowlog\Journal$";

        public const string PlatformLogName = "Testflow.Platform";
        public const string RuntimeLogName = "Testflow.Runtime";

        /// <summary>
        /// 日志队列初始化失败
        /// </summary>
        public const int LogQueueInitFailed = -1000;

        /// <summary>
        /// 非法日志参数
        /// </summary>
        public const int InvalidLogArgument = -1001;
    }
}