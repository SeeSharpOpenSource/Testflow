using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Testflow.Log;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore
{
    internal class ContextManager
    {
        private readonly Dictionary<string, string> _configData;
        private readonly Dictionary<string, Func<string, object>> _valueConvertor;

        public ContextManager(string configDataStr)
        {
            _configData = JsonConvert.DeserializeObject<Dictionary<string, string>>(configDataStr);
            _valueConvertor.Add(typeof(string).Name, strValue => strValue);
            _valueConvertor.Add(typeof(long).Name, strValue => long.Parse(strValue));
            _valueConvertor.Add(typeof(int).Name, strValue => int.Parse(strValue));
            _valueConvertor.Add(typeof(uint).Name, strValue => uint.Parse(strValue));
            _valueConvertor.Add(typeof(short).Name, strValue => short.Parse(strValue));
            _valueConvertor.Add(typeof(ushort).Name, strValue => ushort.Parse(strValue));
            _valueConvertor.Add(typeof(char).Name, strValue => char.Parse(strValue));
            _valueConvertor.Add(typeof(byte).Name, strValue => byte.Parse(strValue));
        }

        public I18N I18N { get; }

        public ILogSession LogSession { get; }

        public TDataType GetProperty<TDataType>(string propertyName)
        {
            Type dataType = typeof(TDataType);
            if (!_configData.ContainsKey(propertyName))
            {
                throw new ArgumentException($"unexist property {propertyName}");
            }
            if (!_configData.ContainsKey(dataType.Name) && !dataType.IsEnum)
            {
                throw new InvalidCastException($"Unsupported cast type: {dataType.Name}");
            }
            object value;
            if (dataType.IsEnum)
            {
                value = Enum.Parse(dataType, _configData[propertyName]);
            }
            else
            {
                value = _valueConvertor[dataType.Name].Invoke(_configData[propertyName]);
            }
            return (TDataType) value;
        }
       
    }
}