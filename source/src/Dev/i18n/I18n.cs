using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using Testflow.Common;
using Testflow.Data;
using Testflow.Modules;
using Testflow.Runtime;

namespace Testflow.i18n
{
    /// <summary>
    /// 国际化功能类
    /// </summary>
    public class I18N : IDisposable
    {
        private static readonly ConcurrentDictionary<I18NOption, I18N> _i18nEntities = new ConcurrentDictionary<I18NOption, I18N>(new I18NOptionComparer());

        /// <summary>
        /// 获取I18n实例
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static I18N GetInstance(I18NOption option)
        {
            if (!_i18nEntities.ContainsKey(option))
            {
                _i18nEntities.TryAdd(option, new I18N(option));
            }
            return _i18nEntities[option];
        }

        /// <summary>
        /// 获取I18n实例
        /// </summary>
        /// <param name="i18nName">i18n名称</param>
        /// <returns></returns>
        public static I18N GetInstance(string i18nName)
        {
            I18NOption fitKey = _i18nEntities.Keys.First(option => i18nName.Equals(option.Name));
            if (null == fitKey)
            {
                throw new TestflowRuntimeException(TestflowErrorCode.I18nRuntimeError, GetResourceItem("NotInitialized"));
            }
            return _i18nEntities[fitKey];
        }

        private readonly ResourceManager _resourceManager;

        private I18N(I18NOption option)
        {
            string languageName = Thread.CurrentThread.CurrentCulture.Name;
            string resource = null;
            if (languageName.Equals(option.FirstLanguage))
            {
                resource = option.FirstLanFile;
            }
            else if (languageName.Equals(option.SecondLanguage))
            {
                resource = option.SecondLanFile;
            }
            else if (Constants.EnglishName.Equals(option.FirstLanguage) || Constants.EnglishName.Equals(option.SecondLanguage))
            {
                resource = Constants.EnglishName.Equals(option.FirstLanguage) ?
                    option.FirstLanguage : option.SecondLanguage;
            }
            else
            {
                string msgFormat = GetResourceItem("UnsupportedLanguage");
                throw new TestflowRuntimeException(TestflowErrorCode.I18nRuntimeError, string.Format(msgFormat, languageName));
            }
            this._resourceManager = new ResourceManager(resource, option.Assembly);
            if (null == this._resourceManager)
            {
                string msgFormat = GetResourceItem("ResourceNotExist");
                throw new TestflowRuntimeException(TestflowErrorCode.I18nRuntimeError, string.Format(msgFormat, resource));
            }
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
                    resource = new ResourceManager("Resources.i18n_i18n_zh.resx", typeof(I18N).Assembly);
                    break;
                case Constants.EnglishName:
                    resource = new ResourceManager("Resources.i18n_i18n_en.resx", typeof (I18N).Assembly);
                    break;
                default:
                    resource = new ResourceManager("Resources.i18n_i18n_en.resx", typeof(I18N).Assembly);
                    break;
            }
            return resource.GetString(labelKey);
        }
    }
}
