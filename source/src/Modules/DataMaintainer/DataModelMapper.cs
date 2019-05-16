using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public DataModelMapper()
        {
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
        }

        public TDataType ReadToObject<TDataType>(DbDataReader reader, TDataType dataObj) where TDataType : class
        {
            Type type = dataObj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in properties)
            {
                string columnName = GetTableColumnName(type.Name, propertyInfo.Name);
                object propertyValue = reader[columnName];
                if (null == propertyValue)
                {
                    continue;
                }
                if (typeof (DateTime).Name.Equals(typeof (TDataType).Name))
                {
                    propertyValue = DateTime.Parse((string)propertyValue);
                }
                propertyInfo.SetValue(dataObj, propertyValue);
            }
            return dataObj;
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
    }
}