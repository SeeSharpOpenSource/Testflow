using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    public interface IRuntimeService : IEntityComponent
    {
        /// <summary>
        /// 所有运行时Handler的集合
        /// </summary>
        IList<IRuntimeSession> Sessions { get; }
        
        /// <summary>
        /// 运行时服务的配置
        /// </summary>
        IRuntimeConfiguration Configuration { get; set; }

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
        /// <returns>返回的序列组运行会话</returns>
        IRuntimeSession GetSession(ISequenceGroup sequenceGroup);
    }

    
}