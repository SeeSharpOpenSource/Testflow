using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.MasterCore.Common;
using Testflow.Runtime;
using Testflow.Utility.Collections;

namespace Testflow.MasterCore.EventData
{
    internal class SequenceTestResult : ISequenceTestResult
    {
        public SequenceTestResult(int sessionId, int sequenceIndex)
        {
            this.Properties = new SerializableMap<string, object>(Constants.DefaultRuntimeSize);

            this.SessionId = sessionId;
            this.SequenceIndex = sequenceIndex;
        }

        public int SessionId { get; }
        public int SequenceIndex { get; }
        public RuntimeState ResultState { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double ElapsedTime { get; set; }
        public IPerformanceResult Performance { get; set; }
        public ISequenceFailedInfo FailedInfo { get; set; }
        public ISerializableMap<string, string> VariableValues { get; }

        public void InitExtendProperties()
        {
            // ignore
        }

        public ISerializableMap<string, object> Properties { get; }
        public void SetProperty(string propertyName, object value)
        {
            this.Properties[propertyName] = value;
        }

        public object GetProperty(string propertyName)
        {
            return this.Properties[propertyName];
        }

        public TDataType GetProperty<TDataType>(string propertyName)
        {
            return (TDataType)GetProperty(propertyName);
        }

        public Type GetPropertyType(string propertyName)
        {
            return Properties[propertyName].GetType();
        }

        public bool ContainsProperty(string propertyName)
        {
            return Properties.ContainsKey(propertyName);
        }

        public IList<string> GetPropertyNames()
        {
            return new List<string>(Properties.Keys);
        }
        
    }
}