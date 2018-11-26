using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data.Description;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 保存一个测试序列组执行后的综合状态统计信息，该信息通过整合各个监视点的数据获得。
    /// </summary>
    public interface ISequenceTestResult : IPropertyExtendable
    {
//        /// <summary>
//        /// 测试序列所在工程
//        /// </summary>
//        ITestProject TestProject { get; }

        /// <summary>
        /// 序列所在的测试序列组
        /// </summary>
        int SessionId { get; }

        /// <summary>
        /// 被执行的测试序列
        /// </summary>
        int SequenceIndex { get; }

        /// <summary>
        /// 测试结果状态
        /// </summary>
        RuntimeState ResultState { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// 消耗的时间，单位为ms
        /// </summary>
        ulong ElapsedTime { get; set; }

        /// <summary>
        /// 性能测试结果
        /// </summary>
        IPerformanceResult Performance { get; set; }

        /// <summary>
        /// 测试失败信息
        /// </summary>
        ISequenceFailedInfo FailedInfo { get; set; }

        /// <summary>
        /// 变量的事实取值
        /// </summary>
        ISerializableMap<string, object> VariableValues { get; }
    }
}