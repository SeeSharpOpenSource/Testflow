using System;
using System.Collections.Generic;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;

namespace Testflow.Modules
{
    /// <summary>
    /// 引擎控制模块
    /// </summary>
    public interface IEngineController : IController
    {
        /// <summary>
        /// 运行时状态
        /// </summary>
        RuntimeState State { get; }

        /// <summary>
        /// 获取运行时状态
        /// </summary>
        /// <param name="sessionId"></param>
        RuntimeState GetRuntimeState(int sessionId);

        /// <summary>
        /// 获取运行时组件
        /// </summary>
        /// <param name="componentName">组件名称</param>
        /// <param name="extraParams">额外参数</param>
        TDataType GetComponent<TDataType>(string componentName, params object[] extraParams);

        /// <summary>
        /// 获取运行时状态信息
        /// </summary>
        /// <param name="infoName">信息名称</param>
        /// <param name="extraParams">额外参数</param>
        TDataType GetRuntimeInfo<TDataType>(string infoName, params object[] extraParams);

        /// <summary>
        /// 注册运行时事件
        /// </summary>
        /// <param name="callBack">事件回调</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="extraParams">额外参数</param>
        void RegisterRuntimeEvent(Delegate callBack, string eventName, params object[] extraParams);

        /// <summary>
        /// 取消注册运行时事件
        /// </summary>
        /// <param name="callBack">事件回调</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="extraParams">额外参数</param>
        void UnregisterRuntimeEvent(Delegate callBack, string eventName, params object[] extraParams);

        /// <summary>
        /// 添加SequenceGroup的运行时对象
        /// </summary>
        /// <param name="sequenceGroup">待添加的序列组数据</param>
        /// <returns>会话ID</returns>
        int AddRuntimeTarget(ISequenceGroup sequenceGroup);

        /// <summary>
        /// 添加TestProject的运行时对象
        /// </summary>
        /// <param name="testProject">待添加的测试工程</param>
        /// <returns>TestProject对应的会话ID</returns>
        int AddRuntimeTarget(ITestProject testProject);

        /// <summary>
        /// 终止运行时会话
        /// </summary>
        void AbortRuntime(int sessionId);

        /// <summary>
        /// 引擎开始执行所有运行时会话
        /// </summary>
        void Start();

        /// <summary>
        /// 引擎停止执行所有运行时会话
        /// </summary>
        void Stop();
    }
}