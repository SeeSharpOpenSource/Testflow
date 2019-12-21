namespace Testflow.ConfigurationManager
{
    public static class Constants
    {
        public const string I18nName = "Config";

        #region 配置文件信息

        public const string ConfigFileDir = "deploy";
        public const string ConfigFileName = "config.xml";

        public const string ConfigFile = "ConfigFile";

        public const string ConfigName = "TestflowConfiguration";

        #endregion


        #region 模块配置父节点名称

        public const string GlobalConfig = "GlobalConfig";
        public const string RuntimeConfig = "Runtime";
        public const string EngineConfig = "EngineCore";
        public const string DataMaintain = "DataMaintain";
        public const string ResultManage = "ResultManager";
        public const string SequenceManage = "SequenceInfo";
        public const string ParamCheck = "ParameterCheck";
        public const string InterfaceLoad = "InterfaceLoad";

        #endregion

        #region 路径相关常量

        public const string WindirVar = "windir";
        public const string DotNetRootDirFormat = @"{0}Microsoft.NET\assembly\";


        #endregion


    }
}