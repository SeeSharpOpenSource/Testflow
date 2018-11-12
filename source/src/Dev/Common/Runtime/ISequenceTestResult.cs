﻿using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 保存一个测试序列组的综合状态统计信息，该信息通过整合各个监视点的数据获得。
    /// </summary>
    public interface ISequenceTestResult : IPropertyExtendable
    {
        /// <summary>
        /// 测试序列所在工程
        /// </summary>
        ITestProject TestProject { get; }

        /// <summary>
        /// 序列所在的测试序列组
        /// </summary>
        ISequenceGroup SequenceGroup { get; }

        /// <summary>
        /// 被执行的测试序列
        /// </summary>
        ISequence Sequence { get; }

        /// <summary>
        /// 测试是否成功
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// 消耗的时间
        /// </summary>
        TimeSpan ElapsedTime { get; set; }

        /// <summary>
        /// 性能测试结果
        /// </summary>
        IPerformanceResult Performance { get; set; }

        /// <summary>
        /// 测试失败信息
        /// </summary>
        ISequenceFailedInfo FailedInfo { get; set; }
    }
}