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
        /// 运行时触发异常的事件
        /// </summary>
        event Action<Exception> ExceptionRaised;

        /// <summary>
        /// 添加待运行的序列数据
        /// </summary>
        /// <param name="sequenceData"></param>
        void SetSequenceData(ISequenceFlowContainer sequenceData);

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
        /// 添加运行时对象
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="sessionId">会话id</param>
        /// <param name="param">额外参数</param>
        /// <returns>对象的运行时ID号</returns>
        long AddRuntimeObject(string objectType, int sessionId, params object[] param);

        /// <summary>
        /// 添加运行时对象
        /// </summary>
        /// <param name="objectId">对象ID</param>
        /// <param name="param">额外参数</param>
        /// <returns>删除运行时对象</returns>
        long RemoveRuntimeObject(int objectId, params object[] param);

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