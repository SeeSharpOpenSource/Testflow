using System.Collections.Generic;
using Testflow.Usr;
using Testflow.Data.Sequence;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 一个测试序列组的所有测试结果的集合
    /// </summary>
    public interface ITestResultCollection : ISerializableMap<int, ISequenceTestResult>
    {

        /// <summary>
        /// 所在的会话ID
        /// </summary>
        int Session { get; set; }

        /// <summary>
        /// SetUp模块是否成功
        /// </summary>
        bool SetUpSuccess { get; set; }

        /// <summary>
        /// 成功执行的序列个数
        /// </summary>
        int SuccessCount { get; set; }

        /// <summary>
        /// 失败的序列个数
        /// </summary>
        int FailedCount { get; set; }

        /// <summary>
        /// 超时的序列个数
        /// </summary>
        int TimeOutCount { get; set; }

        /// <summary>
        /// 终止的序列个数
        /// </summary>
        int AbortCount { get; set; }

        /// <summary>
        /// TearDown模块是否成功
        /// </summary>
        bool TearDownSuccess { get; set; }

        /// <summary>
        /// 运行是否结束
        /// </summary>
        bool TestOver { get; set; }

        /// <summary>
        /// 性能结果数据
        /// </summary>
        IPerformanceResult Performance { get; set; }

        /// <summary>
        /// 变量的实时取值
        /// </summary>
        IDictionary<IVariable, string> WatchData { get; set; }
    }
}