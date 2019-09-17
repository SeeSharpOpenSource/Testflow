using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.DataMaintainer
{
    public class DataMaintainer : IDataMaintainer
    {
        public IModuleConfigData ConfigData { get; set; }

        private DatabaseProxy _databaseProxy;

        public void RuntimeInitialize()
        {
            if (null != _databaseProxy && _databaseProxy.IsRuntimeModule)
            {
                return;
            }
            _databaseProxy?.Dispose();
            _databaseProxy = null;
            Thread.MemoryBarrier();
            _databaseProxy = new RuntimeDatabaseProxy(ConfigData);
        }

        public void DesigntimeInitialize()
        {
            if (null != _databaseProxy && !_databaseProxy.IsRuntimeModule)
            {
                return;
            }
            _databaseProxy?.Dispose();
            _databaseProxy = null;
            Thread.MemoryBarrier();
            _databaseProxy = new DesigntimeDatabaseProxy(ConfigData);
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            this.ConfigData = configData;
        }

        public int GetTestInstanceCount(string fileterString)
        {
            return _databaseProxy.GetTestInstanceCount(fileterString);
        }

        public TestInstanceData GetTestInstance(string runtimeHash)
        {
            return _databaseProxy.GetTestInstance(runtimeHash);
        }

        public IList<TestInstanceData> GetTestInstances(string filterString)
        {
            return _databaseProxy.GetTestInstances(filterString);
        }

        public void AddData(TestInstanceData testInstance)
        {
            _databaseProxy.AddData(testInstance);
        }

        public void UpdateData(TestInstanceData testInstance)
        {
            _databaseProxy.UpdateData(testInstance);
        }

        public void DeleteTestInstance(string fileterString)
        {
            _databaseProxy.DeleteTestInstance(fileterString);
        }

        public IList<SessionResultData> GetSessionResults(string runtimeHash)
        {
            return _databaseProxy.GetSessionResults(runtimeHash);
        }

        public SessionResultData GetSessionResult(string runtimeHash, int sessionId)
        {
            return _databaseProxy.GetSessionResult(runtimeHash, sessionId);
        }

        public void AddData(SessionResultData sessionResult)
        {
            _databaseProxy.AddData(sessionResult);
        }

        public void UpdateData(SessionResultData sessionResult)
        {
            _databaseProxy.UpdateData(sessionResult);
        }

        public IList<SequenceResultData> GetSequenceResults(string runtimeHash, int sessionId)
        {
            return _databaseProxy.GetSequenceResults(runtimeHash, sessionId);
        }

        public SequenceResultData GetSequenceResult(string runtimeHash, int sessionId, int sequenceIndex)
        {
            return _databaseProxy.GetSequenceResult(runtimeHash, sessionId, sequenceIndex);
        }

        public void AddData(SequenceResultData sequenceResult)
        {
            _databaseProxy.AddData(sequenceResult);
        }

        public void UpdateData(SequenceResultData sequenceResult)
        {
            _databaseProxy.UpdateData(sequenceResult);
        }

        public void GetPerformanceResult(string runtimeHash, int session, IPerformanceResult performanceResult)
        {
            _databaseProxy.GetPerformanceResult(runtimeHash, session, performanceResult);
        }

        public void RegisterTypeConvertor(Type type, Func<object, string> toStringFunc, Func<string, object> parseFunc)
        {
            _databaseProxy.RegisterTypeConvertor(type, toStringFunc, parseFunc);

        }

        public void AddData(PerformanceStatus performanceStatus)
        {
            _databaseProxy.AddData(performanceStatus);
        }

        public void AddData(RuntimeStatusData runtimeStatus)
        {
            _databaseProxy.AddData(runtimeStatus);
        }

        public IList<PerformanceStatus> GetPerformanceStatus(string runtimeHash, int session)
        {
            return _databaseProxy.GetPerformanceStatus(runtimeHash, session);
        }

        public PerformanceStatus GetPerformanceStatusByIndex(string runtimeHash, long index)
        {
            return _databaseProxy.GetPerformanceStatusByIndex(runtimeHash, index);
        }

        public IList<RuntimeStatusData> GetRuntimeStatus(string runtimeHash, int session)
        {
            return _databaseProxy.GetRuntimeStatus(runtimeHash, session);
        }

        public RuntimeStatusData GetRuntimeStatusByIndex(string runtimeHash, long index)
        {
            return _databaseProxy.GetRuntimeStatusByIndex(runtimeHash, index);
        }

        public bool ExistFailedStep(string runtimeHash, int session, int sequence)
        {
            return _databaseProxy.ExistFailedStep(runtimeHash, session, sequence);
        }

        public void Dispose()
        {
            _databaseProxy?.Dispose();
        }
    }
}