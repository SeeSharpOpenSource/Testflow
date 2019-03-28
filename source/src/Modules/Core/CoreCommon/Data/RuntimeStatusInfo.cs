using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.Runtime;
using Testflow.Utility.Collections;

namespace Testflow.CoreCommon.Data
{
    public class RuntimeStatusInfo : IRuntimeStatusInfo
    {

        public int SessionId { get; set; }
        public ulong StatusIndex { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public DateTime RecordTime { get; set; }
        public long MemoryUsed { get; set; }
        public long MemoryAllocated { get; set; }
        public ulong ProcessorTime { get; set; }
        public RuntimeState State { get; set; }

        public ICallStack CallStack { get; set; }

        public RuntimeStatusInfo()
        {
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
            throw new NotImplementedException();
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

    }
}