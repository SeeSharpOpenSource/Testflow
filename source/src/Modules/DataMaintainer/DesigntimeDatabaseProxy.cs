using System.Collections.Generic;
using System.Data.Common;
using Testflow.Modules;
using Testflow.Runtime.Data;

namespace Testflow.DataMaintainer
{
    internal class DesigntimeDatabaseProxy : DatabaseProxy
    {
        public DesigntimeDatabaseProxy(IModuleConfigData configData) : base(configData, false)
        {
        }

        public override int GetTestInstanceCount(string fileterString)
        {
            string cmd = SqlCommandFactory.CreateCalcCountCmd(string.Empty, DataBaseItemNames.InstanceTableName);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                bool read = dataReader.Read();
                return (int)dataReader[0];
            }
        }

        public override TestInstanceData GetTestInstanceData(string runtimeHash)
        {
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}'";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.InstanceTableName);
            DbDataReader dataReader = ExecuteReadCommand(cmd);
            TestInstanceData instanceData = new TestInstanceData();
            
        }

        public override IList<TestInstanceData> GetTestInstanceDatas(string filterString)
        {
            throw new System.NotImplementedException();
        }

        public override void AddData(TestInstanceData testInstance)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateData(TestInstanceData testInstance)
        {
            throw new System.NotImplementedException();
        }

        public override void DeleteTestInstance(string fileterString)
        {
            throw new System.NotImplementedException();
        }

        public override IList<SessionResultData> GetSessionResults(string runtimeHash)
        {
            throw new System.NotImplementedException();
        }

        public override SessionResultData GetSessionResult(string runtimeHash, int sessionId)
        {
            throw new System.NotImplementedException();
        }

        public override void AddData(SessionResultData sessionResult)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateData(SessionResultData sessionResult)
        {
            throw new System.NotImplementedException();
        }

        public override IList<SequenceResultData> GetSequenceResultDatas(string runtimeHash, int sessionId)
        {
            throw new System.NotImplementedException();
        }

        public override SequenceResultData GetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex)
        {
            throw new System.NotImplementedException();
        }

        public override void AddData(SequenceResultData sequenceResult)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateData(SequenceResultData sequenceResult)
        {
            throw new System.NotImplementedException();
        }

        public override void AddData(PerformanceStatus performanceStatus)
        {
            throw new System.NotImplementedException();
        }

        public override void AddData(RuntimeStatusData runtimeStatus)
        {
            throw new System.NotImplementedException();
        }
    }
}