using System;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.MasterCore.TestMaintain;
using Testflow.Modules;
using Testflow.Runtime;

namespace Testflow.MasterCore.Core
{
    /// <summary>
    /// 实现引擎的运行时流程管理功能
    /// </summary>
    internal class EngineFlowController : IMessageHandler, IDisposable
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private readonly ITestEntityMaintainer _testsMaintainer;
        private readonly DebugManager _debugManager;
        private ISequenceFlowContainer _sequenceData;

        private readonly BlockHandle _blockHandle;

        public EngineFlowController(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
            // TODO 暂时写死，只是用本地测试生成实体
            _testsMaintainer = new LocalTestEntityMaintainer(_globalInfo, _blockHandle);
            if (EnableDebug)
            {
                _debugManager = new DebugManager(_globalInfo);
            }
            this._blockHandle = new BlockHandle()
            {
                Timeout = _globalInfo.ConfigData.GetProperty<int>("TestGenTimeout")
            };
        }

        public RuntimeType RuntimeType => _globalInfo.ConfigData.GetProperty<RuntimeType>("RuntimeType");

        public bool EnableDebug => RuntimeType.Debug == RuntimeType;

        public DebugManager Debugger => _debugManager;

        public ITestEntityMaintainer TestMaintainer => _testsMaintainer;

        public void RegisterMessageHandler()
        {
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Ctrl.ToString(), this);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.RuntimeError.ToString(), this);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.RmtGen.ToString(), _testsMaintainer);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Debug.ToString(), _debugManager);
        }

        public void Initialize(ISequenceFlowContainer sequenceContainer)
        {
            _sequenceData = sequenceContainer;
            GenerateTestMaintainer(sequenceContainer);
        }

        // TODO 暂时是一个失败，所有的都停止操作，后续优化为状态控制
        // TODO 目标平台暂时写死
        private void GenerateTestMaintainer(ISequenceFlowContainer sequenceContainer)
        {
            try
            {
                if (sequenceContainer is ITestProject)
                {
                    ITestProject testProject = (ITestProject) sequenceContainer;
                    _testsMaintainer.Generate(testProject, RuntimePlatform.Clr);
                    foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
                    {
                        _testsMaintainer.Generate(sequenceGroup, RuntimePlatform.Clr);
                    }
                }
                else if (sequenceContainer is ISequenceGroup)
                {
                    _testsMaintainer.Generate((ISequenceGroup) sequenceContainer, RuntimePlatform.Clr);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (Exception)
            {
                _testsMaintainer.FreeHosts();
                throw;
            }
        }

        public void StartTestGeneration()
        {
            _testsMaintainer.StartHost();
            ISequenceManager sequenceManager = _globalInfo.TestflowRunner.SequenceManager;
            if (_sequenceData is ITestProject)
            {
                ITestProject testProject = _sequenceData as ITestProject;
                _testsMaintainer.SendTestGenMessage(CoreConstants.TestProjectSessionId,
                    sequenceManager.RuntimeSerialize(testProject));
                foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
                {
                    _testsMaintainer.SendTestGenMessage(ModuleUtils.GetSessionId(testProject, sequenceGroup), 
                        sequenceManager.RuntimeSerialize(sequenceGroup));
                }
            }
            else
            {
                _testsMaintainer.SendTestGenMessage(0, sequenceManager.RuntimeSerialize(_sequenceData as ISequenceGroup));
            }
            // 等待远程生成结束
            _blockHandle.Wait(Constants.RmtGenState);
        }

        public void StartTestWork()
        {
            // TODO 这里没有再去监听运行端是否开始的返回信息，可能会出现断链的问题，后期再解决
            ControlMessage startMessage = new ControlMessage(MessageNames.CtrlStart, CommonConst.BroadcastSession);
            foreach (int session in _testsMaintainer.TestContainers.Keys)
            {
                startMessage.Id = session;
                _globalInfo.MessageTransceiver.Send(startMessage);
                TestStateEventInfo stateEventInfo = new TestStateEventInfo(session, TestState.TestStart, startMessage.Time);
                _globalInfo.EventQueue.Enqueue(stateEventInfo);
            }
        }

        public bool HandleMessage(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            ControlMessage abortMessage = new ControlMessage(MessageNames.CtrlAbort, CommonConst.BroadcastSession);
            abortMessage.AddParam("IsRequest", true.ToString());
            foreach (int session in _testsMaintainer.TestContainers.Keys)
            {
                abortMessage.Id = session;
                _globalInfo.MessageTransceiver.Send(abortMessage);
            }
//            _testsMaintainer.FreeHosts();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}