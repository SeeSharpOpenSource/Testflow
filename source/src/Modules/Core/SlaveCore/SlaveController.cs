using System;
using System.Threading;
using Testflow.Usr;
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
using Testflow.SlaveCore.SlaveFlowControl;

namespace Testflow.SlaveCore
{
    internal class SlaveController : IDisposable
    {
        private readonly MessageTransceiver _transceiver;
        private readonly SlaveContext _context;
        private Thread _flowTaskThread;
        private DownlinkMessageProcessor _downlinkMsgProcessor;

        public SlaveController(SlaveContext context)
        {
            this._transceiver = context.MessageTransceiver;
            this._context = context;
        }

        public void StartSlaveTask()
        {
            _context.MessageTransceiver.StartReceive();

            _downlinkMsgProcessor = new DownlinkMessageProcessor(_context);

            SlaveFlowTaskBase taskEntrance = SlaveFlowTaskBase.GetFlowTaskEntrance(_context);

            _context.UplinkMsgProcessor.Start();

            _flowTaskThread = new Thread(taskEntrance.DoFlowTask)
            {
                IsBackground = true,
                Name = string.Format(Constants.TaskRootThreadNameFormat, _context.SessionId)
            };
            // 开始流程处理线程
            _flowTaskThread.Start();
            try
            {
                // 在主线程内开始侦听下行消息并处理
                _downlinkMsgProcessor.StartListen();
            }
            catch (Exception ex)
            {
                _context.LogSession.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex,
                    "TestflowRuntimeException caught.");
                // 发送异常错误信息
                RuntimeErrorMessage runtimeErrorMessage = new RuntimeErrorMessage(_context.SessionId, ex)
                {
                    Index = _context.MsgIndex
                };
                _context.UplinkMsgProcessor.SendMessage(runtimeErrorMessage);
            }
            finally
            {
                StopSlaveTask();
            }
        }

        private void StopSlaveTask()
        {
            _flowTaskThread.Abort();
            if (_flowTaskThread.IsAlive)
            {
                _flowTaskThread.Join(Constants.ThreadAbortJoinTime);
            }
            _context.UplinkMsgProcessor?.Stop();
            _downlinkMsgProcessor?.Stop();
        }

        public void Dispose()
        {
            StopSlaveTask();
            _transceiver.Dispose();
            _context.Dispose();
        }
    }
}