using System;
using System.Collections.Generic;
using System.Text;
using Testflow.ConfigurationManager.Data;
using Testflow.Modules;

namespace Testflow.ConfigurationManager
{
    public class GlobalConfigData : IDisposable
    {
        public GlobalConfigData()
        {
            this._configData = new Dictionary<string, Dictionary<string, object>>(10);
        }

        private Dictionary<string, Dictionary<string, object>> _configData;

        public void AddConfigItem(string rootName, string property, object value)
        {
            _configData[rootName].Add(property, value);
        }

        public void SetConfigItem(string rootName, string property, object value)
        {
            _configData[rootName][property] = value;
        }

        public void AddConfigRoot(string rootName)
        {
            _configData.Add(rootName, new Dictionary<string, object>(20));
        }

        public IModuleConfigData GetGlobalConfigData()
        {
             return new ModuleConfigData(_configData[Constants.GlobalConfig], null);
        }

        public IModuleConfigData GetModuleConfigData(string moduleName)
        {
            IDictionary<string, object> moduleConfigData = _configData.ContainsKey(moduleName)
                ? _configData[moduleName]
                : null;
            return new ModuleConfigData(_configData[Constants.GlobalConfig], moduleConfigData);
        }

        public TDataType GetConfigValue<TDataType>(string blockName, string propertyName)
        {
            return (TDataType) _configData[blockName][propertyName];
        }

        public ConfigData GetConfigData()
        {
            ConfigData configData = new ConfigData();
            configData.Name = Constants.ConfigName;
            foreach (KeyValuePair<string, Dictionary<string, object>> keyValuePair in _configData)
            {
                ConfigBlock configBlock = new ConfigBlock();
                configBlock.Name = keyValuePair.Key;
                configData.ModuleConfigData.Add(configBlock);
                foreach (KeyValuePair<string, object> itemPair in keyValuePair.Value)
                {
                    ConfigItem configItem = new ConfigItem();
                    configItem.Name = itemPair.Key;
                    configItem.Value = itemPair.Value.ToString();
                    Type type = itemPair.Value.GetType();
                    if (itemPair.Key.Equals("PlatformEncoding"))
                    {
                        type = typeof (Encoding);
                    }
                    configItem.Type = $"{type.Namespace}.{type.Name}";
                    configBlock.ConfigItems.Add(configItem);
                    
                }
            }
            return configData;
        }

        public void Dispose()
        {
            _configData = null;
        }
    }
}