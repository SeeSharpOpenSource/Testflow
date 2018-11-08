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

        /// <summary>
        /// 运行容器
        /// </summary>
        IRunningContainer RunDomain { get; }

        /// <summary>
        /// 状态刷新控制器
        /// </summary>
        IStatusMaintainer StatusMaintainer { get; }

        /// <summary>
        /// 持续时间
        /// </summary>
        TimeSpan ElapsedTime { get; }

        

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

        IRuntimeStatusInfo AcquireStatus();

        #endregion
    }
}