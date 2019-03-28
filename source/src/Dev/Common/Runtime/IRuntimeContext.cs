using System;
using System.Collections.Generic;
using System.Diagnostics;
using Testflow.Common;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 保存单个运行会话的上下文信息
    /// </summary>
    public interface IRuntimeContext : IEntityComponent
    {
        /// <summary>
        /// 运行会话的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 运行时的会话ID
        /// </summary>
        long SessionId { get; }

        /// <summary>
        /// 主机信息
        /// </summary>
        IHostInfo HostInfo { get; }

        /// <summary>
        /// 运行当前会话的进程
        /// </summary>
        Process Process { get; }

        /// <summary>
        /// 当前运行会话的AppDomain
        /// </summary>
        AppDomain RunDomain { get; set; }

        /// <summary>
        /// 当前运行会话的线程ID
        /// </summary>
        int ThreadID { get; }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="parameters">传入的参数名称</param>
        /// <returns></returns>
        object GetService(string serviceName, params object[] parameters);
        
        /// <summary>
        /// 当前会话关联的测试组
        /// </summary>
        ITestProject TestGroup { get; }

        /// <summary>
        /// 当前会话关联的测试序列组，如果当前Handle是TestGroup，则该项为null
        /// </summary>
        ISequenceGroup SequenceGroup { get; }

        /// <summary>
        /// 调试器
        /// </summary>
        ISequenceDebuggerCollection Debuggers { get; }

        /// <summary>
        /// 当前的调试器
        /// </summary>
        IDebuggerHandle DebuggerHandle { get; }

        /// <summary>
        /// 所有序列的运行时状态信息
        /// </summary>
        IRuntimeStatusCollection RunTimeStatus { get; }
    }

    
}