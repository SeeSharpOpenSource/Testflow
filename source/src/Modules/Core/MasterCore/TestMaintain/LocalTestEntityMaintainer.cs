using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.TestMaintain.Container;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.TestMaintain
{
    /// <summary>
    /// 本地测试实体维护模块
    /// </summary>
    internal class LocalTestEntityMaintainer : ITestEntityMaintainer, IDisposable
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private Dictionary<int, RuntimeContainer> _runtimeContainers;

        public LocalTestEntityMaintainer(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
            this._runtimeContainers = new Dictionary<int, RuntimeContainer>(Constants.DefaultRuntimeSize);
        }

        /// <summary>
        /// 连接运行主机端
        /// </summary>
        public void ConnectHost(HostInfo runnerHosts)
        {
            // ignore
        }

        /// <summary>
        /// 释放远程主机端的连接
        /// </summary>
        public void DisconnectHost(int sessionId)
        {
            // ignore
        }

        public void StartHost()
        {
            foreach (RuntimeContainer container in _runtimeContainers.Values)
            {
                container.Start(GetSlaveHostConfigData(container.Session));
            }
        }

        public void FreeHost(int id)
        {
            if (_runtimeContainers.ContainsKey(id))
            {
                _runtimeContainers[id].Dispose();
                _runtimeContainers.Remove(id);
            }
        }

        public RuntimeContainer Generate(ITestProject testProject, RuntimePlatform platform, params object[] param)
        {
            RuntimeContainer runtimeContainer = RuntimeContainer.CreateContainer(Constants.TestProjectSessionId,
                platform, _globalInfo, param);
            _runtimeContainers.Add(runtimeContainer.Session, runtimeContainer);
            return runtimeContainer;
        }

        public RuntimeContainer Generate(ISequenceGroup sequenceGroup, RuntimePlatform platform, params object[] param)
        {
            // 如果是SequenceGroup并且还有入参，则必须和包含上级TestProject一起运行
            if (null != sequenceGroup && null == sequenceGroup.Parent && 0 != sequenceGroup.Arguments.Count)
            {
                ModuleUtility.LogAndRaiseDataException(LogLevel.Error, "SequenceGroup with input arguments cannot run with out test project.", ModuleErrorCode.SequenceDataError, null, "UnexistArgumentSource");
            }
            RuntimeContainer runtimeContainer = RuntimeContainer.CreateContainer(0, platform, _globalInfo, param);
            _runtimeContainers.Add(runtimeContainer.Session, runtimeContainer);
            return runtimeContainer;
        }

        public void SendTestGenMessage(int session, string sequenceData)
        {
            // TODO 暂时不配置Host信息
            RunnerType runnerType = (session == Constants.TestProjectSessionId)
                ? RunnerType.TestProject
                : RunnerType.SequenceGroup;
            RmtGenMessage rmtGenMessage = new RmtGenMessage(CoreConstants.DownRmtGenMsgName, session, runnerType,
                sequenceData);
            rmtGenMessage.Params.Add("MsgType", "Generation");
        }

        public void FreeHosts()
        {
            foreach (RuntimeContainer runtimeContainer in _runtimeContainers.Values)
            {
                runtimeContainer.Dispose();
            }
            _runtimeContainers.Clear();
        }

        public bool HandleMessage(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        private string GetSlaveHostConfigData(int session)
        {
            Dictionary<string, string> configData = new Dictionary<string, string>();
            configData.Add("Session", session.ToString());
            configData.Add("LogLevel", _globalInfo.ConfigData.GetProperty<LogLevel>("LogLevel").ToString());
            configData.Add("EngineQueueFormat", _globalInfo.ConfigData.GetProperty<FormatterType>("EngineQueueFormat").ToString());
            configData.Add("MessengerType", _globalInfo.ConfigData.GetProperty<MessengerType>("MessengerType").ToString());
            // TODO 暂时使用本地地址
            configData.Add("HostAddress", Constants.LocalHostAddr);
            configData.Add("StatusUploadInterval", _globalInfo.ConfigData.GetProperty<int>("StatusUploadInterval").ToString());
            configData.Add("ConnectionTimeout", _globalInfo.ConfigData.GetProperty<int>("ConnectionTimeout").ToString());
            configData.Add("SyncTimeout", _globalInfo.ConfigData.GetProperty<int>("SyncTimeout").ToString());
            configData.Add("TestTimeout", _globalInfo.ConfigData.GetProperty<int>("TestTimeout").ToString());

            return JsonConvert.SerializeObject(configData);
        }

        public void Dispose()
        {
            FreeHosts();
        }
    }
}