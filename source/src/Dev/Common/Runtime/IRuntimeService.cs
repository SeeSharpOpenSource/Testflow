using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data.Sequence;

namespace Testflow.Runtime
{
    /// <summary>
    /// 运行时服务
    /// </summary>
    public interface IRuntimeService : IEntityComponent
    {
        /// <summary>
        /// 服务会话的测试工程组
        /// </summary>
        ITestProject TestProject { get; set; }

        /// <summary>
        /// 运行时状态
        /// </summary>
        RuntimeState State { get; }

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

        #region 事件

        /// <summary>
        /// 测试开始生成时触发
        /// </summary>
        event RuntimeDelegate.TestGenerationAction TestGenStart;

        /// <summary>
        /// 测试生成结束后触发
        /// </summary>
        event RuntimeDelegate.TestGenerationAction TestGenOver;

        /// <summary>
        /// 所有测试开始执行时触发
        /// </summary>
        event RuntimeDelegate.TestProjectStatusAction TestStart;

        /// <summary>
        /// 所有测试执行结束后触发
        /// </summary>
        event RuntimeDelegate.TestProjectStatusAction TestOver;

        #endregion

        #region 运行时控制

        /// <summary>
        /// 加载运行时待执行的测试工程
        /// </summary>
        /// <param name="testProject">待运行的测试工程</param>
        void Load(ITestProject testProject);

        /// <summary>
        /// 加载运行时待执行的序列组
        /// </summary>
        /// <param name="sequenceGroup">待运行的序列组</param>
        void Load(ISequenceGroup sequenceGroup);

        /// <summary>
        /// 激活运行时服务
        /// </summary>
        void Activate();

        /// <summary>
        /// 停止运行时服务
        /// </summary>
        void Deactivate();

        /// <summary>
        /// 开始运行
        /// </summary>
        void Run();

        /// <summary>
        /// 强制停止
        /// </summary>
        void Stop();

        #endregion

    }

    
}