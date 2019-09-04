using System.Collections.Generic;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.Modules
{
    /// <summary>
    /// 数据持久化模块
    /// </summary>
    public interface IDataMaintainer : IController
    {
        #region Result maintain

        /// <summary>
        /// 返回所有符合过滤字符串的TestInstance的条目数
        /// </summary>
        int GetTestInstanceCount(string fileterString);

        /// <summary>
        /// 获取指定运行时Hash的TestInstance数据
        /// </summary>
        TestInstanceData GetTestInstance(string runtimeHash);

        /// <summary>
        /// 返回所有符合过滤字符串的TestInstance条目
        /// </summary>
        IList<TestInstanceData> GetTestInstances(string filterString);

        /// <summary>
        /// 记录TestInstanceData
        /// </summary>
        void AddData(TestInstanceData testInstance);

        /// <summary>
        /// 更新TestInstanceData
        /// </summary>
        void UpdateData(TestInstanceData testInstance);

        /// <summary>
        /// 使用指定的过滤语句删除TestInstance
        /// </summary>
        void DeleteTestInstance(string fileterString);

        /// <summary>
        /// 获取某个运行实例的所有会话结果
        /// </summary>
        IList<SessionResultData> GetSessionResults(string runtimeHash);

        /// <summary>
        /// 获取某个运行实例的某个会话结果
        /// </summary>
        SessionResultData GetSessionResult(string runtimeHash, int sessionId);

        /// <summary>
        /// 记录SessionResultData
        /// </summary>
        void AddData(SessionResultData sessionResult);

        /// <summary>
        /// 更新SessionResultData
        /// </summary>
        void UpdateData(SessionResultData sessionResult);

        /// <summary>
        /// 获取某个运行实例的某个会话的所有序列执行结果
        /// </summary>
        IList<SequenceResultData> GetSequenceResults(string runtimeHash, int sessionId);

        /// <summary>
        /// 获取某个运行实例的某个会话的某个序列的执行结果
        /// </summary>
        SequenceResultData GetSequenceResult(string runtimeHash, int sessionId, int sequenceIndex);

        /// <summary>
        /// 记录SequenceResultData
        /// </summary>
        void AddData(SequenceResultData sequenceResult);

        /// <summary>
        /// 更新SequenceResultData
        /// </summary>
        void UpdateData(SequenceResultData sequenceResult);

        /// <summary>
        /// 获取性能结果
        /// </summary>
        /// <param name="session">会话ID</param>
        /// <param name="performanceResult">性能结果</param>
        /// <param name="runtimeHash">实例哈希值</param>
        void GetPerformanceResult(string runtimeHash, int session, IPerformanceResult performanceResult);

        #endregion

        #region Middle status maintain

        /// <summary>
        /// 记录PerformanceStatus
        /// </summary>
        void AddData(PerformanceStatus performanceStatus);

        /// <summary>
        /// 记录RuntimeStatusData
        /// </summary>
        void AddData(RuntimeStatusData runtimeStatus);

        /// <summary>
        /// 获取某个会话的所有性能状态信息
        /// </summary>
        IList<PerformanceStatus> GetPerformanceStatus(string runtimeHash, int session);

        /// <summary>
        /// 获取某个会话的某个ID的性能数据
        /// </summary>
        PerformanceStatus GetPerformanceStatusByIndex(string runtimeHash, long index);

        /// <summary>
        /// 获取某个会话的运行时信息
        /// </summary>
        IList<RuntimeStatusData> GetRuntimeStatus(string runtimeHash, int session);

        /// <summary>
        /// 获取某个会话某个索引的运行时信息
        /// </summary>
        RuntimeStatusData GetRuntimeStatusByIndex(string runtimeHash, long index);

        /// <summary>
        /// 获取某个序列执行过程中是否存在失败的step
        /// </summary>
        bool ExistFailedStep(string runtimeHash, int session, int sequence);

        #endregion
    }
}