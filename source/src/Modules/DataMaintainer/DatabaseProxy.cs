using System.Collections.Generic;
using System.Data.Common;
using Testflow.Modules;
using Testflow.Runtime.Data;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.DataMaintainer
{
    internal abstract class DatabaseProxy
    {
        public bool IsRuntimeModule { get; }

        protected readonly ILogService Logger;
        protected readonly DbConnection Connection;
        protected readonly I18N I18N;

        protected readonly IModuleConfigData ConfigData;
        protected DataModelMapper DataModelMapper;

        protected DatabaseProxy(IModuleConfigData configData, bool isRuntimeModuleModule)
        {
            this.ConfigData = configData;
            IsRuntimeModule = isRuntimeModuleModule;

            I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "i18n_datamaintain_zh.resx", "i18n_datamaintain_zh.resx")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            I18N = I18N.GetInstance(Constants.I18nName);
            try
            {
                Logger = TestflowRunner.GetInstance().LogService;
                DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");
                Connection = factory.CreateConnection();
                if (null == Connection)
                {
                    Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, "Connect db failed.");
                    throw new TestflowRuntimeException(ModuleErrorCode.ConnectDbFailed, I18N.GetStr("ConnectDbFailed"));
                }
                Connection.ConnectionString = $"Data Source={Constants.DataBaseName}";
                Connection.Close();
            }
            catch (DbException ex)
            {
                Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Connect db failed.");
                throw new TestflowRuntimeException(ModuleErrorCode.ConnectDbFailed, I18N.GetStr("ConnectDbFailed"), ex);
            }

            DataModelMapper = new DataModelMapper();
        }

        public virtual int GetTestInstanceCount(string fileterString)
        {
            string cmd = SqlCommandFactory.CreateCalcCountCmd(string.Empty, DataBaseItemNames.InstanceTableName);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                bool read = dataReader.Read();
                return (int)dataReader[0];
            }
        }

        public virtual TestInstanceData GetTestInstanceData(string runtimeHash)
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

        public virtual IList<TestInstanceData> GetTestInstanceDatas(string filterString)
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

        public virtual void AddData(TestInstanceData testInstance)
        {
            
        }

        public virtual void UpdateData(TestInstanceData testInstance)
        {
            
        }

        public virtual void DeleteTestInstance(string fileterString)
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

        public virtual IList<SessionResultData> GetSessionResults(string runtimeHash)
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

        public virtual SessionResultData GetSessionResult(string runtimeHash, int sessionId)
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

        public virtual void AddData(SessionResultData sessionResult)
        {
            
        }

        public virtual void UpdateData(SessionResultData sessionResult)
        {
            
        }

        public virtual IList<SequenceResultData> GetSequenceResultDatas(string runtimeHash, int sessionId)
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

        public virtual SequenceResultData GetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex)
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

        public virtual void AddData(SequenceResultData sequenceResult)
        {
            
        }

        public virtual void UpdateData(SequenceResultData sequenceResult)
        {
            
        }

        public virtual void AddData(PerformanceStatus performanceStatus)
        {
            
        }

        public virtual void AddData(RuntimeStatusData runtimeStatus)
        {
            
        }

        protected DbDataReader ExecuteReadCommand(string command)
        {
            try
            {
                DbCommand dbCommand = Connection.CreateCommand();
                dbCommand.CommandText = command;
                dbCommand.CommandTimeout = Constants.CommandTimeout;
                return dbCommand.ExecuteReader();
            }
            catch (DbException ex)
            {
                Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Database operation failed.");
                throw new TestflowRuntimeException(ModuleErrorCode.DbOperationFailed, I18N.GetStr("DbOperationFailed"), ex);
            }
        }

        protected void ExecuteWriteCommand(string command)
        {
            try
            {
                DbCommand dbCommand = Connection.CreateCommand();
                dbCommand.CommandText = command;
                dbCommand.CommandTimeout = Constants.CommandTimeout;
                dbCommand.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Database operation failed.");
                throw new TestflowRuntimeException(ModuleErrorCode.DbOperationFailed, I18N.GetStr("DbOperationFailed"), ex);
            }
        }

        public void Dispose()
        {
            Connection?.Close();
        }
    }
}