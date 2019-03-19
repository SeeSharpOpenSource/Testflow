using System;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.MasterCore.TestMaintain;
using Testflow.Runtime;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Core
{
    /// <summary>
    /// 实现引擎的运行时流程管理功能
    /// </summary>
    internal class EngineFlowController : IMessageHandler
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private readonly ITestEntityMaintainer _testsMaintainer;
        private readonly DebugManager _debugManager;

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

        RuntimeType RuntimeType => _globalInfo.ConfigData.GetProperty<RuntimeType>("RuntimeType");

        private bool EnableDebug => RuntimeType.Debug == RuntimeType;

        public void RegisterMessageHandler()
        {
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Ctrl.ToString(), this);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.RmtGen.ToString(), _testsMaintainer);
            _globalInfo.MessageTransceiver.AddConsumer(MessageType.Debug.ToString(), _debugManager);
        }

        public void Initialize(ISequenceFlowContainer sequenceContainer)
        {
            GenerateTestMaintainer(sequenceContainer);
        }

        // TODO 暂时是一个失败，所有的都停止操作，后续优化为状态控制
        private void GenerateTestMaintainer(ISequenceFlowContainer sequenceContainer)
        {
            try
            {
                if (sequenceContainer is ITestProject)
                {
                    ITestProject testProject = (ITestProject) sequenceContainer;
                    _testsMaintainer.Generate(testProject);
                    foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
                    {
                        _testsMaintainer.Generate(sequenceGroup);
                    }
                }
                else if (sequenceContainer is ISequenceGroup)
                {
                    _testsMaintainer.Generate((ISequenceGroup) sequenceContainer);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            catch (ApplicationException ex)
            {
                _globalInfo.LogService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex);
                _globalInfo.State = RuntimeState.Error;
                _testsMaintainer.FreeHosts();
                throw;
            }
            catch (Exception ex)
            {
                _globalInfo.LogService.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex);
                _globalInfo.State = RuntimeState.Error;
                _testsMaintainer.FreeHosts();
                throw;
            }
        }

        public void HandleMessage(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}