using System;
using Testflow.Common;
using Testflow.DataInterface.Sequence;

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
        /// 获取服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="parameters">传入的参数名称</param>
        /// <returns></returns>
        object GetService(string serviceName, params object[] parameters);
        
        /// <summary>
        /// 会话的运行时状态
        /// </summary>
        RuntimeState State { get; }
        
        /// <summary>
        /// 当前会话的开始时间
        /// </summary>
        DateTime StartTime { get; }



        /// <summary>
        /// 当前会话关联的测试序列组，如果当前Handle是TestGroup，则该项为null
        /// </summary>
        ISequenceGroup SequenceGroup { get; }

        /// <summary>
        /// 当前会话关联的测试组
        /// </summary>
        ITestGroup TestGroup { get; }
    }
}