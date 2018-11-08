using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.ModuleInterface;

namespace Testflow.Common
{
    /// <summary>
    /// 国际化接口类
    /// </summary>
    public interface I18NInterface : IController
    {
        /// <summary>
        /// 根据LabelKey获取在当前环境下对应的字符串
        /// </summary>
        /// <param name="labelKey">待检索的LabelKey</param>
        /// <returns>国际化后的字符串</returns>
        string GetStr(string labelKey);

        /// <summary>
        /// 根据LabelKey获取在当前环境下对应的字符串，可以使用占位符替换指定位置的字符
        /// </summary>
        /// <param name="labelKey">待检索的LabelKey</param>
        /// <param name="param">替换占位符的字符集合</param>
        /// <returns>国际化后的字符串</returns>
        string GetFStr(string labelKey, params string[] param);
    }
}
