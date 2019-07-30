using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Testflow.ConfigurationManager.Data;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.ConfigurationManager
{
    internal class ConfigDataLoader : IDisposable
    {
        private readonly Dictionary<string, Func<string, object>> _valueConvertor;
        public ConfigDataLoader()
        {
            this._valueConvertor = new Dictionary<string, Func<string, object>>(10);
            _valueConvertor.Add(GetFullName(typeof(string)), strValue => strValue);
            _valueConvertor.Add(GetFullName(typeof(long)), strValue => long.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(int)), strValue => int.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(uint)), strValue => uint.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(short)), strValue => short.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(ushort)), strValue => ushort.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(char)), strValue => char.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(byte)), strValue => byte.Parse(strValue));
            _valueConvertor.Add(GetFullName(typeof(bool)), strValue => bool.Parse(strValue));
        }

        public GlobalConfigData Load(string configFile)
        {
            ConfigData configData = GetConfigData(configFile);
            GlobalConfigData globalConfigData = GetGlobalConfigData(configData);
            AddExtraGlobalConfigData(globalConfigData);
            return globalConfigData;
        }

        private ConfigData GetConfigData(string configFile)
        {
            ConfigData configData;
            using (FileStream fileStream = new FileStream(configFile, FileMode.Open))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof (ConfigData),
                    new Type[] {typeof (ConfigBlock), typeof (ConfigItem)});
                configData = xmlSerializer.Deserialize(fileStream) as ConfigData;
            }
            return configData;
        }

        private GlobalConfigData GetGlobalConfigData(ConfigData configData)
        {
            GlobalConfigData globalConfigData = new GlobalConfigData();
            foreach (ConfigBlock configBlock in configData)
            {
                string blockName = configBlock.Name;
                globalConfigData.AddConfigRoot(blockName);
                foreach (ConfigItem configItem in configBlock)
                {
                    Type valueType = Type.GetType(configItem.Type);
                    if (null == valueType)
                    {
                        I18N i18N = I18N.GetInstance(Constants.I18nName);
                        throw new TestflowRuntimeException(ModuleErrorCode.ConfigDataError,
                            i18N.GetFStr("CannotLoadType", configItem.Type));
                    }
                    object value;
                    if (valueType.IsEnum)
                    {
                        value = Enum.Parse(valueType, configItem.Value);
                    }
                    else if (valueType.IsValueType || valueType == typeof (string))
                    {
                        value = _valueConvertor[GetFullName(valueType)].Invoke(configItem.Value);
                    }
                    else if (valueType == typeof (Encoding))
                    {
                        value = Encoding.GetEncoding(configItem.Name);
                    }
                    else
                    {
                        I18N i18N = I18N.GetInstance(Constants.I18nName);
                        throw new TestflowRuntimeException(ModuleErrorCode.ConfigDataError,
                            i18N.GetFStr("UnsupportedCast", configItem.Type));
                    }
                    globalConfigData.AddConfigItem(blockName, configItem.Name, value);
                }
            }
            return globalConfigData;
        }

        private void AddExtraGlobalConfigData(GlobalConfigData configData)
        {
            string homeDir = Environment.GetEnvironmentVariable(CommonConst.EnvironmentVariable);
            configData.AddConfigItem(Constants.GlobalConfig, "TestflowHome", homeDir);

            string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            configData.AddConfigItem(Constants.GlobalConfig, "DotNetLibDir", runtimeDirectory);

            string platformDir = configData.GetConfigValue<string>(Constants.GlobalConfig, "PlatformLibDir");
            configData.SetConfigItem(Constants.GlobalConfig, "PlatformLibDir", $"{homeDir}{Path.DirectorySeparatorChar}{platformDir}");
        }

        private static string GetFullName(Type type)
        {
            return $"{type.Namespace}.{type.Name}";
        }

        public void Dispose()
        {
            _valueConvertor.Clear();
        }
    }
}