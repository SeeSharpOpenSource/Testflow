using System;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.MasterCore.TestMaintain;
using Testflow.Modules;
using Testflow.Runtime;
using Testflow.Utility.MessageUtil;

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
        
        public EngineFlowController(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
            // TODO 暂时写死，只是用本地测试生成实体
            _testsMaintainer = new LocalTestEntityMaintainer(_globalInfo);
            if (EnableDebug)
            {
                _debugManager = new DebugManager(_globalInfo);
            }
            
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
            catch (TestflowException)
            {
                _globalInfo.StateMachine.State = RuntimeState.Error;
                _testsMaintainer.FreeHosts();
                throw;
            }
            catch (ApplicationException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex);
                _globalInfo.StateMachine.State = RuntimeState.Error;
                _testsMaintainer.FreeHosts();
                throw;
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex);
                _globalInfo.StateMachine.State = RuntimeState.Collapsed;
                _testsMaintainer.FreeHosts();
                throw;
            }
        }

        public void Start()
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
        }

        public bool HandleMessage(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}