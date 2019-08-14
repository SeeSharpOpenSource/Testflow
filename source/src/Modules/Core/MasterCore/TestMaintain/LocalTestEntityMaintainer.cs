using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.TestMaintain.Container;
using Testflow.Runtime;
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
        private readonly BlockHandle _blockHandle;
        
        public LocalTestEntityMaintainer(ModuleGlobalInfo globalInfo, BlockHandle blockHandle)
        {
            _globalInfo = globalInfo;
            this._runtimeContainers = new Dictionary<int, RuntimeContainer>(Constants.DefaultRuntimeSize);
            this._blockHandle = blockHandle;
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

        public void StartHost(ISequenceFlowContainer sequenceData)
        {
            if (sequenceData is ISequenceGroup)
            {
                _runtimeContainers[0].Start(GetSlaveHostConfigData(_runtimeContainers[0].Session, sequenceData.Name));
            }
            else
            {
                ITestProject testProject = sequenceData as ITestProject;
                foreach (RuntimeContainer container in _runtimeContainers.Values)
                {
                    string sessionName = null;
                    if (CommonConst.TestGroupSession == container.Session)
                    {
                        sessionName = Constants.TestProjectSessionName;
                    }
                    else
                    {
                        sessionName = testProject.SequenceGroups[container.Session].Name;
                    }
                    container.Start(GetSlaveHostConfigData(container.Session, sessionName));
                }
            }
        }

        public void FreeHost(int id)
        {
            try
            {
                if (_runtimeContainers.ContainsKey(id))
                {
                    RuntimeContainer runtimeContainer = _runtimeContainers[id];
                    _runtimeContainers.Remove(id);
                    runtimeContainer.Dispose();
                }
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, ex, 
                    "Exception raised when free host.");
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
                ModuleUtils.LogAndRaiseDataException(LogLevel.Error, "SequenceGroup with input arguments cannot run with out test project.", ModuleErrorCode.SequenceDataError, null, "UnexistArgumentSource");
            }
            RuntimeContainer runtimeContainer = RuntimeContainer.CreateContainer(0, platform, _globalInfo, param);
            _runtimeContainers.Add(runtimeContainer.Session, runtimeContainer);
            return runtimeContainer;
        }

        public void SendRmtGenMessage(int session, string sequenceData)
        {
            // TODO 暂时不配置Host信息
            RunnerType runnerType = (session == Constants.TestProjectSessionId)
                ? RunnerType.TestProject
                : RunnerType.SequenceGroup;
            RmtGenMessage rmtGenMessage = new RmtGenMessage(MessageNames.DownRmtGenMsgName, session, runnerType,
                sequenceData);
            rmtGenMessage.Params.Add("MsgType", "Generation");
            _globalInfo.MessageTransceiver.Send(rmtGenMessage);
            // 发送生成事件
            TestGenEventInfo testGenEventInfo = new TestGenEventInfo(rmtGenMessage, TestGenState.StartGeneration);
            _globalInfo.EventQueue.Enqueue(testGenEventInfo);
        }
        
        public Dictionary<int, RuntimeContainer> TestContainers => _runtimeContainers;

        public void FreeHosts()
        {
            foreach (RuntimeContainer runtimeContainer in _runtimeContainers.Values)
            {
                runtimeContainer.Dispose();
            }
            _runtimeContainers.Clear();
        }

        public bool HandleMessage(MessageBase message)
        {
            bool state = false;
            RmtGenMessage rmtGenMessage = (RmtGenMessage)message;
            if (rmtGenMessage.Params["MsgType"].Equals("Success"))
            {
                state = true;
                TestGenEventInfo genEventInfo = new TestGenEventInfo(rmtGenMessage.Id, TestGenState.GenerationOver,
                    rmtGenMessage.Time);
                _globalInfo.EventQueue.Enqueue(genEventInfo);
                _runtimeContainers[rmtGenMessage.Id].HostReady = true;
            }
            else if (rmtGenMessage.Params["MsgType"].Equals("Failed"))
            {
                state = false;
                TestGenEventInfo genEventInfo = new TestGenEventInfo(rmtGenMessage.Id, TestGenState.Error,
                    rmtGenMessage.Time);
                if (rmtGenMessage.Params.ContainsKey("FailedInfo"))
                {
                    genEventInfo.ErrorInfo = rmtGenMessage.Params["FailedInfo"];
                }
                _globalInfo.EventQueue.Enqueue(genEventInfo);
                FreeHost(rmtGenMessage.Id);
            }
            // 如果所有的host都已经ready，则释放主线程等待生成结束的锁
            if (_runtimeContainers.Values.All(item => item.HostReady))
            {
                _blockHandle.Free(Constants.RmtGenState);
            }
            return state;
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        private string GetSlaveHostConfigData(int session, string sessionName)
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
            configData.Add("RuntimeType", _globalInfo.ConfigData.GetProperty<RuntimeType>("RuntimeType").ToString());
            configData.Add("SessionName", sessionName);
            configData.Add("InstanceName", _globalInfo.ConfigData.GetProperty<string>("TestName"));
            configData.Add("DotNetLibDir", "");
            configData.Add("PlatformLibDir", "");
            configData.Add("InstanceLibDir", "");
            configData.Add("EnablePerformanceMonitor", _globalInfo.ConfigData.GetProperty<bool>("EnablePerformanceMonitor").ToString());

            return JsonConvert.SerializeObject(configData);
        }

        public void Dispose()
        {
            FreeHosts();
        }
    }
}