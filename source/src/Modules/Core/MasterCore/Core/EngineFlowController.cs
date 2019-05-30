using System;
using System.Linq;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.MasterCore.TestMaintain;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Runtime.Data;

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
            if (EnableDebug)
            {
                _globalInfo.MessageTransceiver.AddConsumer(MessageType.Debug.ToString(), _debugManager);
            }
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
            _globalInfo.StateMachine.State = RuntimeState.TestGen;
            if (sequenceContainer is ITestProject)
            {
                ITestProject testProject = (ITestProject)sequenceContainer;
                _testsMaintainer.Generate(testProject, RuntimePlatform.Clr);
                foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
                {
                    _testsMaintainer.Generate(sequenceGroup, RuntimePlatform.Clr);
                }
            }
            else if (sequenceContainer is ISequenceGroup)
            {
                _testsMaintainer.Generate((ISequenceGroup)sequenceContainer, RuntimePlatform.Clr);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void StartTestGeneration()
        {
            _testsMaintainer.StartHost(_sequenceData);
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
                // 初始化每个模块的监听变量
                foreach (int session in _testsMaintainer.TestContainers.Keys)
                {
                    _debugManager?.SendInitBreakPointMessage(session);
                }
            }
            else
            {
                _testsMaintainer.SendTestGenMessage(0, sequenceManager.RuntimeSerialize(_sequenceData as ISequenceGroup));
                _debugManager?.SendInitBreakPointMessage(0);
            }
            // 等待远程生成结束
            _blockHandle.Wait(Constants.RmtGenState);
        }

        // 如果是TestProject运行，则先开始TestProject的SetUp，在触发SequenceOver事件后检查是否是TestProject的SetUp，如果是则开始其他Session。
        // 注册其他的Session事件，如果所有Session都已经执行结束，则发送执行TestProject的TearDown的命令。
        // 如果是SequenceGroup执行，直接开始即可。
        public void StartTestWork()
        {
            _globalInfo.EventDispatcher.SessionOver += SessionOverClean;
            if (_sequenceData is ITestProject)
            {
                // 注册TestProject的Setup执行结束后的事件
                _globalInfo.EventDispatcher.SequenceOver += StartTestSessionsIfSetUpOver;

                ControlMessage startMessage = new ControlMessage(MessageNames.CtrlStart, CommonConst.TestGroupSession);
                startMessage.AddParam("RunSetup", true.ToString());
                _globalInfo.MessageTransceiver.Send(startMessage);
            }
            else
            {
                // TODO 这里没有再去监听运行端是否开始的返回信息，可能会出现断链的问题，后期再解决
                ControlMessage startMessage = new ControlMessage(MessageNames.CtrlStart, CommonConst.BroadcastSession);
                startMessage.AddParam("RunAll", true.ToString());
                foreach (int session in _testsMaintainer.TestContainers.Keys)
                {
                    startMessage.Id = session;
                    _globalInfo.MessageTransceiver.Send(startMessage);
                }
            }
        }

        /// <summary>
        /// 如果TestProject的SetUp执行成功发送开始测试Session的命令
        /// </summary>
        private void StartTestSessionsIfSetUpOver(ISequenceTestResult result)
        {
            if (result.SessionId != CommonConst.TestGroupSession || result.SequenceIndex != CommonConst.SetupIndex)
            {
                return;
            }
            _globalInfo.EventDispatcher.SequenceOver -= StartTestSessionsIfSetUpOver;
            if (result.ResultState != RuntimeState.Success)
            {
                return;
            }
            ControlMessage startMessage = new ControlMessage(MessageNames.CtrlStart, CommonConst.BroadcastSession);
            startMessage.AddParam("RunAll", true.ToString());
            // 发送开始指令
            foreach (int session in _testsMaintainer.TestContainers.Keys)
            {
                if (session == CommonConst.TestGroupSession)
                {
                    continue;
                }
                startMessage.Id = session;
                _globalInfo.MessageTransceiver.Send(startMessage);
            }
        }

        
        private void SessionOverClean(ITestResultCollection testResults)
        {
            _testsMaintainer.FreeHost(testResults.Session);
            // 如果只剩下TestMaintainer的数据，则发送执行TestProjectTearDown的命令
            if (_sequenceData is ITestProject && _testsMaintainer.TestContainers.Count == 1 &&
                _testsMaintainer.TestContainers.ContainsKey(CommonConst.TestGroupSession))
            {
                RunRootTearDownIfOtherSessionOver();
            }
        }

        /// <summary>
        /// 如果其他Session执行结束发送开始执行TestProjectTeardown的命令
        /// </summary>
        private void RunRootTearDownIfOtherSessionOver()
        {
            ControlMessage startMessage = new ControlMessage(MessageNames.CtrlStart, CommonConst.TestGroupSession);
            startMessage.AddParam("RunTearDown", true.ToString());
            _globalInfo.MessageTransceiver.Send(startMessage);
        }

        public bool HandleMessage(MessageBase message)
        {
            if (message is ControlMessage)
            {
                return HandleControlMessage((ControlMessage) message);
            }
            else
            {
                return HandleRuntimeErrorMessage((RuntimeErrorMessage) message);
            }
        }

        private bool HandleControlMessage(ControlMessage message)
        {
            int session = message.Id;
            string name = message.Name;
            switch (name)
            {
                case MessageNames.CtrlAbort:
                    bool abortSuccess = bool.Parse(message.Params["AbortSuccess"]);
                    AbortEventInfo abortEventInfo = new AbortEventInfo(session, false, abortSuccess);
                    if (!abortSuccess && message.Params.ContainsKey("Message"))
                    {
                        abortEventInfo.FailInfo = message.Params["Message"];
                    }
                    _globalInfo.EventQueue.Enqueue(abortEventInfo);
                    _testsMaintainer.FreeHost(session);
                    // 如果所有的都已经结束，则修改状态机状态
                    if (0 == _testsMaintainer.TestContainers.Count)
                    {
                        _globalInfo.StateMachine.State = RuntimeState.Abort;
                    }
                    // 同步释放，每个Session的停止都是同步执行的。
                    _blockHandle.Free(Constants.AbortState);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return true;
        }

        private bool HandleRuntimeErrorMessage(RuntimeErrorMessage message)
        {
            ExceptionEventInfo exceptionEventInfo = new ExceptionEventInfo(message);
            _globalInfo.EventQueue.Enqueue(exceptionEventInfo);
            _testsMaintainer.FreeHost(message.Id);
            return true;
        }

        public void AddToQueue(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void Abort(int sessionId)
        {
            this._globalInfo.StateMachine.State = RuntimeState.AbortRequested;
            if (sessionId == CommonConst.TestGroupSession || sessionId == CommonConst.BroadcastSession)
            {
                foreach (int session in _testsMaintainer.TestContainers.Keys)
                {
                    Abort(session);
                }
            }
            else
            {
                ControlMessage abortMessage = new ControlMessage(MessageNames.CtrlAbort, CommonConst.BroadcastSession);
                abortMessage.AddParam("IsRequest", true.ToString());
                abortMessage.Id = sessionId;
                _globalInfo.MessageTransceiver.Send(abortMessage);

                AbortEventInfo abortEventInfo = new AbortEventInfo(sessionId, true, false);
                _globalInfo.EventQueue.Enqueue(abortEventInfo);

                // 目前使用同步Abort，每次只能释放一个
                _blockHandle.Timeout = _globalInfo.ConfigData.GetProperty<int>("AbortTimeout");
                _blockHandle.Wait(Constants.AbortState);
            }
        }

        public void Stop()
        {
            _testsMaintainer.FreeHosts();
        }

        public void Dispose()
        {
            Abort(CommonConst.BroadcastSession);
            Stop();
        }
    }
}