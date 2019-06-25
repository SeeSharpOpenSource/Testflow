﻿using System;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;

namespace Testflow.SlaveCore.Common
{
    internal class DownlinkMessageProcessor
    {
        private readonly SlaveContext _context;
        private CancellationTokenSource _cancellation;
        private LocalMessageQueue<MessageBase> _messageQueue;

        public DownlinkMessageProcessor(SlaveContext context)
        {
            this._context = context;
        }

        public void StartListen()
        {
            // 打印状态日志
            _context.LogSession.Print(LogLevel.Debug, _context.SessionId, 
                $"Downlink message processer started, thread:{Thread.CurrentThread.ManagedThreadId}.");

            _cancellation = new CancellationTokenSource();

            // 首先接收RmtGenMessage
            _messageQueue = _context.MessageTransceiver.MessageQueue;
            MessageBase message = _messageQueue.WaitUntilMessageCome();
            RmtGenMessage rmtGenMessage = (RmtGenMessage)message;
            if (null == rmtGenMessage)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidMessageReceived,
                    _context.I18N.GetFStr("InvalidMessageReceived", message.GetType().Name));
            }
            _context.RmtGenMessage = rmtGenMessage;

            while (!_cancellation.IsCancellationRequested)
            {
                message = _messageQueue.WaitUntilMessageCome();
                if (null == message)
                {
                    continue;
                }
                switch (message.Type)
                {
                    case MessageType.Ctrl:
                        ProcessControlMessage((ControlMessage)message);
                        break;
                    case MessageType.Debug:
                        ProcessDebugMessage((DebugMessage)message);
                        break;
                    case MessageType.Sync:
                        ProcessSyncMessage((ResourceSyncMessage) message);
                        break;
                    // 暂未在Master端实现发送RuntimeError消息的错误
                    case MessageType.RmtGen:
                    case MessageType.RuntimeError:
                    case MessageType.Status:
                    case MessageType.TestGen:
                    default:
                        throw new TestflowRuntimeException(ModuleErrorCode.InvalidMessageReceived,
                            _context.I18N.GetFStr("InvalidMessageReceived", message.GetType().Name));
                }
            }
            _context.LogSession.Print(LogLevel.Debug, _context.SessionId, 
                $"Downlink message processor stopped, Thread:{Thread.CurrentThread.ManagedThreadId}");
        }

        public void Stop()
        {
            _cancellation?.Cancel();
            _messageQueue?.StopEnqueue();
            _messageQueue?.FreeLock();
        }

        private void ProcessControlMessage(ControlMessage message)
        {
            switch (message.Name)
            {
                case MessageNames.CtrlStart:
                    _context.CtrlStartMessage = message;
                    break;
                case MessageNames.CtrlAbort:
                    _context.FlowControlThread.Abort();
                    // 如果线程还未结束，则等待
                    if (_context.FlowControlThread.IsAlive)
                    {
                        _context.FlowControlThread.Join(Constants.ThreadAbortJoinTime);
                    }
                    _cancellation.Cancel();
                    break;
            }
        }

        private void ProcessDebugMessage(DebugMessage message)
        {
            switch (message.DebugMsgType)
            {
                case DebugMessageType.AddBreakPoint:
                    _context.BreakPoints.Add(message.BreakPoint.ToString());
                    break;
                case DebugMessageType.RemoveBreakPoint:
                    _context.BreakPoints.Remove(message.BreakPoint.ToString());
                    break;
                case DebugMessageType.RefreshWatch:
                    // TODO 后续再实现，目前master端未提供该接口
                    break;
                case DebugMessageType.FreeDebugBlock:
                    // TODO
                    break;
                case DebugMessageType.BreakPointHitted:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessSyncMessage(ResourceSyncMessage message)
        {
            throw new NotImplementedException();
        }
    }
}