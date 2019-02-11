using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Resources;
using System.Threading;
using Testflow.Common;

namespace Testflow.Utility.I18nUtil
{
    /// <summary>
    /// 国际化功能类
    /// </summary>
    public class I18N : IDisposable
    {
        private static readonly ConcurrentDictionary<I18NOption, I18N> _i18nEntities = new ConcurrentDictionary<I18NOption, I18N>(new I18NOptionComparer());

        /// <summary>
        /// 获取I18N实例
        /// </summary>
        /// <param name="option">国际化参数</param>
        public static I18N GetInstance(I18NOption option)
        {
            if (!_i18nEntities.ContainsKey(option))
            {
                _i18nEntities.TryAdd(option, new I18N(option));
            }
            return _i18nEntities[option];
        }

        /// <summary>
        /// 初始化I18N实例，如果已经存在则返回
        /// </summary>
        /// <param name="option">国际化参数</param>
        public static void InitInstance(I18NOption option)
        {
            if (!_i18nEntities.ContainsKey(option))
            {
                _i18nEntities.TryAdd(option, new I18N(option));
            }
        }

        /// <summary>
        /// 获取I18n实例
        /// </summary>
        /// <param name="i18nName">i18n名称</param>
        /// <returns></returns>
        public static I18N GetInstance(string i18nName)
        {
            I18NOption fitKey = _i18nEntities.Keys.FirstOrDefault(option => i18nName.Equals(option.Name));
            if (null == fitKey)
            {
                throw new TestflowRuntimeException(TestflowErrorCode.I18nRuntimeError, GetResourceItem("NotInitialized"));
            }
            return _i18nEntities[fitKey];
        }

        /// <summary>
        /// 移除指定名称的I18n实例
        /// </summary>
        /// <param name="i18nName"></param>
        public static void RemoveInstance(string i18nName)
        {
            I18NOption fitKey = _i18nEntities.Keys.FirstOrDefault(option => i18nName.Equals(option.Name));
            I18N i18n;
            if (null != fitKey)
            {
                _i18nEntities.TryRemove(fitKey, out i18n);
                i18n?.Dispose();
            }
        }

        /// <summary>
        /// 移除指定的I18n实例
        /// </summary>
        /// <param name="option">待删除的option实例</param>
        public static void RemoveInstance(I18NOption option)
        {
            I18NOption fitKey = _i18nEntities.Keys.FirstOrDefault(item => option.Equals(item));
            I18N i18n;
            if (null != fitKey)
            {
                _i18nEntities.TryRemove(fitKey, out i18n);
                i18n?.Dispose();
            }
        }

        private readonly ResourceManager _resourceManager;

        private I18N(I18NOption option)
        {
            string languageName = Thread.CurrentThread.CurrentCulture.Name;
            string resourceShortName = null;
            if (languageName.Equals(option.FirstLanguage))
            {
                resourceShortName = option.FirstLanguageFile;
            }
            else if (languageName.Equals(option.SecondLanguage))
            {
                resourceShortName = option.SecondLanguageFile;
            }
            else if (Constants.EnglishName.Equals(option.FirstLanguage) || Constants.EnglishName.Equals(option.SecondLanguage))
            {
                resourceShortName = Constants.EnglishName.Equals(option.FirstLanguage) ?
                    option.FirstLanguage : option.SecondLanguage;
            }
            else
            {
                string msgFormat = GetResourceItem("UnsupportedLanguage");
                throw new TestflowRuntimeException(TestflowErrorCode.I18nRuntimeError, string.Format(msgFormat, languageName));
            }
            string resourceFullName = null;
            // 修改国际化为使用反射获取资源文件配置
            foreach (string resourceName in option.Assembly.GetManifestResourceNames())
            {
                if (resourceName.Equals(resourceShortName) || resourceName.Contains($".{resourceShortName}.resx") ||
                    resourceName.Contains($".{resourceShortName}.resources"))
                {
                    resourceFullName = resourceName.Remove(resourceName.LastIndexOf("."));
                    break;
                }
            }
            if (null == resourceFullName)
            {
                string msgFormat = GetResourceItem("ResourceNotExist");
                throw new TestflowRuntimeException(TestflowErrorCode.I18nRuntimeError, string.Format(msgFormat, resourceShortName));
            }
            this._resourceManager = new ResourceManager(resourceFullName, option.Assembly);
        }

//        public IModuleConfigData ConfigData { get; set; }

        /// <summary>
        /// 根据LabelKey获取在当前环境下对应的字符串
        /// </summary>
        /// <param name="labelKey">待检索的LabelKey</param>
        /// <returns>国际化后的字符串</returns>
        public string GetStr(string labelKey)
        {
            return _resourceManager.GetString(labelKey);
        }

        /// <summary>
        /// 根据LabelKey获取在当前环境下对应的字符串，可以使用占位符替换指定位置的字符
        /// </summary>
        /// <param name="labelKey">待检索的LabelKey</param>
        /// <param name="param">替换占位符的字符集合</param>
        /// <returns>国际化后的字符串</returns>
        public string GetFStr(string labelKey, params string[] param)
        {
            string msgFormat = _resourceManager.GetString(labelKey);
            if (null == msgFormat)
            {
                string errFormat = GetResourceItem("ItemNotExist");
                throw new TestflowRuntimeException(TestflowErrorCode.I18nRuntimeError, errFormat);
            }
            return string.Format(msgFormat, param);
        }

        public void Dispose()
        {
            // TODO
        }

        private static string GetResourceItem(string labelKey)
        {
            ResourceManager resource;
            switch (Thread.CurrentThread.CurrentCulture.Name)
            {
                case Constants.ChineseName:
                    resource = new ResourceManager("Testflow.Utility.Resources.i18n_i18n_zh", typeof(I18N).Assembly);
                    break;
                case Constants.EnglishName:
                    resource = new ResourceManager("Testflow.Utility.Resources.i18n_i18n_en", typeof (I18N).Assembly);
                    break;
                default:
                    resource = new ResourceManager("Testflow.Utility.Resources.i18n_i18n_en", typeof(I18N).Assembly);
                    break;
            }
            return resource.GetString(labelKey);
        }
    }
}
