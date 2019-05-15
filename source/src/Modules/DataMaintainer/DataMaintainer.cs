using System.Collections.Generic;
using Testflow.Modules;
using Testflow.Runtime.Data;

namespace Testflow.DataMaintainer
{
    public class DataMaintainer : IDataMaintainer
    {
        public IModuleConfigData ConfigData { get; set; }

        public void RuntimeInitialize()
        {
            throw new System.NotImplementedException();
        }

        public void DesigntimeInitialize()
        {
            throw new System.NotImplementedException();
        }

        public void ApplyConfig(IModuleConfigData configData)
        {
            throw new System.NotImplementedException();
        }

        public int GetTestInstanceCount(string fileterString)
        {
            throw new System.NotImplementedException();
        }

        public TestInstanceData GetTestInstanceData(string runtimeHash)
        {
            throw new System.NotImplementedException();
        }

        public IList<TestInstanceData> GetTestInstanceDatas(string filterString)
        {
            throw new System.NotImplementedException();
        }

        public void AddData(TestInstanceData testInstance)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateData(TestInstanceData testInstance)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteTestInstance(string fileterString)
        {
            throw new System.NotImplementedException();
        }

        public IList<SessionResultData> GetSessionResults(string runtimeHash)
        {
            throw new System.NotImplementedException();
        }

        public SessionResultData GetSessionResult(string runtimeHash, int sessionId)
        {
            throw new System.NotImplementedException();
        }

        public void AddData(SessionResultData sessionResult)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateData(SessionResultData sessionResult)
        {
            throw new System.NotImplementedException();
        }

        public IList<SequenceResultData> GetSequenceResultDatas(string runtimeHash, int sessionId)
        {
            throw new System.NotImplementedException();
        }

        public SequenceResultData GetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex)
        {
            throw new System.NotImplementedException();
        }

        public void AddData(SequenceResultData sequenceResult)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateData(SequenceResultData sequenceResult)
        {
            throw new System.NotImplementedException();
        }

        public void AddData(PerformanceStatus performanceStatus)
        {
            throw new System.NotImplementedException();
        }

        public void AddData(RuntimeStatusData runtimeStatus)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}