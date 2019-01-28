using System.Reflection;
using Testflow.Common;

namespace Testflow.I18nUtil
{
    /// <summary>
    /// I18n参数配置类
    /// </summary>
    internal class I18NOption
    {
        /// <summary>
        /// 创建I18n参数类
        /// </summary>
        /// <param name="assembly">应用的程序集</param>
        /// <param name="firstLanguageFile">第一语言信息所在资源文件</param>
        /// <param name="secondLanguageFile">第二语言信息所在资源文件</param>
        public I18NOption(Assembly assembly, string firstLanguageFile, string secondLanguageFile)
        {
            this.Assembly = assembly;

            this.FirstLanguage = CommonConst.ChineseName;
            this.SecondLanguage = CommonConst.EnglishName;

            this.FirstLanguageFile = firstLanguageFile;
            this.SecondLanguageFile = secondLanguageFile;

            this._name = null;
        }

        /// <summary>
        /// 程序集
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// 第一语言所在资源文件
        /// </summary>
        public string FirstLanguageFile { get; set; }
        
        /// <summary>
        /// 第二语言所在资源文件
        /// </summary>
        public string SecondLanguageFile { get; set; }
        
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
                return _name ?? $"{Assembly.FullName}+{FirstLanguage}+${SecondLanguage}+{FirstLanguageFile}+${SecondLanguageFile}";
            }
            set { this._name = value; }
        }

        /// <summary>
        /// 判断两个Option实例是否相等
        /// </summary>
        public bool Equals(I18NOption comparer)
        {
            return ReferenceEquals(Assembly, comparer.Assembly) && FirstLanguage.Equals(comparer.FirstLanguage) &&
                   SecondLanguage.Equals(comparer.SecondLanguage) && FirstLanguageFile.Equals(comparer.FirstLanguageFile) &&
                   SecondLanguageFile.Equals(comparer.SecondLanguageFile);
        }
    }
}