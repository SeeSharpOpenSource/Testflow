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
        public int SequenceIndex { get; set; }
        public ulong StatusIndex { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public DateTime RecordTime { get; set; }
        public long MemoryUsed { get; set; }
        public long MemoryAllocated { get; set; }
        public ulong ProcessorTime { get; set; }
        public RuntimeState State { get; set; }

        [MQIgnore]
        public ICallStack CallStack => RawCallStack;

        [MQIgnore]
        public ISerializableMap<string, object> VariableValues => RawVarValues;

        public CallStack RawCallStack { get; set; }

        public SerializableMap<string, object> RawVarValues { get; set; }


        public RuntimeStatusInfo()
        {
            this.RawVarValues = new SerializableMap<string, object>(Constants.DefaultRuntimeSize);
            this.RawCallStack = new CallStack();
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