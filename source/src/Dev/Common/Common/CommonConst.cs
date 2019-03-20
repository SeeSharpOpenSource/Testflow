namespace Testflow.Common
{
    /// <summary>
    /// 公共常量类
    /// </summary>
    public static class CommonConst
    {
        /// <summary>
        /// 外部属性的默认扩展参数个数
        /// </summary>
        internal const int DefaultExtendParamCapacity = 4;

        /// <summary>
        /// 序列信息文件扩展名
        /// </summary>
        public const string SequenceFileExtension = "tfseq";

        /// <summary>
        /// 序列参数配置文件的扩展名
        /// </summary>
        public const string SequenceDataFileExtension = "tfparam";

        /// <summary>
        /// 测试组工程文件扩展名
        /// </summary>
        public const string TestGroupFileExtension = "tfproj";

        /// <summary>
        /// 日志文件扩展名
        /// </summary>
        public const string LogFileExtension = "log";

        /// <summary>
        /// 最小有效的double值
        /// </summary>
        public const double MinDoubleValue = 1E-200;

        public const string ChineseName = "zh-CN";
        public const string EnglishName = "en-US";

        internal const string I18nName = "LoggerI18N";

        /// <summary>
        /// 平台日志会话id
        /// </summary>
        public const int PlatformLogSession = 1000000;

        /// <summary>
        /// 平台日志会话id
        /// </summary>
        public const int PlatformSession = 1000000;

        /// <summary>
        /// 测试工程的会话id
        /// </summary>
        public const int TestGroupSession = -1;
    }
}