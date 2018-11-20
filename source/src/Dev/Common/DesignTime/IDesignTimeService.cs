using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Description;
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
        /// 设计时导入的所有组件和程序集的映射
        /// </summary>
        IDictionary<string, IComInterfaceDescription> Components { get; }

        /// <summary>
        /// TearDown模块的会话
        /// </summary>
        IDesignTimeSession TearDownSession { get; }

        #region 设计时控制

        /// <summary>
        /// 激活运行时服务
        /// </summary>
        void Activate();

        /// <summary>
        /// 创建一个新的TestProject并加载
        /// </summary>
        /// <param name="name">TestProject的名称</param>
        /// <param name="description">TestProject的描述信息</param>
        /// <returns>加载成功后的TestProject</returns>
        ITestProject Load(string name, string description);

        /// <summary>
        /// 加载一个TestProject
        /// </summary>
        /// <param name="testProject">待加载的测试工程</param>
        /// <returns>加载成功后的TestProject</returns>
        ITestProject Load(ITestProject testProject);

        /// <summary>
        /// 创建一个新的TestProject并加载，添加一个已有的sequenceGroup到该Project
        /// </summary>
        /// <param name="name">TestProject的名称</param>
        /// <param name="description">TestProject的描述信息</param>
        /// <param name="sequenceGroup">待加载的测试组</param>
        /// <returns>加载成功后的TestProject</returns>
        ITestProject Load(string name, string description, ISequenceGroup sequenceGroup);

        /// <summary>
        /// 卸载当前测试工程
        /// </summary>
        void Unload();

        /// <summary>
        /// 停止设计时服务
        /// </summary>
        void Deactivate();

        #endregion


        #region Sequence Edit

        /// <summary>
        /// 添加组件库到当前设计时会话
        /// </summary>
        /// <param name="comInterface">待添加的组件</param>
        /// <returns>添加的组件</returns>
        IComInterfaceDescription AddComponent(IComInterfaceDescription comInterface);

        /// <summary>
        /// 在当前会话中删除某个组件库
        /// </summary>
        /// <param name="comInterface">待删除组件库</param>
        /// <returns>删除后的组件库</returns>
        IComInterfaceDescription RemoveComponent(IComInterfaceDescription comInterface);

        /// <summary>
        /// 在当前会话中删除某个组件库
        /// </summary>
        /// <param name="componentName">待删除组件库名称</param>
        /// <returns>删除后的组件库</returns>
        IComInterfaceDescription RemoveComponent(string componentName);

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