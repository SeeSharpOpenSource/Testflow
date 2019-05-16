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
            TestInstanceData instanceData;
            if (dataReader.Read())
            {
                instanceData = new TestInstanceData();
                DataModelMapper.ReadToObject(dataReader, instanceData);
            }
            else
            {
                instanceData = null;
            }
            return instanceData;
        }

        public override IList<TestInstanceData> GetTestInstanceDatas(string filterString)
        {
            string cmd = SqlCommandFactory.CreateQueryCmd(filterString, DataBaseItemNames.InstanceTableName);
            DbDataReader dataReader = ExecuteReadCommand(cmd);
            List<TestInstanceData> testInstanceDatas = new List<TestInstanceData>(50);
            while (dataReader.Read())
            {
                TestInstanceData instanceData = new TestInstanceData();
                DataModelMapper.ReadToObject(dataReader, instanceData);
                testInstanceDatas.Add(instanceData);
            }
            return testInstanceDatas;
        }

        public override void AddData(TestInstanceData testInstance)
        {
            throw new System.InvalidProgramException();
        }

        public override void UpdateData(TestInstanceData testInstance)
        {
            throw new System.InvalidProgramException();
        }

        public override void DeleteTestInstance(string fileterString)
        {
            string deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.StatusTableName, fileterString);
            ExecuteWriteCommand(deleteCmd);

            deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.PerformanceTableName, fileterString);
            ExecuteWriteCommand(deleteCmd);

            deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.SequenceResultColumn, fileterString);
            ExecuteWriteCommand(deleteCmd);

            deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.SessionTableName, fileterString);
            ExecuteWriteCommand(deleteCmd);

            deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.InstanceTableName, fileterString);
            ExecuteWriteCommand(deleteCmd);
        }

        public override IList<SessionResultData> GetSessionResults(string runtimeHash)
        {
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}'";
            List<SessionResultData> resultDatas = new List<SessionResultData>(10);

            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.SessionTableName);

            DbDataReader dataReader = ExecuteReadCommand(cmd);
            while (dataReader.Read())
            {
                SessionResultData sessionResultData = new SessionResultData();
                DataModelMapper.ReadToObject(dataReader, sessionResultData);
                resultDatas.Add(sessionResultData);
            }

            return resultDatas;
        }

        public override SessionResultData GetSessionResult(string runtimeHash, int sessionId)
        {
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={sessionId}";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.SessionTableName);

            DbDataReader dataReader = ExecuteReadCommand(cmd);

            SessionResultData sessionResultData = null;
            if (dataReader.Read())
            {
                sessionResultData = new SessionResultData();
                DataModelMapper.ReadToObject(dataReader, sessionResultData);
            }
            return sessionResultData;
        }

        public override void AddData(SessionResultData sessionResult)
        {
            throw new System.InvalidProgramException();
        }

        public override void UpdateData(SessionResultData sessionResult)
        {
            throw new System.InvalidProgramException();
        }

        public override IList<SequenceResultData> GetSequenceResultDatas(string runtimeHash, int sessionId)
        {
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={sessionId}";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.SequenceTableName);

            DbDataReader dataReader = ExecuteReadCommand(cmd);
            List<SequenceResultData> resultDatas = new List<SequenceResultData>(10);
            while (dataReader.Read())
            {
                SequenceResultData sequenceResultData = new SequenceResultData();
                DataModelMapper.ReadToObject(dataReader, sequenceResultData);
                resultDatas.Add(sequenceResultData);
            }
            return resultDatas;
        }

        public override SequenceResultData GetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex)
        {
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={sessionId} AND " +
                            $"{DataBaseItemNames.SequenceIndexColumn}={sequenceIndex}";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.SequenceTableName);

            DbDataReader dataReader = ExecuteReadCommand(cmd);
            SequenceResultData sequenceResultData = null;
            if (dataReader.Read())
            {
                sequenceResultData = new SequenceResultData();
                DataModelMapper.ReadToObject(dataReader, sequenceResultData);
            }
            return sequenceResultData;
        }

        public override void AddData(SequenceResultData sequenceResult)
        {
            throw new System.InvalidProgramException();
        }

        public override void UpdateData(SequenceResultData sequenceResult)
        {
            throw new System.InvalidProgramException();
        }

        public override void AddData(PerformanceStatus performanceStatus)
        {
            throw new System.InvalidProgramException();
        }

        public override void AddData(RuntimeStatusData runtimeStatus)
        {
            throw new System.InvalidProgramException();
        }
    }
}