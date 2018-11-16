using System;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 运行时单个测试用例组运行的会话
    /// </summary>
    public interface IRuntimeSession : IEntityComponent
    {
        /// <summary>
        /// 当前运行时的上下文信息
        /// </summary>
        IRuntimeContext Context { get; }

        /// <summary>
        /// 运行时状态
        /// </summary>
        RuntimeState State { get; }

        #region Status相关事件

        /// <summary>
        /// Events raised when a sequence is start and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.StatusReceivedAction SequenceStarted;

        /// <summary>
        /// Events raised when receive runtime status information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.StatusReceivedAction StatusReceived;

        /// <summary>
        /// Events raised when a sequence is over and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.StatusReceivedAction SequenceSuccess;

        /// <summary>
        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.StatusReceivedAction SequenceFailed;

        /// <summary>
        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.TestSessionOverAction TestOver;

        /// <summary>
        /// 断点命中事件，当某个断点被命中时触发
        /// </summary>
        event RuntimeDelegate.BreakPointHittedAction BreakPointHitted;

        #endregion


        #region 运行时控制

        /// <summary>
        /// 开始运行
        /// </summary>
        void Start();

        /// <summary>
        /// 强制停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 根据调用堆栈获取当前执行的Step
        /// </summary>
        /// <param name="stack">当前的执行堆栈</param>
        /// <returns></returns>
        ISequenceStep GetSequenceStep(ICallStack stack);

        #endregion

        #region 状态控制

        /// <summary>
        /// 强制更新所有的状态标识
        /// </summary>
        void ForceRefreshStatus();

        #endregion
    }
}