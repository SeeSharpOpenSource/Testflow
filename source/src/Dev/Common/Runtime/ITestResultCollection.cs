using System.Collections.Generic;

namespace Testflow.Runtime
{
    /// <summary>
    /// 一个测试序列组的所有测试结果的集合
    /// </summary>
    public interface ITestResultCollection : IDictionary<int, ISequenceTestResult>
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

        /// <summary>
        /// 变量的事实取值
        /// </summary>
        Dictionary<string, object> VariableValues { get; }
    }
}