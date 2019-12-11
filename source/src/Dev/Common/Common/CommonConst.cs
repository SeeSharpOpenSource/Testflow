namespace Testflow.Usr
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

        /// <summary>
        /// 英文资源文件后缀
        /// </summary>
        public const string EnglishResourceName = "en";

        internal const string I18nName = "LoggerI18N";

        /// <summary>
        /// 标记Testflow根目录的环境变量
        /// </summary>
        public const string EnvironmentVariable = "TESTFLOW_HOME";

        /// <summary>
        /// 标记Testflow根目录的环境变量
        /// </summary>
        public const string WorkspaceVariable = "TESTFLOW_WORKSPACE";

        /// <summary>
        /// 数据文件目录
        /// </summary>
        public const string DataDir = "data";

        /// <summary>
        /// 部署文件目录
        /// </summary>
        public const string DeployDir = "deploy";

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

        /// <summary>
        /// 广播会话ID
        /// </summary>
        public const int BroadcastSession = -10;

        /// <summary>
        /// Setup序列的id
        /// </summary>
        public const int SetupIndex = -1;

        /// <summary>
        /// Teardown序列的id
        /// </summary>
        public const int TeardownIndex = -2;

        /// <summary>
        /// 配置实例属性的方法
        /// </summary>
        public const string SetInstancePropertyFunc = "SetInstanceProperties";

        /// <summary>
        /// 配置实例属性的方法
        /// </summary>
        public const string SetStaticPropertyFunc = "SetStaticProperties";

        /// <summary>
        /// 配置实例属性的方法
        /// </summary>
        public const string SetInstanceFieldFunc = "SetInstanceFields";

        /// <summary>
        /// 配置实例属性的方法
        /// </summary>
        public const string SetStaticFieldFunc = "SetStaticFields";

        /// <summary>
        /// 全局通用的时间戳格式化字符串
        /// </summary>
        public const string GlobalTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffK";

        /// <summary>
        /// 单个序列能支持的协程上限数量
        /// </summary>
        public const int SequenceCoroutineCapacity = 1000;
    }
}