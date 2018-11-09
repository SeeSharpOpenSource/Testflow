using System;
using Testflow.Common;
using Testflow.DataInterface;

namespace Testflow.Runtime
{
    public interface IRuntimeSession : IEntityComponent
    {
        /// <summary>
        /// 当前运行时的上下文信息
        /// </summary>
        IRuntimeContext Context { get; }

        RuntimeState

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
        event RuntimeDelegate.StatusReceivedAction SequenceOver;

        /// <summary>
        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.StatusReceivedAction SequenceFailed;

        /// <summary>
        /// Events raised when a sequence is failed and host receive runtime stauts information. Asynchronous event.
        /// </summary>
        event RuntimeDelegate.TestOverAction TestOver;

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

        #endregion

        #region 状态控制

        void ForceRefreshStatus();

        #endregion
    }
}