using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.DataMaintainer
{
    internal abstract class DatabaseProxy
    {
        public bool IsRuntimeModule { get; }

        protected readonly ILogService Logger;
        protected DbConnection Connection;
        protected readonly I18N I18N;

        protected readonly IModuleConfigData ConfigData;
        protected DataModelMapper DataModelMapper;

        private ReaderWriterLockSlim _databaseLock;

        protected DatabaseProxy(IModuleConfigData configData, bool isRuntimeModuleModule)
        {
            this.ConfigData = configData;
            IsRuntimeModule = isRuntimeModuleModule;

            I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "i18n_datamaintain_zh", "i18n_datamaintain_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);
            I18N = I18N.GetInstance(Constants.I18nName);
            Logger = TestflowRunner.GetInstance().LogService;
            try
            {
                // 使用DbProviderFactory方式连接需要在App.Config文件中定义DbProviderFactories节点
                // 但是App.Config文件只在入口Assembly中时才会被默认加载，所以目前写死为SqlConnection
//                DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");
//                Connection = factory.CreateConnection();
//                if (null == Connection)
//                {
//                    Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, "Connect db failed.");
//                    throw new TestflowRuntimeException(ModuleErrorCode.ConnectDbFailed, I18N.GetStr("ConnectDbFailed"));
//                }
                InitializeDatabaseAndConnection();
            }
            catch (DbException ex)
            {
                Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Connect db failed.");
                throw new TestflowRuntimeException(ModuleErrorCode.ConnectDbFailed, I18N.GetStr("ConnectDbFailed"), ex);
            }

            this._databaseLock = new ReaderWriterLockSlim();

            DataModelMapper = new DataModelMapper();
        }

        public virtual long GetTestInstanceCount(string filterString)
        {
            string cmd = SqlCommandFactory.CreateCalcCountCmd(filterString, DataBaseItemNames.InstanceTableName);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                long count = 0;
                if (dataReader.Read() && !dataReader.IsDBNull(0))
                {
                    count = dataReader.GetInt64(0);
                }
                return count;
            }
        }

        public virtual TestInstanceData GetTestInstance(string runtimeHash)
        {
            return InternalGetTestInstanceData(runtimeHash);
        }

        protected TestInstanceData InternalGetTestInstanceData(string runtimeHash)
        {
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}'";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.InstanceTableName);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
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
        }

        public virtual IList<TestInstanceData> GetTestInstances(string filterString)
        {
            string cmd = SqlCommandFactory.CreateQueryCmd(filterString, DataBaseItemNames.InstanceTableName);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                List<TestInstanceData> testInstanceDatas = new List<TestInstanceData>(50);
                while (dataReader.Read())
                {
                    TestInstanceData instanceData = new TestInstanceData();
                    DataModelMapper.ReadToObject(dataReader, instanceData);
                    testInstanceDatas.Add(instanceData);
                }
                return testInstanceDatas;
            }
        }

        public virtual void AddData(TestInstanceData testInstance)
        {
            Dictionary<string, string> columnToValue = DataModelMapper.GetColumnValueMapping(testInstance);
            string cmd = SqlCommandFactory.CreateInsertCmd(DataBaseItemNames.InstanceTableName, columnToValue);
            ExecuteWriteCommand(cmd);
        }

        public virtual void UpdateData(TestInstanceData testInstance)
        {
            // 获取原数据，转换为键值对类型
            TestInstanceData lastInstanceData = InternalGetTestInstanceData(testInstance.RuntimeHash);
            Dictionary<string, string> lastInstanceValues = DataModelMapper.GetColumnValueMapping(lastInstanceData);

            Dictionary<string, string> columnToValue = DataModelMapper.GetColumnValueMapping(testInstance);
            // 比较并创建更新命令
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{testInstance.RuntimeHash}'";
            string cmd = SqlCommandFactory.CreateUpdateCmd(DataBaseItemNames.InstanceTableName, lastInstanceValues,
                columnToValue, filter);
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return;
            }
            ExecuteWriteCommand(cmd);
        }

        public virtual void DeleteTestInstance(string fileterString)
        {
            DbTransaction transaction = null;
            try
            {
                // 删除TestInstance需要执行事务流程
                transaction = Connection.BeginTransaction(IsolationLevel.Serializable);

                string deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.StatusTableName, fileterString);
                ExecuteWriteCommand(deleteCmd, transaction);

                deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.PerformanceTableName, fileterString);
                ExecuteWriteCommand(deleteCmd, transaction);

                deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.SequenceTableName, fileterString);
                ExecuteWriteCommand(deleteCmd, transaction);

                deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.SessionTableName, fileterString);
                ExecuteWriteCommand(deleteCmd, transaction);

                deleteCmd = SqlCommandFactory.CreateDeleteCmd(DataBaseItemNames.InstanceTableName, fileterString);
                ExecuteWriteCommand(deleteCmd, transaction);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction?.Rollback();
                throw;
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public virtual IList<SessionResultData> GetSessionResults(string runtimeHash)
        {
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}'";
            List<SessionResultData> resultDatas = new List<SessionResultData>(10);

            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.SessionTableName);

            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                while (dataReader.Read())
                {
                    SessionResultData sessionResultData = new SessionResultData();
                    DataModelMapper.ReadToObject(dataReader, sessionResultData);
                    resultDatas.Add(sessionResultData);
                }
                return resultDatas;
            }
        }

        public virtual SessionResultData GetSessionResult(string runtimeHash, int sessionId)
        {
            return InternalGetSessionResult(runtimeHash, sessionId);
        }

        protected SessionResultData InternalGetSessionResult(string runtimeHash, int sessionId)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={sessionId}";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.SessionTableName);

            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                SessionResultData sessionResultData = null;
                if (dataReader.Read())
                {
                    sessionResultData = new SessionResultData();
                    DataModelMapper.ReadToObject(dataReader, sessionResultData);
                }
                return sessionResultData;
            }
        }

        public virtual void AddData(SessionResultData sessionResult)
        {
            Dictionary<string, string> columnToValue = DataModelMapper.GetColumnValueMapping(sessionResult);
            string cmd = SqlCommandFactory.CreateInsertCmd(DataBaseItemNames.SessionTableName, columnToValue);
            ExecuteWriteCommand(cmd);
        }

        public virtual void UpdateData(SessionResultData sessionResult)
        {
            SessionResultData lastSessionResult = InternalGetSessionResult(sessionResult.RuntimeHash, sessionResult.Session);
            Dictionary<string, string> lastSessionValues = DataModelMapper.GetColumnValueMapping(lastSessionResult);

            Dictionary<string, string> columnToValue = DataModelMapper.GetColumnValueMapping(sessionResult);

            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{sessionResult.RuntimeHash}' AND {DataBaseItemNames.SessionIdColumn}={sessionResult.Session}";
            string cmd = SqlCommandFactory.CreateUpdateCmd(DataBaseItemNames.SessionTableName, lastSessionValues,
                columnToValue, filter);
            ExecuteWriteCommand(cmd);
        }

        public virtual IList<SequenceResultData> GetSequenceResults(string runtimeHash, int sessionId)
        {
            string filter = $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={sessionId}";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.SequenceTableName);

            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                List<SequenceResultData> resultDatas = new List<SequenceResultData>(10);
                while (dataReader.Read())
                {
                    SequenceResultData sequenceResultData = new SequenceResultData();
                    DataModelMapper.ReadToObject(dataReader, sequenceResultData);
                    resultDatas.Add(sequenceResultData);
                }
                return resultDatas;
            }
        }

        public virtual SequenceResultData GetSequenceResult(string runtimeHash, int sessionId, int sequenceIndex)
        {
            return InternalGetSequenceResultData(runtimeHash, sessionId, sequenceIndex);
        }

        public virtual void GetPerformanceResult(string runtimeHash, int session, IPerformanceResult performance)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={session}";
            // 最高CPU时间
            string cmd = SqlCommandFactory.CreateQueryMaxCmd(DataBaseItemNames.PerformanceTableName,
                DataBaseItemNames.ProcessorTimeColumn, filter);
            using (DbDataReader reader = ExecuteReadCommand(cmd))
            {
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    performance.CpuTime = (ulong) reader.GetDouble(0);
                }
            }
            // 最高分配内存
            cmd = SqlCommandFactory.CreateQueryMaxCmd(DataBaseItemNames.PerformanceTableName,
                DataBaseItemNames.MemoryAllocatedColumn, filter);
            using (DbDataReader reader = ExecuteReadCommand(cmd))
            {
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    performance.MaxAllocatedMemory = reader.GetInt64(0);
                }
            }
            // 最高使用内存
            cmd = SqlCommandFactory.CreateQueryMaxCmd(DataBaseItemNames.PerformanceTableName,
                DataBaseItemNames.MemoryUsedColumn, filter);
            using (DbDataReader reader = ExecuteReadCommand(cmd))
            {
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    performance.MaxUsedMemory = reader.GetInt64(0);
                }
            }
            // 平均分配内存
            cmd = SqlCommandFactory.CreateQueryAverageCmd(DataBaseItemNames.PerformanceTableName,
                DataBaseItemNames.MemoryAllocatedColumn, filter);
            using (DbDataReader reader = ExecuteReadCommand(cmd))
            {
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    performance.AverageAllocatedMemory = (long) reader.GetDouble(0);
                }
            }
            // 平均使用内存
            cmd = SqlCommandFactory.CreateQueryAverageCmd(DataBaseItemNames.PerformanceTableName,
                DataBaseItemNames.MemoryUsedColumn, filter);
            using (DbDataReader reader = ExecuteReadCommand(cmd))
            {
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    performance.AverageUsedMemory = (long) reader.GetDouble(0);
                }
            }
        }

        protected SequenceResultData InternalGetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={sessionId} AND " +
                $"{DataBaseItemNames.SequenceIndexColumn}={sequenceIndex}";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.SequenceTableName);

            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                SequenceResultData sequenceResultData = null;
                if (dataReader.Read())
                {
                    sequenceResultData = new SequenceResultData();
                    DataModelMapper.ReadToObject(dataReader, sequenceResultData);
                }
                return sequenceResultData;
            }
        }

        public virtual void AddData(SequenceResultData sequenceResult)
        {
            Dictionary<string, string> columnToValue = DataModelMapper.GetColumnValueMapping(sequenceResult);
            string cmd = SqlCommandFactory.CreateInsertCmd(DataBaseItemNames.SequenceTableName, columnToValue);
            ExecuteWriteCommand(cmd);
        }

        public virtual void UpdateData(SequenceResultData sequenceResult)
        {
            SequenceResultData lastSequenceResult = InternalGetSequenceResultData(sequenceResult.RuntimeHash,
                sequenceResult.Session, sequenceResult.SequenceIndex);


            Dictionary<string, string> lastSequenceValues = DataModelMapper.GetColumnValueMapping(lastSequenceResult);

            Dictionary<string, string> columnToValue = DataModelMapper.GetColumnValueMapping(sequenceResult);

            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{sequenceResult.RuntimeHash}' AND {DataBaseItemNames.SessionIdColumn}={sequenceResult.Session} AND {DataBaseItemNames.SequenceIndexColumn}={sequenceResult.SequenceIndex}";

            string cmd = SqlCommandFactory.CreateUpdateCmd(DataBaseItemNames.SequenceTableName, lastSequenceValues,
                columnToValue, filter);
            ExecuteWriteCommand(cmd);
        }

        public virtual void AddData(PerformanceStatus performanceStatus)
        {
            Dictionary<string, string> columnToValue = DataModelMapper.GetColumnValueMapping(performanceStatus);
            string cmd = SqlCommandFactory.CreateInsertCmd(DataBaseItemNames.PerformanceTableName, columnToValue);
            ExecuteWriteCommand(cmd);
        }

        public virtual void AddData(RuntimeStatusData runtimeStatus)
        {
            Dictionary<string, string> columnToValue = DataModelMapper.GetColumnValueMapping(runtimeStatus);
            string cmd = SqlCommandFactory.CreateInsertCmd(DataBaseItemNames.StatusTableName, columnToValue);
            ExecuteWriteCommand(cmd);
        }

        public virtual IList<PerformanceStatus> GetPerformanceStatus(string runtimeHash, int session)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={session}";
            string cmd = SqlCommandFactory.CreateQueryCmdWithOrder(filter, DataBaseItemNames.PerformanceTableName, 
                DataBaseItemNames.StatusIndexColumn);
            List<PerformanceStatus> performanceStatuses = new List<PerformanceStatus>(500);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                while (dataReader.Read())
                {
                    PerformanceStatus performanceStatus = new PerformanceStatus();
                    DataModelMapper.ReadToObject(dataReader, performanceStatus);
                    performanceStatuses.Add(performanceStatus);
                }
            }
            return performanceStatuses;
        }

        public virtual PerformanceStatus GetPerformanceStatusByIndex(string runtimeHash, long index)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.StatusIndexColumn}={index}";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.PerformanceTableName);
            PerformanceStatus performanceStatus = null;
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                if (dataReader.Read())
                {
                    performanceStatus = new PerformanceStatus();
                    DataModelMapper.ReadToObject(dataReader, performanceStatus);
                }
            }
            return performanceStatus;
        }

        public virtual IList<RuntimeStatusData> GetRuntimeStatus(string runtimeHash, int session)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={session}";
            string cmd = SqlCommandFactory.CreateQueryCmdWithOrder(filter, DataBaseItemNames.StatusTableName, 
                DataBaseItemNames.StatusIndexColumn);
            List<RuntimeStatusData> statusDatas = new List<RuntimeStatusData>(500);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                while (dataReader.Read())
                {
                    RuntimeStatusData runtimeStatusData = new RuntimeStatusData();
                    DataModelMapper.ReadToObject(dataReader, runtimeStatusData);
                    statusDatas.Add(runtimeStatusData);
                }
            }
            return statusDatas;
        }

        public IList<RuntimeStatusData> GetRuntimeStatus(string runtimeHash, int session, int sequenceIndex)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={session} AND {DataBaseItemNames.SequenceIndexColumn}={sequenceIndex}";
            string cmd = SqlCommandFactory.CreateQueryCmdWithOrder(filter, DataBaseItemNames.StatusTableName,
                DataBaseItemNames.StatusIndexColumn);
            List<RuntimeStatusData> statusDatas = new List<RuntimeStatusData>(500);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                while (dataReader.Read())
                {
                    RuntimeStatusData runtimeStatusData = new RuntimeStatusData();
                    DataModelMapper.ReadToObject(dataReader, runtimeStatusData);
                    statusDatas.Add(runtimeStatusData);
                }
            }
            return statusDatas;
        }

        public virtual RuntimeStatusData GetRuntimeStatusByIndex(string runtimeHash, long index)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.StatusIndexColumn}={index}";
            string cmd = SqlCommandFactory.CreateQueryCmd(filter, DataBaseItemNames.StatusTableName);
            RuntimeStatusData statusData = null;
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                while (dataReader.Read())
                {
                    statusData = new RuntimeStatusData();
                    DataModelMapper.ReadToObject(dataReader, statusData);
                }
            }
            return statusData;
        }

        public IList<RuntimeStatusData> GetRuntimeStatusInRange(string runtimeHash, int session, long startIndex, long count)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={session} AND {DataBaseItemNames.StatusIndexColumn} >= {startIndex}";
            string cmd = SqlCommandFactory.CreateQueryCmdWithOrder(filter, DataBaseItemNames.StatusTableName, DataBaseItemNames.StatusIndexColumn);
            if (count > 0)
            {
                cmd += $" LIMIT {count}";
            }
            List<RuntimeStatusData> statusDatas = new List<RuntimeStatusData>(500);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                while (dataReader.Read())
                {
                    RuntimeStatusData runtimeStatusData = new RuntimeStatusData();
                    DataModelMapper.ReadToObject(dataReader, runtimeStatusData);
                    statusDatas.Add(runtimeStatusData);
                }
            }
            statusDatas.TrimExcess();
            return statusDatas;
        }

        public long GetRuntimeStatusCount(string runtimeHash, int session, int sequenceIndex)
        {
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={session} AND {DataBaseItemNames.SequenceIndexColumn}={sequenceIndex}";
            string cmd = SqlCommandFactory.CreateCalcCountCmd(filter, DataBaseItemNames.StatusTableName);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                long count = 0;
                if (dataReader.Read() && !dataReader.IsDBNull(0))
                {
                    count = dataReader.GetInt64(0);
                }
                return count;
            }
        }

        public bool ExistFailedStep(string runtimeHash, int session, int sequence)
        {
            const string failedResultFilter = "StepResult IN ('Error', 'Timeout', 'Failed', 'Abort')";
            string filter =
                $"{DataBaseItemNames.RuntimeIdColumn}='{runtimeHash}' AND {DataBaseItemNames.SessionIdColumn}={session} AND {DataBaseItemNames.SequenceIndexColumn} = {sequence} AND {failedResultFilter}";
            string cmd = SqlCommandFactory.CreateCalcCountCmd(filter, DataBaseItemNames.StatusTableName);
            using (DbDataReader dataReader = ExecuteReadCommand(cmd))
            {
                int count = 0;
                if (dataReader.Read() && !dataReader.IsDBNull(0))
                {
                    count = dataReader.GetInt32(0);
                }
                return count > 0;
            }
        }

        private DbDataReader ExecuteReadCommand(string command, DbTransaction transaction = null)
        {
            bool getLock = false;
            try
            {
                getLock = _databaseLock.TryEnterReadLock(Constants.BlockTimeout);
                if (!getLock)
                {
                    Logger.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                        $"Data base operation timeout. Cmd:<{command}>");
                    throw new TestflowRuntimeException(ModuleErrorCode.DbOperationTimeout, I18N.GetStr("DbOperationTimeout"));
                }
                DbCommand dbCommand = Connection.CreateCommand();
                dbCommand.CommandText = command;
                dbCommand.CommandTimeout = Constants.CommandTimeout;
                if (null != transaction)
                {
                    dbCommand.Transaction = transaction;
                }
                return dbCommand.ExecuteReader();
            }
            catch (DbException ex)
            {
                Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Database operation failed.");
                throw new TestflowRuntimeException(ModuleErrorCode.DbOperationFailed, I18N.GetStr("DbOperationFailed"), ex);
            }
            finally
            {
                if (getLock)
                {
                    _databaseLock.ExitReadLock();
                }
            } 
        }

        private void ExecuteWriteCommand(string command, DbTransaction transaction = null)
        {
            bool getLock = false;
            try
            {
                getLock = _databaseLock.TryEnterWriteLock(Constants.BlockTimeout);
                if (!getLock)
                {
                    Logger.Print(LogLevel.Error, CommonConst.PlatformLogSession,
                        $"Data base operation timeout. Cmd:<{command}>");
                    throw new TestflowRuntimeException(ModuleErrorCode.DbOperationTimeout, I18N.GetStr("DbOperationTimeout"));
                }
                DbCommand dbCommand = Connection.CreateCommand();
                dbCommand.CommandText = command;
                dbCommand.CommandTimeout = Constants.CommandTimeout;
                if (null != transaction)
                {
                    dbCommand.Transaction = transaction;
                }
                dbCommand.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Database operation failed.");
                throw new TestflowRuntimeException(ModuleErrorCode.DbOperationFailed, I18N.GetStr("DbOperationFailed"), ex);
            }
            finally
            {
                if (getLock)
                {
                    _databaseLock.ExitWriteLock();
                }
            } 
        }

        private void InitializeDatabaseAndConnection()
        {
            string testflowHome = ConfigData.GetProperty<string>("TestflowHome");
            string databaseFilePath = ConfigData.GetProperty<string>("DatabaseName");
            // 使用DbProviderFactory方式连接需要在App.Config文件中定义DbProviderFactories节点
            // 但是App.Config文件只在入口Assembly中时才会被默认加载，所以目前写死为SqlConnection
//            Connection.ConnectionString = $"Data Source={databaseFilePath}";

            // 如果已经存在则直接跳出
            if (File.Exists(databaseFilePath))
            {
                Connection = new SQLiteConnection($"Data Source={databaseFilePath};Synchronous=OFF;");
                Connection.Open();
                return;
            }
            Connection = new SQLiteConnection($"Data Source={databaseFilePath};Synchronous=OFF;");
            DbTransaction transaction = null;
            try
            {
                const string endDelim = ";";
                const string commentPrefix = "--";
                Connection.Open();
                string sqlFilePath =
                    $"{testflowHome}{CommonConst.DeployDir}{Path.DirectorySeparatorChar}{Constants.SqlFileName}";
                using (StreamReader reader = new StreamReader(sqlFilePath, Encoding.UTF8))
                {
                    StringBuilder createTableCmd = new StringBuilder(500);
                    string lineData;
                    transaction = Connection.BeginTransaction(IsolationLevel.Serializable);
                    while (null != (lineData = reader.ReadLine()))
                    {
                        lineData = lineData.Trim();
                        if (lineData.StartsWith(commentPrefix))
                        {
                            continue;
                        }
                        createTableCmd.Append(lineData);
                        if (lineData.EndsWith(endDelim))
                        {
                            DbCommand dbCommand = Connection.CreateCommand();
                            dbCommand.CommandText = createTableCmd.ToString();
                            dbCommand.Transaction = transaction;
                            dbCommand.ExecuteNonQuery();
                            createTableCmd.Clear();
                        }
                    }
                    transaction.Commit();
                    transaction.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Create database failed.");
                transaction?.Rollback();
                transaction?.Dispose();
                Connection?.Dispose();
                // 如果失败则删除文件
                File.Delete(databaseFilePath);
                throw;
            }
        }

        public void Dispose()
        {
            Connection?.Close();
            _databaseLock?.Dispose();
        }

        public void RegisterTypeConvertor(Type type, Func<object, string> toStringFunc, Func<string, object> parseFunc)
        {
            DataModelMapper.RegisterTypeConvertor(type, toStringFunc, parseFunc);
        }
    }
}