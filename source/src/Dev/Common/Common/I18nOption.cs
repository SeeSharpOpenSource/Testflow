namespace Testflow.Common
{
    /// <summary>
    /// I18n参数配置类
    /// </summary>
    public class I18nOption
    {
        /// <summary>
        /// 创建I18n参数类
        /// </summary>
        /// <param name="firstLanFile">第一语言信息所在资源文件</param>
        /// <param name="secondLanFile">第二语言信息所在资源文件</param>
        public I18nOption(string firstLanFile, string secondLanFile)
        {
            this.FirstLanguage = "zh-CN";
            this.SecondLanguage = "en-US";

            this.FirstLanFile = firstLanFile;
            this.SecondLanFile = secondLanFile;
        }

        /// <summary>
        /// 第一语言所在资源文件
        /// </summary>
        public string FirstLanFile { get; set; }
        
        /// <summary>
        /// 第二语言所在资源文件
        /// </summary>
        public string SecondLanFile { get; set; }
        
        /// <summary>
        /// 第一语言名称
        /// </summary>
        public string FirstLanguage { get; set; }
        
        /// <summary>
        /// 第二语言名称
        /// </summary>
        public string SecondLanguage { get; set; }
    }
}