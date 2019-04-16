using System;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.Runtime;

namespace Testflow.SlaveCore.Controller
{
    internal class SlaveController : IDisposable
    {
        private readonly MessageTransceiver _transceiver;
        private readonly SlaveContext _slaveContext;

        public SlaveController(SlaveContext slaveContext)
        {
            this._transceiver = slaveContext.MessageTransceiver;
            this._slaveContext = slaveContext;
        }

        public void StartMonitoring()
        {
            _transceiver.StartReceive();
            HandleDownlinkMessage();
        }

        private void HandleDownlinkMessage()
        {
            try
            {
                _slaveContext.LogSession.Print(LogLevel.Info, CommonConst.PlatformLogSession, "Monitoring thread start.");

                TestGeneration();

                StartDownLinkMessageListening();

                SendGenerationOverMessage();

                StateMonitoring();

                _slaveContext.LogSession.Print(LogLevel.Info, CommonConst.PlatformLogSession, "Monitoring thread over.");
            }
            catch (Exception ex)
            {
                StatusMessage statusMessage = new StatusMessage(MessageNames.ErrorStatusName, RuntimeState.Error,
                    _slaveContext.SessionId);
                statusMessage.ExceptionInfo = new SequenceFailedInfo(ex);
                _slaveContext.Runner?.FillStatusMessageInfo(statusMessage);
                FillPerformance(statusMessage);
                _transceiver.SendMessage(statusMessage);

                _slaveContext.LogSession.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, 
                    "Monitoring thread exited with unexpted exception.");
            }
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

        // 测试数据
        private void TestGeneration()
        {
            LocalMessageQueue<MessageBase> messageQueue = _slaveContext.MessageTransceiver.MessageQueue;
            // 首先接收RmtGenMessage
            MessageBase message = messageQueue.WaitUntilMessageCome();

            RmtGenMessage rmtGenMessage = (RmtGenMessage)message;
            if (null == rmtGenMessage)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidMessageReceived,
                    _slaveContext.I18N.GetFStr("InvalidMessageReceived", message.GetType().Name));
            }
            
            // TODO slave端暂时没有好的获取SequenceManager的方式，目前直接引用SequenceManager，后续再去优化
            SequenceManager.SequenceManager sequenceManager = new SequenceManager.SequenceManager();
            _slaveContext.SequenceType = rmtGenMessage.SequenceType;
            if (rmtGenMessage.SequenceType == RunnerType.TestProject)
            {
                _slaveContext.Sequence = sequenceManager.RuntimeDeserializeTestProject(rmtGenMessage.Sequence);
            }
            else
            {
                _slaveContext.Sequence = sequenceManager.RuntimeDeserializeSequenceGroup(rmtGenMessage.Sequence);
            }
        }

        // TODO 暂时写死，使用AppDomain为单位计算
        private void FillPerformance(StatusMessage message)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            message.Performance.ProcessorTime = currentDomain.MonitoringTotalProcessorTime.TotalMilliseconds;
            message.Performance.MemoryUsed = currentDomain.MonitoringSurvivedMemorySize;
            message.Performance.MemoryAllocated = currentDomain.MonitoringTotalAllocatedMemorySize;
        }

        public void Dispose()
        {
            _transceiver.Dispose();
        }
    }
}