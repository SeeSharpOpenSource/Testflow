using System;
using Testflow.DataInterface.Sequence;

namespace Testflow.DataInterface
{
    /// <summary>
    /// 保存单个运行会话的上下文信息
    /// </summary>
    public interface IRuntimeContext
    {
        string Name { get; }
        long SessionId { get; }
        object GetService(string serviceName, params object[] parameters);

        int ThreadID { get; }
        DateTime StartTime { get; }
        ISequenceGroupData SequenceGroup { get; }
    }
}