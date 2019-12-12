using System;
using System.Collections.Generic;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.StatusManage;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Utility.Collections;

namespace Testflow.MasterCore.EventData
{
    internal class RuntimeStatusInfo : IRuntimeStatusInfo
    {
        public RuntimeStatusInfo(SessionStateHandle stateHandle, ulong statusIndex, Dictionary<int, string> failedInfos, 
            IDictionary<IVariable, string> watchDatas, PerformanceData performance, 
            Dictionary<ICallStack, StepResult> stepResults)
        {
            this.Properties = new SerializableMap<string, object>(Constants.DefaultRuntimeSize);
            this.SessionId = stateHandle.Session;

            this.CallStacks = new List<ICallStack>(stateHandle.SequenceCount);
            this.SequenceState = new List<RuntimeState>(stateHandle.SequenceCount);

            this.StatusIndex = statusIndex;
            this.StartGenTime = stateHandle.StartGenTime;
            this.EndGenTime = stateHandle.EndGenTime;
            this.StartTime = stateHandle.StartTime;
            this.ElapsedTime = stateHandle.ElapsedTime;
            this.CurrentTime = stateHandle.CurrentTime;
            if (null != performance)
            {
                this.MemoryUsed = performance.MemoryUsed;
                this.MemoryAllocated = performance.MemoryAllocated;
                this.ProcessorTime = performance.ProcessorTime;
            }
            this.State = stateHandle.State;

            foreach (int sequenceIndex in stateHandle.SequenceIndexes)
            {
                CallStacks.Add(stateHandle[sequenceIndex].RunStack);
                SequenceState.Add(stateHandle[sequenceIndex].State);
            }
            if (null != failedInfos)
            {
                this.FailedInfos = new Dictionary<int, IFailedInfo>(failedInfos.Count);
                foreach (KeyValuePair<int, string> keyValuePair in failedInfos)
                {
                    this.FailedInfos.Add(keyValuePair.Key, new FailedInfo(keyValuePair.Value));
                }
            }
            StepResults = stepResults;
            this.WatchDatas = watchDatas;
        }

        public int SessionId { get; }
        public ulong StatusIndex { get; set; }
        public DateTime StartGenTime { get; set; }
        public DateTime EndGenTime { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public DateTime CurrentTime { get; set; }
        public long MemoryUsed { get; set; }
        public long MemoryAllocated { get; set; }
        public double ProcessorTime { get; set; }
        public RuntimeState State { get; set; }
        public IList<ICallStack> CallStacks { get; }
        public IList<RuntimeState> SequenceState { get; }
        public IDictionary<int, IFailedInfo> FailedInfos { get; set; }
        public IDictionary<ICallStack, StepResult> StepResults { get; set; }
        public IDictionary<IVariable, string> WatchDatas { get; set; }

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
            return (TDataType) GetProperty(propertyName);
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