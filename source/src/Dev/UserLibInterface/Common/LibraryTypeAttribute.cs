using System;
using System.Reflection;
using Testflow.Utility.I18nUtil;

namespace Testflow.Usr.Common
{
    /// <summary>
    /// 库类型属性标签
    /// </summary>
    public class LibraryTypeAttribute : Attribute
    {
        /// <summary>
        /// 库的类型
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// 创建库类型属性实例
        /// </summary>
        /// <param name="type"></param>
        public LibraryTypeAttribute(string type)
        {
            I18NOption i18NOption = new I18NOption(Assembly.GetAssembly(this.GetType()), "i18n_userlib_zh",
                "i18n_userlib_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            this.Type = i18N.GetStr(type) ?? type;
        }
    }
}