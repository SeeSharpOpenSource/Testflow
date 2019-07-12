using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Runtime;
using Testflow.Usr;

namespace Testflow.RuntimeService
{
    public class RuntimeConfiguration :IRuntimeConfiguration
    {
        public ISerializableMap<string, object> Properties { get; set; }

        /// <summary>
        /// 运行时类型，运行/调试
        /// </summary>
        RuntimeType Type { get; set; }
        RuntimeType IRuntimeConfiguration.Type { get; set ; }

        /// <summary>
        /// 状态更新周期，单位为ms
        /// </summary>
        int StatusTransCycle { get; set; }
        int IRuntimeConfiguration.StatusTransCycle { get; set; }

        public bool ContainsProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public object GetProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public TDataType GetProperty<TDataType>(string propertyName)
        {
            throw new NotImplementedException();
        }

        public IList<string> GetPropertyNames()
        {
            throw new NotImplementedException();
        }

        public Type GetPropertyType(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void InitExtendProperties()
        {
            throw new NotImplementedException();
        }

        public void SetProperty(string propertyName, object value)
        {
            throw new NotImplementedException();
        }
    }
}
