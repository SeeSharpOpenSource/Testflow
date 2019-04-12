using System;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
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

                TestStateMonitoring();

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

        private void TestStateMonitoring()
        {
            LocalMessageQueue<MessageBase> messageQueue = _slaveContext.MessageTransceiver.MessageQueue;
            // 首先接收RmtGenMessage
            MessageBase message = messageQueue.WaitUntilMessageCome();



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