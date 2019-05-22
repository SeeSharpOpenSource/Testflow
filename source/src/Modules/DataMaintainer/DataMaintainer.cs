using System.Collections.Generic;
using System.Threading;
using Testflow.Modules;
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
            // TODO
        }

        public int GetTestInstanceCount(string fileterString)
        {
            return _databaseProxy.GetTestInstanceCount(fileterString);
        }

        public TestInstanceData GetTestInstanceData(string runtimeHash)
        {
            return _databaseProxy.GetTestInstanceData(runtimeHash);
        }

        public IList<TestInstanceData> GetTestInstanceDatas(string filterString)
        {
            return _databaseProxy.GetTestInstanceDatas(filterString);
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

        public IList<SequenceResultData> GetSequenceResultDatas(string runtimeHash, int sessionId)
        {
            return _databaseProxy.GetSequenceResultDatas(runtimeHash, sessionId);
        }

        public SequenceResultData GetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex)
        {
            return _databaseProxy.GetSequenceResultData(runtimeHash, sessionId, sequenceIndex);
        }

        public void AddData(SequenceResultData sequenceResult)
        {
            _databaseProxy.AddData(sequenceResult);
        }

        public void UpdateData(SequenceResultData sequenceResult)
        {
            _databaseProxy.UpdateData(sequenceResult);
        }

        public void AddData(PerformanceStatus performanceStatus)
        {
            _databaseProxy.AddData(performanceStatus);
        }

        public void AddData(RuntimeStatusData runtimeStatus)
        {
            _databaseProxy.AddData(runtimeStatus);
        }

        public void Dispose()
        {
            _databaseProxy?.Dispose();
        }
    }
}