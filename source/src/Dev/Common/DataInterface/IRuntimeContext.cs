using System;
using Testflow.DataInterface.Sequence;

namespace Testflow.DataInterface
{
    /// <summary>
    /// 保存单个运行会话的上下文信息
    /// </summary>
    public interface IRuntimeContext
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
        /// 当前运行会话的线程ID
        /// </summary>
        int ThreadID { get; }
        
        /// <summary>
        /// 当前会话的开始时间
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// 当前会话关联的测试序列组
        /// </summary>
        ISequenceGroupData SequenceGroup { get; }
    }
}