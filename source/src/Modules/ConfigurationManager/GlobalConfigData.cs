using System;
using System.Collections.Generic;
using Testflow.Modules;

namespace Testflow.ConfigurationManager
{
    internal class GlobalConfigData : IDisposable
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

        public void Dispose()
        {
            _configData = null;
        }
    }
}