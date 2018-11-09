using System.Collections.Generic;

namespace Testflow.Runtime
{
    public interface IStatisticCollection : IDictionary<int, ISynthesizedStatistics>
    {
        /// <summary>
        /// SetUp模块是否成功
        /// </summary>
        bool SetUpSuccess { get; }

        /// <summary>
        /// 成功执行的序列个数
        /// </summary>
        int SuccessCount { get; }

        /// <summary>
        /// 失败的序列个数
        /// </summary>
        int FailedCount { get; }

        /// <summary>
        /// 超时的序列个数
        /// </summary>
        int TimeOutCount { get; }

        /// <summary>
        /// TearDown模块是否成功
        /// </summary>
        bool TearDownSuccess { get; }
    }
}