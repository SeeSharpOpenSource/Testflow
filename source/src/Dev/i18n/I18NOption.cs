using System.Reflection;

namespace Testflow.i18n
{
    /// <summary>
    /// I18n参数配置类
    /// </summary>
    public class I18NOption
    {
        /// <summary>
        /// 创建I18n参数类
        /// </summary>
        /// <param name="assembly">应用的程序集</param>
        /// <param name="firstLanFile">第一语言信息所在资源文件</param>
        /// <param name="secondLanFile">第二语言信息所在资源文件</param>
        public I18NOption(Assembly assembly, string firstLanFile, string secondLanFile)
        {
            this.Assembly = assembly;

            this.FirstLanguage = Constants.ChineseName;
            this.SecondLanguage = Constants.EnglishName;

            this.FirstLanFile = firstLanFile;
            this.SecondLanFile = secondLanFile;

            this._name = null;
        }

        /// <summary>
        /// 程序集
        /// </summary>
        public Assembly Assembly { get; }

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

        private string _name;
        /// <summary>
        /// 国际化模块名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name ?? $"{Assembly.FullName}+{FirstLanguage}+${SecondLanguage}+{FirstLanFile}+${SecondLanFile}";
            }
            set { this._name = value; }
        }
    }
}