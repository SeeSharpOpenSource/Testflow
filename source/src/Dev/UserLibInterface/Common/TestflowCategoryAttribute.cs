using System;
using System.Reflection;
using Testflow.Data;
using Testflow.ExtensionBase;
using Testflow.Utility.I18nUtil;

namespace Testflow.Usr.Common
{
    /// <summary>
    /// 库类型属性标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TestflowCategoryAttribute : Attribute
    {
        /// <summary>
        /// 库的类型
        /// </summary>
        public string CategoryString { get; }

        /// <summary>
        /// 库的类型
        /// </summary>
        public LibraryCategory Category { get; }

        /// <summary>
        /// 创建库类型属性实例
        /// </summary>
        /// <param name="categoryString"></param>
        public TestflowCategoryAttribute(string categoryString)
        {
            I18NOption i18NOption = new I18NOption(Assembly.GetAssembly(this.GetType()), "i18n_userlib_zh",
                "i18n_userlib_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            this.CategoryString = i18N.GetStr(categoryString) ?? categoryString;
            Category = LibraryCategory.Miscellaneous;
        }

        /// <summary>
        /// 创建库类型属性实例
        /// </summary>
        public TestflowCategoryAttribute(LibraryCategory category)
        {
            I18NOption i18NOption = new I18NOption(Assembly.GetAssembly(this.GetType()), "i18n_userlib_zh",
                "i18n_userlib_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            this.Category = category;
            this.CategoryString = i18N.GetStr(category.ToString());
        }
    }
}