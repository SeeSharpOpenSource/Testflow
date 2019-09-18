using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Sockets;
using System.Reflection;
using Testflow.Runtime.Data;

namespace Testflow.DataMaintainer
{
    internal class DataModelMapper
    {
        // 数据类型到对应表格的映射
        private readonly Dictionary<string, string> _typeToTableMapping;
        // 对应表格->每个表格的映射表->表格列名和属性名的正反映射
        private readonly Dictionary<string, Dictionary<string, string>> _tableToColumnPropertyMapping;

        private readonly Dictionary<string, Func<object, string>> _valueToStrConvertor;
        
        private readonly Dictionary<string, Func<object, object>> _rawDataToValueConvertor;

        // 类型转换和字符串转换为对象委托的映射
        private readonly Dictionary<Type, Func<object, string>> _classTypeConvertorMapping;

        // 类型转换和对象转换为字符串委托的映射
        private readonly Dictionary<Type, Func<string, object>> _classTypeParserMapping;


        public DataModelMapper()
        {
            _classTypeParserMapping = new Dictionary<Type, Func<string, object>>(10);
            _classTypeConvertorMapping = new Dictionary<Type, Func<object, string>>(10);

            _typeToTableMapping = new Dictionary<string, string>(10)
            {
                {typeof (TestInstanceData).Name, DataBaseItemNames.InstanceTableName},
                {typeof (SessionResultData).Name, DataBaseItemNames.SessionTableName},
                {typeof (SequenceResultData).Name, DataBaseItemNames.SequenceTableName},
                {typeof (RuntimeStatusData).Name, DataBaseItemNames.StatusTableName},
                {typeof (PerformanceStatus).Name, DataBaseItemNames.PerformanceTableName}
            };

            _tableToColumnPropertyMapping = new Dictionary<string, Dictionary<string, string>>(10)
            {
                {DataBaseItemNames.InstanceTableName, new Dictionary<string, string>()},
                {
                    DataBaseItemNames.SessionTableName, new Dictionary<string, string>()
                    {
                        { "Session", DataBaseItemNames.SessionIdColumn },
                        { DataBaseItemNames.SessionIdColumn, "Session" },
                        { "State", DataBaseItemNames.SessionStateColumn},
                        { DataBaseItemNames.SessionStateColumn, "State"}
                    }
                },
                {
                    DataBaseItemNames.SequenceTableName, new Dictionary<string, string>(10)
                    {
                        { "Session", DataBaseItemNames.SessionIdColumn },
                        { DataBaseItemNames.SessionIdColumn, "Session" },
                        { "Result", DataBaseItemNames.SequenceResultColumn },
                        { DataBaseItemNames.SequenceResultColumn, "Result" },
                    }
                },
                {
                    DataBaseItemNames.StatusTableName, new Dictionary<string, string>(10)
                    {
                        { "Session", DataBaseItemNames.SessionIdColumn },
                        { DataBaseItemNames.SessionIdColumn, "Session" },
                        { "Sequence", DataBaseItemNames.SequenceIndexColumn },
                        { DataBaseItemNames.SequenceIndexColumn, "Sequence" },
                        { "Time", DataBaseItemNames.RecordTimeColumn },
                        { DataBaseItemNames.RecordTimeColumn, "Time" },
                        { "Result", DataBaseItemNames.StepResultColumn },
                        { DataBaseItemNames.StepResultColumn, "Result" },
                    }
                },
                {
                    DataBaseItemNames.PerformanceTableName, new Dictionary<string, string>(10)
                    {
                        { "Session", DataBaseItemNames.SessionIdColumn },
                        { DataBaseItemNames.SessionIdColumn, "Session" },
                        { "Index", DataBaseItemNames.StatusIndexColumn },
                        { DataBaseItemNames.StatusIndexColumn, "Index" },
                        { "TimeStamp", DataBaseItemNames.RecordTimeColumn },
                        { DataBaseItemNames.RecordTimeColumn, "TimeStamp" },
                    }
                }
            };

            this._valueToStrConvertor = new Dictionary<string, Func<object, string>>(10);
            this._valueToStrConvertor.Add(typeof (int).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (double).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (uint).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (short).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (ushort).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (byte).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (char).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (long).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (ulong).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (float).Name, new Func<object, string>((value) => value.ToString()));
            this._valueToStrConvertor.Add(typeof (string).Name, new Func<object, string>((value) => $"'{value}'"));
            this._valueToStrConvertor.Add(typeof (DateTime).Name,
                new Func<object, string>((value) => $"'{((DateTime) value).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")}'"));

            this._rawDataToValueConvertor = new Dictionary<string, Func<object, object>>(10);
            this._rawDataToValueConvertor.Add(typeof(int).Name, new Func<object, object>((rawValue) => Convert.ToInt32(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(double).Name, new Func<object, object>((rawValue) => Convert.ToDouble(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(uint).Name, new Func<object, object>((rawValue) => Convert.ToUInt32(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(short).Name, new Func<object, object>((rawValue) => Convert.ToInt16(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(ushort).Name, new Func<object, object>((rawValue) => Convert.ToUInt16(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(byte).Name, new Func<object, object>((rawValue) => Convert.ToByte(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(char).Name, new Func<object, object>((rawValue) => Convert.ToChar(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(long).Name, new Func<object, object>((rawValue) => rawValue));
            this._rawDataToValueConvertor.Add(typeof(ulong).Name, new Func<object, object>((rawValue) => Convert.ToUInt64(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(float).Name, new Func<object, object>((rawValue) => Convert.ToSingle(rawValue)));
            this._rawDataToValueConvertor.Add(typeof(string).Name, new Func<object, object>((rawValue) => rawValue));
        }

        public TDataType ReadToObject<TDataType>(DbDataReader reader, TDataType dataObj) where TDataType : class
        {
            Type type = dataObj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in properties)
            {
                string columnName = GetTableColumnName(type.Name, propertyInfo.Name);
                object propertyValue = reader[columnName];
                if (null == propertyValue || DBNull.Value == propertyValue)
                {
                    continue;
                }
                Type propertyType = propertyInfo.PropertyType;
                if (typeof (DateTime).Name.Equals(propertyType.Name))
                {
                    propertyValue = DateTime.Parse((string)propertyValue);
                }
                else if (propertyType.IsEnum)
                {
                    propertyValue = Enum.Parse(propertyType, (string)propertyValue);
                }
                else if (_rawDataToValueConvertor.ContainsKey(propertyType.Name))
                {
                    propertyValue = _rawDataToValueConvertor[propertyType.Name].Invoke(propertyValue);
                }
                else if (_classTypeParserMapping.ContainsKey(propertyType))
                {
                    propertyValue = _classTypeParserMapping[propertyType].Invoke(propertyValue.ToString());
                }
                propertyInfo.SetValue(dataObj, propertyValue);
            }
            return dataObj;
        }

        public Dictionary<string, string> GetColumnValueMapping<TDataType>(TDataType dataObj) where TDataType : class
        {
            Type type = dataObj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Dictionary<string, string> columnValueMapping = new Dictionary<string, string>(properties.Length);
            foreach (PropertyInfo propertyInfo in properties)
            {
                string columnName = GetTableColumnName(type.Name, propertyInfo.Name);
                Type propertyType = propertyInfo.PropertyType;
                object value = propertyInfo.GetValue(dataObj);
                if (null == value) continue;
                string valueStr = string.Empty;
                if (propertyType.IsEnum)
                {
                    valueStr = $"'{value}'";
                }
                else if (_valueToStrConvertor.ContainsKey(propertyType.Name))
                {
                    valueStr = _valueToStrConvertor[propertyType.Name].Invoke(value);
                }
                else if (_classTypeConvertorMapping.ContainsKey(propertyType))
                {
                    valueStr = $"'{_classTypeConvertorMapping[propertyType].Invoke(value)}'";
                }
                columnValueMapping.Add(columnName, valueStr);
            }
            return columnValueMapping;
        }

        private string GetTableColumnName(string typeName, string propertyName)
        {
            string tableName = _typeToTableMapping[typeName];
            if (!_tableToColumnPropertyMapping[tableName].ContainsKey(propertyName))
            {
                return propertyName;
            }
            return _tableToColumnPropertyMapping[tableName][propertyName];
        }

        private string GetPropertyName(string typeName, string tableColumnName)
        {
            string tableName = _typeToTableMapping[typeName];
            if (!_tableToColumnPropertyMapping[tableName].ContainsKey(tableColumnName))
            {
                return tableColumnName;
            }
            return _tableToColumnPropertyMapping[tableName][tableColumnName];
        }

        public void RegisterTypeConvertor(Type type, Func<object, string> toStringFunc, Func<string, object> parseFunc)
        {
            if (_classTypeConvertorMapping.ContainsKey(type))
            {
                _classTypeConvertorMapping[type] = toStringFunc;
                _classTypeParserMapping[type] = parseFunc;
            }
            else
            {
                _classTypeConvertorMapping.Add(type, toStringFunc);
                _classTypeParserMapping.Add(type, parseFunc);
            }
        }
    }
}