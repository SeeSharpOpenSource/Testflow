using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace Testflow.DataMaintainer
{
    internal class DataModelMapper
    {

        public DataModelMapper()
        {
//            Dictionary<string, string> 
        }

        public TDataType ReadToObject<TDataType>(DbDataReader reader, TDataType dataObj) where TDataType : class
        {
            Type type = dataObj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in properties)
            {
                object propertyValue = reader[propertyInfo.Name];
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
    }
}