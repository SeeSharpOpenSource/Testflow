using System;
using System.Collections.Generic;
using Testflow.Modules;
using Testflow.Usr;
using Testflow.Utility.Collections;

namespace Testflow.ConfigurationManager
{
    public class ModuleConfigData : IModuleConfigData
    {
        internal ModuleConfigData(IDictionary<string, object> globalConfig, IDictionary<string, object> moduleConfig)
        {
            this.Properties = new SerializableMap<string, object>(globalConfig.Count + moduleConfig?.Count ?? 0);
            foreach (KeyValuePair<string, object> keyValuePair in globalConfig)
            {
                SetProperty(keyValuePair.Key, keyValuePair.Value);
            }
            if (null != moduleConfig)
            {
                foreach (KeyValuePair<string, object> keyValuePair in moduleConfig)
                {
                    SetProperty(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        internal ModuleConfigData()
        {
            this.Properties = new SerializableMap<string, object>(10);
        }

        public void InitExtendProperties()
        {
            // ignore
        }

        public ISerializableMap<string, object> Properties { get; }
        public void SetProperty(string propertyName, object value)
        {
            if (Properties.ContainsKey(propertyName))
            {
                this.Properties[propertyName] = value;
            }
            else
            {
                this.Properties.Add(propertyName, value);
            }
        }

        public object GetProperty(string propertyName)
        {
            return this.Properties.ContainsKey(propertyName) ? Properties[propertyName] : null;
        }

        public TDataType GetProperty<TDataType>(string propertyName)
        {
            return (TDataType) this.Properties[propertyName];
        }

        public Type GetPropertyType(string propertyName)
        {
            return (Properties.ContainsKey(propertyName) && null != Properties[propertyName])
                ? Properties[propertyName].GetType()
                : null;
        }

        public bool ContainsProperty(string propertyName)
        {
            return Properties.ContainsKey(propertyName);
        }

        public IList<string> GetPropertyNames()
        {
            return new List<string>(Properties.Keys);
        }

        public string Version { get; set; }
        public string Name { get; set; }
    }
}