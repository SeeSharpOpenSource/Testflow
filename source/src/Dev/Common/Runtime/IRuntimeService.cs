using System.Collections.Generic;
using Testflow.Common;
using Testflow.DataInterface.Sequence;

namespace Testflow.Runtime
{
    public interface IRuntimeService : IEntityComponent
    {
        /// <summary>
        /// 所有运行时Handler的集合
        /// </summary>
        IList<IRuntimeSession> Sessions { get; }

        /// <summary>
        /// 根据序列组名称获取Handler
        /// </summary>
        /// <param name="sequenceGroupName">序列组名称</param>
        /// <returns>返回的序列组Handler</returns>
        IRuntimeSession GetSession(string sequenceGroupName);

        /// <summary>
        /// 根据序列组获取Handler
        /// </summary>
        /// <param name="sequenceGroup">序列组</param>
        /// <returns>返回的序列组Handler</returns>
        IRuntimeSession GetSession(ISequenceGroup sequenceGroup);

    }

    
}