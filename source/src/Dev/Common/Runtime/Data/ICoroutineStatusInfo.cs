using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.Runtime.Data
{
    /// <summary>
    /// 协程的运行时情况
    /// </summary>
    public interface ICoroutineStatusInfo
    {
        /// <summary>
        /// 协程Id
        /// </summary>
        int CoroutineId { get; set; }

        /// <summary>
        /// 当前协程运行的堆栈信息
        /// </summary>
        ICallStack Stack { get; set; }

        /// <summary>
        /// 当前Step的执行结果信息
        /// </summary>
        StepResult Result { get; set; }

        /// <summary>
        /// 如果执行失败，失败信息的详细信息，否则为null
        /// </summary>
        IFailedInfo FailedInfo { get; set; }

        /// <summary>
        /// 当前协程下被监视的变量数据
        /// </summary>
        IDictionary<IVariable, string> WatchDatas { get; set; }
    }
}