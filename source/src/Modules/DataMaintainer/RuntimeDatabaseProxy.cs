﻿using System.Collections.Generic;
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

        public override TestInstanceData GetTestInstanceData(string runtimeHash)
        {
            throw new System.InvalidOperationException();
        }

        public override IList<TestInstanceData> GetTestInstanceDatas(string filterString)
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

        public override IList<SequenceResultData> GetSequenceResultDatas(string runtimeHash, int sessionId)
        {
            throw new System.InvalidOperationException();
        }

        public override SequenceResultData GetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex)
        {
            throw new System.InvalidOperationException();
        }
    }
}