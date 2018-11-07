using System.Collections.Generic;
using Testflow.Common;
using Testflow.DataInterface.Sequence;

namespace Testflow.DesignTime
{
    public interface IDesignTimeService : IEntityComponent
    {
        /// <summary>
        /// SetUp模块的Handler
        /// </summary>
        IDesignTimeSession SetUpHandler { get; }

        /// <summary>
        /// 保存所有设计时Handler的集合
        /// </summary>
        IList<IDesignTimeSession> Sessions { get; }

        /// <summary>
        /// TearDown模块的Handler
        /// </summary>
        IDesignTimeSession TearDownHandler { get; }

        /// <summary>
        /// 添加一个SequenceGroup
        /// </summary>
        /// <param name="name">SequenceGroup的名称</param>
        /// <param name="description">SequenceGroup的描述信息</param>
        /// <returns>新添加的Handler对象</returns>
        IDesignTimeSession AddSequenceGroup(string name, string description);

        /// <summary>
        /// 添加一个SequenceGroup
        /// </summary>
        /// <param name="sequenceGroup">待添加的SequenceGroup</param>
        /// <returns>新添加的Handler对象</returns>
        IDesignTimeSession AddSequenceGroup(ISequenceGroup sequenceGroup);

        /// <summary>
        /// 删除一个SequenceGroup
        /// </summary>
        /// <param name="name">SequenceGroup的名称</param>
        /// <param name="description">SequenceGroup的描述信息</param>
        /// <returns>新添加的Handler对象</returns>
        IDesignTimeSession RemoveSequenceGroup(string name, string description);

        /// <summary>
        /// 删除一个SequenceGroup
        /// </summary>
        /// <param name="sequenceGroup">待删除的SequenceGroup</param>
        /// <returns>删除的Handler对象</returns>
        IDesignTimeSession RemoveSequenceGroup(ISequenceGroup sequenceGroup);

        /// <summary>
        /// 删除一个SequenceGroup
        /// </summary>
        /// <param name="sequenceGroup">待删除的Handler</param>
        /// <returns>删除的Handler对象</returns>
        IDesignTimeSession RemoveSequenceGroup(IDesignTimeSession sequenceGroup);
    }
}