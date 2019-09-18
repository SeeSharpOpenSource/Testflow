using System;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Messages;
using Testflow.SlaveCore.Common;
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
            // 打印状态日志
            _context.LogSession.Print(LogLevel.Info, _context.SessionId, "Slave controller started.");

            _context.MessageTransceiver.StartReceive();
            _context.LogSession.Print(LogLevel.Debug, _context.SessionId, "Slave transceiver started.");

            _downlinkMsgProcessor = new DownlinkMessageProcessor(_context);
            
            _context.UplinkMsgProcessor.Start();
            
            _flowTaskThread = new Thread(TestTaskWork)
            {
                IsBackground = true,
                Name = string.Format(Constants.TaskRootThreadNameFormat, _context.SessionId)
            };
            // TODO 配置运行时线程为STA模式以支持对SLAVE端显示界面的支持，目前该配置除了对COM组件的调用以外未发现有额外的影响
            _flowTaskThread.SetApartmentState(ApartmentState.STA);
            // 开始流程处理线程
            _flowTaskThread.Start();
            _context.FlowControlThread = _flowTaskThread;

            // 打印状态日志
            _context.LogSession.Print(LogLevel.Debug, _context.SessionId, 
                $"Flow task thread started, thread:{_flowTaskThread.ManagedThreadId}");

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
                _context.UplinkMsgProcessor.SendMessage(runtimeErrorMessage, true);
            }
            finally
            {
                StopSlaveTask();
                _context.LogSession.Print(LogLevel.Info, _context.SessionId, "Slave controller  stopped.");
            }
        }

        private void TestTaskWork(object state)
        {
            try
            {
                SlaveFlowTaskBase taskEntrance = SlaveFlowTaskBase.GetFlowTaskEntrance(_context);
                taskEntrance.DoFlowTask();
            }
            finally
            {
                _context.UplinkMsgProcessor?.Stop();
                _downlinkMsgProcessor?.Stop();
            }
        }

        private void StopSlaveTask()
        {
            
            if (_flowTaskThread.IsAlive && !_flowTaskThread.Join(Constants.ThreadAbortJoinTime))
            {
                _flowTaskThread.Abort();
                Thread.Sleep(Constants.AbortWaitTime);
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