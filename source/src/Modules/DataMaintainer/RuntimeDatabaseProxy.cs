using System.Collections.Generic;
using Testflow.Modules;
using Testflow.Runtime.Data;

namespace Testflow.DataMaintainer
{
    internal class RuntimeDatabaseProxy : DatabaseProxy
    {
        public RuntimeDatabaseProxy(IModuleConfigData configData) : base(configData, true)
        {
        }

        public override int GetTestInstanceCount(string filterString)
        {
            throw new System.InvalidOperationException();
        }

        public override TestInstanceData GetTestInstance(string runtimeHash)
        {
            throw new System.InvalidOperationException();
        }

        public override IList<TestInstanceData> GetTestInstances(string filterString)
        {
            throw new System.InvalidOperationException();
        }
        
        public override void DeleteTestInstance(string fileterString)
        {
            throw new System.InvalidOperationException();
        }

        public override IList<SessionResultData> GetSessionResults(string runtimeHash)
        {
            throw new System.InvalidOperationException();
        }

        public override SessionResultData GetSessionResult(string runtimeHash, int sessionId)
        {
            throw new System.InvalidOperationException();
        }

        public override IList<SequenceResultData> GetSequenceResults(string runtimeHash, int sessionId)
        {
            throw new System.InvalidOperationException();
        }

        public override SequenceResultData GetSequenceResult(string runtimeHash, int sessionId, int sequenceIndex)
        {
            throw new System.InvalidOperationException();
        }

        public override IList<PerformanceStatus> GetPerformanceStatus(string runtimeHash, int session)
        {
            throw new System.InvalidOperationException();
        }

        public override PerformanceStatus GetPerformanceStatusByIndex(string runtimeHash, long index)
        {
            throw new System.InvalidOperationException();
        }

        public override IList<RuntimeStatusData> GetRuntimeStatus(string runtimeHash, int session)
        {
            throw new System.InvalidOperationException();
        }

        public override RuntimeStatusData GetRuntimeStatusByIndex(string runtimeHash, long index)
        {
            throw new System.InvalidOperationException();
        }
    }
}