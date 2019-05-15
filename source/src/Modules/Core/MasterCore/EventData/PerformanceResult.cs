using System;
using System.Collections.Generic;
using Testflow.Usr;
using Testflow.Runtime;

namespace Testflow.MasterCore.EventData
{
    public class PerformanceResult : IPerformanceResult
    {
        public PerformanceResult()
        {
            this.CpuTime = 0;
            this.AverageAllocatedMemory = 0;
            this.AverageUsedMemory = 0;
            this.MaxAllocatedMemory = 0;
            this.MaxUsedMemory = 0;
        }

        public void InitExtendProperties()
        {
            throw new NotImplementedException();
        }

        public ISerializableMap<string, object> Properties { get; }
        public void SetProperty(string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public object GetProperty(string propertyName)
        {
            return null;
        }

        public TDataType GetProperty<TDataType>(string propertyName)
        {
            throw new NotImplementedException();
        }

        public Type GetPropertyType(string propertyName)
        {
            throw new NotImplementedException();
        }

        public bool ContainsProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public IList<string> GetPropertyNames()
        {
            throw new NotImplementedException();
        }

        public ulong CpuTime { get; set; }
        public long AverageAllocatedMemory { get; set; }
        public long MaxAllocatedMemory { get; set; }
        public long AverageUsedMemory { get; set; }
        public long MaxUsedMemory { get; set; }
    }
}