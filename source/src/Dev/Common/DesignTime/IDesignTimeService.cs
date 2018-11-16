using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;

namespace Testflow.DesignTime
{
    /// <summary>
    /// 设计时服务
    /// </summary>
    public interface IDesignTimeService : IEntityComponent
    {
        /// <summary>
        /// 当前服务的测试工程
        /// </summary>
        ITestProject TestProject { get; set; }

        /// <summary>
        /// SetUp模块的会话
        /// </summary>
        IDesignTimeSession SetUpSession { get; }

        /// <summary>
        /// 保存所有设计时会话的集合
        /// </summary>
        IDictionary<int, IDesignTimeSession> SequenceSessions { get; }

        /// <summary>
        /// TearDown模块的会话
        /// </summary>
        IDesignTimeSession TearDownSession { get; }

        #region 设计时控制

        /// <summary>
        /// 激活设计时服务
        /// </summary>
        void Activate();

        /// <summary>
        /// 激活设计时服务
        /// </summary>
        /// <param name="testProject">待加载的测试工程</param>
        void Activate(ITestProject testProject);

        /// <summary>
        /// 激活设计时服务
        /// </summary>
        /// <param name="sequenceGroup">待加载的测试组</param>
        void Activate(ISequenceGroup sequenceGroup);

        /// <summary>
        /// 卸载当前测试工程
        /// </summary>
        void UnloadTestProject();

        /// <summary>
        /// 停止设计时服务
        /// </summary>
        void Deactivate();

        #endregion


        #region Sequence Edit

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

        #endregion

        #region 设计时支持

        /// <summary>
        /// 获取字符串对应变量匹配前缀的的属性集合
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="propertyPrefix"></param>
        /// <returns></returns>
        IList<string> GetFittedProperties(string variable, string propertyPrefix);

        #endregion


    }
}