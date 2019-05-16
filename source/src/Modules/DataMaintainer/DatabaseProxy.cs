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

        public abstract int GetTestInstanceCount(string fileterString);
        public abstract TestInstanceData GetTestInstanceData(string runtimeHash);
        public abstract IList<TestInstanceData> GetTestInstanceDatas(string filterString);
        public abstract void AddData(TestInstanceData testInstance);
        public abstract void UpdateData(TestInstanceData testInstance);
        public abstract void DeleteTestInstance(string fileterString);
        public abstract IList<SessionResultData> GetSessionResults(string runtimeHash);
        public abstract SessionResultData GetSessionResult(string runtimeHash, int sessionId);
        public abstract void AddData(SessionResultData sessionResult);
        public abstract void UpdateData(SessionResultData sessionResult);
        public abstract IList<SequenceResultData> GetSequenceResultDatas(string runtimeHash, int sessionId);
        public abstract SequenceResultData GetSequenceResultData(string runtimeHash, int sessionId, int sequenceIndex);
        public abstract void AddData(SequenceResultData sequenceResult);
        public abstract void UpdateData(SequenceResultData sequenceResult);
        public abstract void AddData(PerformanceStatus performanceStatus);
        public abstract void AddData(RuntimeStatusData runtimeStatus);

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