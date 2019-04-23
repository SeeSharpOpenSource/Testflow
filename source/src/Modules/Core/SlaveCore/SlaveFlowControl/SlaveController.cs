using System;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class SlaveController : IDisposable
    {
        private readonly MessageTransceiver _transceiver;
        private readonly SlaveContext _context;
        private TestRunner _runner;

        public SlaveController(SlaveContext context)
        {
            this._transceiver = context.MessageTransceiver;
            this._context = context;
        }

        public void StartslaveTask()
        {
            _transceiver.StartReceive();

            try
            {
                _context.LogSession.Print(LogLevel.Info, CommonConst.PlatformLogSession, 
                    $"Test process of session {_context.SessionId} start.");

                SessionTaskEntity sessionExecutionModel = TestGeneration();
                _context.SessionTaskEntity = sessionExecutionModel;

                StartDownLinkMessageListening();

                SendGenerationOverMessage();

                StartTestWork(sessionExecutionModel);

                StateMonitoring();

                _context.LogSession.Print(LogLevel.Info, CommonConst.PlatformLogSession, "Test process start.");
            }
            catch (Exception ex)
            {
                StatusMessage statusMessage = new StatusMessage(MessageNames.ErrorStatusName, RuntimeState.Error,
                    _context.SessionId);
                statusMessage.ExceptionInfo = new SequenceFailedInfo(ex);
                _context.Runner?.FillStatusMessageInfo(statusMessage);
                FillPerformance(statusMessage);
                _transceiver.SendMessage(statusMessage);

                _context.LogSession.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, 
                    "Monitoring thread exited with unexpted exception.");

                StopSlaveTask();
            }
        }

        private void StartTestWork(SessionTaskEntity sessionExecutionModel)
        {
            _runner = TestRunner.CreateRunner(_context);
            _runner.Start(sessionExecutionModel);
        }

        private void StateMonitoring()
        {
            throw new NotImplementedException();
        }

        private void SendGenerationOverMessage()
        {
            throw new NotImplementedException();
        }

        private void StartDownLinkMessageListening()
        {
            throw new NotImplementedException();
        }

        // 生成测试数据
        private SessionTaskEntity TestGeneration()
        {
            LocalMessageQueue<MessageBase> messageQueue = _context.MessageTransceiver.MessageQueue;
            // 首先接收RmtGenMessage
            MessageBase message = messageQueue.WaitUntilMessageCome();

            RmtGenMessage rmtGenMessage = (RmtGenMessage)message;
            if (null == rmtGenMessage)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidMessageReceived,
                    _context.I18N.GetFStr("InvalidMessageReceived", message.GetType().Name));
            }

            try
            {
                // 发送测试开始消息
                _context.State = RuntimeState.TestGen;
                TestGenMessage testGenStartMessage = new TestGenMessage(MessageNames.TestGenName, _context.SessionId,
                    CommonConst.PlatformSession, GenerationStatus.InProgress);
                _context.MessageTransceiver.SendMessage(testGenStartMessage);

                InitializeRuntimeComponents(rmtGenMessage);
                SessionTaskEntity sessionExecutionModel = InitializeExecutionModel();

                // 发送测试结束消息
                _context.State = RuntimeState.StartIdle;
                TestGenMessage testGenOverMessage = new TestGenMessage(MessageNames.TestGenName, _context.SessionId,
                    CommonConst.PlatformSession, GenerationStatus.Success);
                _context.MessageTransceiver.SendMessage(testGenOverMessage);

                return sessionExecutionModel;
            }
            catch (Exception ex)
            {
                _context.LogSession.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex, "Test Generation failed.");
                _context.State = RuntimeState.Error;
                TestGenMessage testGenFailMessage = new TestGenMessage(MessageNames.TestGenName, _context.SessionId,
                CommonConst.PlatformSession, GenerationStatus.Failed);
                _context.MessageTransceiver.SendMessage(testGenFailMessage);
                
                throw;
            }
        }

        private void InitializeRuntimeComponents(RmtGenMessage rmtGenMessage)
        {
            // TODO slave端暂时没有好的获取SequenceManager的方式，目前直接引用SequenceManager，后续再去优化
            SequenceManager.SequenceManager sequenceManager = new SequenceManager.SequenceManager();
            _context.SequenceType = rmtGenMessage.SequenceType;
            if (rmtGenMessage.SequenceType == RunnerType.TestProject)
            {
                ITestProject testProject = sequenceManager.RuntimeDeserializeTestProject(rmtGenMessage.Sequence);
                _context.Sequence = testProject;
                _context.ExecutionModel = ExecutionModel.SequentialExecution;
            }
            else
            {
                ISequenceGroup sequenceGroup = sequenceManager.RuntimeDeserializeSequenceGroup(rmtGenMessage.Sequence);
                _context.Sequence = sequenceGroup;
                _context.ExecutionModel = sequenceGroup.ExecutionModel;
            }

            sequenceManager.Dispose();
            _context.VariableMapper = new VariableMapper(_context);
            switch (_context.SequenceType)
            {
                case RunnerType.TestProject:
                    ITestProject testProject = (ITestProject) _context.Sequence;
                    _context.TypeInvoker = new AssemblyInvoker(_context, testProject.Assemblies,
                        testProject.TypeDatas);
                    break;
                case RunnerType.SequenceGroup:
                    ISequenceGroup sequenceGroup = (ISequenceGroup) _context.Sequence;
                    _context.TypeInvoker = new AssemblyInvoker(_context, sequenceGroup.Assemblies,
                        sequenceGroup.TypeDatas);
                    break;
                default:
                    throw new InvalidProgramException();
            }
            _context.TypeInvoker.LoadAssemblyAndType();
        }

        private SessionTaskEntity InitializeExecutionModel()
        {
            SessionTaskEntity sessionExecutionModel = new SessionTaskEntity(_context);
            sessionExecutionModel.Generate();
            return sessionExecutionModel;
        }

        // TODO 暂时写死，使用AppDomain为单位计算
        private void FillPerformance(StatusMessage message)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            message.Performance.ProcessorTime = currentDomain.MonitoringTotalProcessorTime.TotalMilliseconds;
            message.Performance.MemoryUsed = currentDomain.MonitoringSurvivedMemorySize;
            message.Performance.MemoryAllocated = currentDomain.MonitoringTotalAllocatedMemorySize;
        }

        public void StopSlaveTask()
        {

        }

        public void Dispose()
        {
            _transceiver.Dispose();
            _context.Dispose();
        }
    }
}