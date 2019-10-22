using System;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Common
{
    /// <summary>
    /// 上行消息包装器，如果指定时间内无消息上报，则自动上传中间状态消息
    /// </summary>
    internal class UplinkMessageProcessor : IDisposable
    {
        private readonly SlaveContext _context;
        private readonly MessageTransceiver _transceiver;
        private readonly Timer _waitTimer;
        private readonly int _heartBeatInterval;
        private Thread _statusPackageThd;
        private CancellationTokenSource _cancellation;

        // 是否正在处理消息的标志位。-1为未执行状态，0为阻塞状态，1为处理过程中，2为处理结束，3为过程结束
        private int _eventProcessFlag;

        public Func<MessageBase> HeartbeatMsgGenerator { get; set; }

        public UplinkMessageProcessor(SlaveContext context)
        {
            this._context = context;
            this._transceiver = _context.MessageTransceiver;
            this._waitTimer = new Timer(SendHeartBeatMessage, null, Timeout.Infinite, Timeout.Infinite);
            this._heartBeatInterval = _context.GetProperty<int>("StatusUploadInterval");
            this._eventProcessFlag = -1;
        }

        public void Start()
        {
            this._cancellation = new CancellationTokenSource();
            _statusPackageThd = new Thread(PackageStatusInfo)
            {
                IsBackground = true,
                Name = string.Format(Constants.StatusMonitorThread, _context.SessionId)
            };
            _statusPackageThd.Start();
            _waitTimer.Change(_heartBeatInterval, _heartBeatInterval);
            // 打印状态日志
            _context.LogSession.Print(LogLevel.Debug, _context.SessionId, 
                $"Uplink message processor started, thread:{_statusPackageThd.ManagedThreadId}.");
        }

        public void Stop()
        {
            _context.StatusQueue.FreeBlocks();

            _waitTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _cancellation?.Cancel();

            // 打印状态日志
            _context.LogSession.Print(LogLevel.Debug, _context.SessionId,
                $"Uplink message processor stoped, thread:{_statusPackageThd.ManagedThreadId}.");
        }

        private void PackageStatusInfo()
        {
            LocalEventQueue<SequenceStatusInfo> statusQueue = _context.StatusQueue;
            while (!_cancellation.IsCancellationRequested)
            {
                // 标记事件处理flag为阻塞中
                Thread.MemoryBarrier();
                Thread.VolatileWrite(ref _eventProcessFlag, 0);

                SequenceStatusInfo statusInfo = statusQueue.WaitUntilMessageCome();
                // 如果为null，StatusQueue已经停止接收，直接跳出
                if (null == statusInfo)
                {
                    return;
                }
                // 标记事件处理flag为处理中
                Thread.MemoryBarrier();
                Thread.VolatileWrite(ref _eventProcessFlag, 1);
                
                SendSequenceStatusMessage(statusInfo);

                // 标记事件处理flag为处理结束
                Thread.MemoryBarrier();
                Thread.VolatileWrite(ref _eventProcessFlag, 2);
            }

            // 标记事件处理flag为结束状态
            Thread.MemoryBarrier();
            Thread.VolatileWrite(ref _eventProcessFlag, 3);
        }

        private void SendSequenceStatusMessage(SequenceStatusInfo statusInfo)
        {
            StatusMessage statusMessage = null;
            switch (statusInfo.ReportType)
            {
                case StatusReportType.Start:
                    statusMessage = new StatusMessage(MessageNames.ReportStatusName, _context.State, _context.SessionId)
                    {
                        Time = statusInfo.Time, Index = _context.MsgIndex
                    };
                    statusMessage.InterestedSequence.Add(statusInfo.Sequence);
                    statusMessage.Stacks.Add(statusInfo.Stack);
                    statusMessage.SequenceStates.Add(RuntimeState.Running);
                    statusMessage.Results.Add(statusInfo.Result);
                    statusMessage.ExecutionTimes.Add(statusInfo.ExecutionTime.ToString(CommonConst.GlobalTimeFormat));
                    statusMessage.ExecutionTicks.Add(statusInfo.ExecutionTicks);
                    statusMessage.Coroutines.Add(statusInfo.CoroutineId);
                    _transceiver.SendMessage(statusMessage);
                    break;
                case StatusReportType.Record:
                    statusMessage = new StatusMessage(MessageNames.ReportStatusName, _context.State, _context.SessionId)
                    {
                        Time = statusInfo.Time, Index = _context.MsgIndex
                    };
                    statusMessage.InterestedSequence.Add(statusInfo.Sequence);
                    statusMessage.Stacks.Add(statusInfo.Stack);
                    statusMessage.SequenceStates.Add(RuntimeState.Running);
                    statusMessage.Results.Add(statusInfo.Result);
                    statusMessage.WatchData = statusInfo.WatchDatas;
                    statusMessage.ExecutionTimes.Add(statusInfo.ExecutionTime.ToString(CommonConst.GlobalTimeFormat));
                    statusMessage.ExecutionTicks.Add(statusInfo.ExecutionTicks);
                    statusMessage.Coroutines.Add(statusInfo.CoroutineId);
                    if (statusInfo.FailedInfo != null)
                    {
                        statusMessage.FailedInfo.Add(statusInfo.Sequence, statusInfo.FailedInfo.ToString());
                    }
                    _transceiver.SendMessage(statusMessage);
                    break;
                case StatusReportType.DebugHitted:
                    DebugMessage debugMessage = new DebugMessage(MessageNames.BreakPointHitName, _context.SessionId,
                        statusInfo.Stack, false)
                    {
                        Time = statusInfo.Time,
                        Index = _context.MsgIndex
                    };
                    // TODO add watch datas
                    _transceiver.SendMessage(debugMessage);
                    break;
                case StatusReportType.Failed:
                    statusMessage = new StatusMessage(MessageNames.ReportStatusName, _context.State, _context.SessionId)
                    {
                        Time = statusInfo.Time, Index = _context.MsgIndex
                    };
                    statusMessage.InterestedSequence.Add(statusInfo.Sequence);
                    statusMessage.Stacks.Add(statusInfo.Stack);
                    statusMessage.SequenceStates.Add(RuntimeState.Failed);
                    statusMessage.Results.Add(statusInfo.Result);
                    statusMessage.WatchData = statusInfo.WatchDatas;
                    statusMessage.ExecutionTimes.Add(statusInfo.ExecutionTime.ToString(CommonConst.GlobalTimeFormat));
                    statusMessage.ExecutionTicks.Add(statusInfo.ExecutionTicks);
                    statusMessage.Coroutines.Add(statusInfo.CoroutineId);
                    if (statusInfo.FailedInfo != null)
                    {
                        statusMessage.FailedInfo.Add(statusInfo.Sequence, statusInfo.FailedInfo.ToString());
                    }
                    _transceiver.SendMessage(statusMessage);
                    break;
                case StatusReportType.Over:
                    statusMessage = new StatusMessage(MessageNames.ReportStatusName, _context.State, _context.SessionId)
                    {
                        Time = statusInfo.Time, Index = _context.MsgIndex
                    };
                    statusMessage.InterestedSequence.Add(statusInfo.Sequence);
                    statusMessage.Stacks.Add(statusInfo.Stack);
                    statusMessage.SequenceStates.Add(RuntimeState.Success);
                    statusMessage.Results.Add(statusInfo.Result);
                    statusMessage.WatchData = statusInfo.WatchDatas;
                    statusMessage.ExecutionTimes.Add(statusInfo.ExecutionTime.ToString(CommonConst.GlobalTimeFormat));
                    statusMessage.ExecutionTicks.Add(statusInfo.ExecutionTicks);
                    statusMessage.Coroutines.Add(statusInfo.CoroutineId);
                    _transceiver.SendMessage(statusMessage);
                    break;
                case StatusReportType.Error:
                    statusMessage = new StatusMessage(MessageNames.ReportStatusName, _context.State, _context.SessionId)
                    {
                        Time = statusInfo.Time, Index = _context.MsgIndex
                    };
                    statusMessage.InterestedSequence.Add(statusInfo.Sequence);
                    statusMessage.Stacks.Add(statusInfo.Stack);
                    statusMessage.SequenceStates.Add(RuntimeState.Error);
                    statusMessage.Results.Add(statusInfo.Result);
                    statusMessage.WatchData = statusInfo.WatchDatas;
                    statusMessage.ExecutionTimes.Add(statusInfo.ExecutionTime.ToString(CommonConst.GlobalTimeFormat));
                    statusMessage.ExecutionTicks.Add(statusInfo.ExecutionTicks);
                    statusMessage.Coroutines.Add(statusInfo.CoroutineId);
                    if (statusInfo.FailedInfo != null)
                    {
                        statusMessage.FailedInfo.Add(statusInfo.Sequence, statusInfo.FailedInfo.ToString());
                        statusMessage.ExceptionInfo = null;
                    }
                    _transceiver.SendMessage(statusMessage);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SendHeartBeatMessage(object state)
        {
            bool cancellationRequested = _cancellation.IsCancellationRequested;
            if (cancellationRequested || null == HeartbeatMsgGenerator)
            {
                return;
            }
            MessageBase message = HeartbeatMsgGenerator.Invoke();
            if (null != message)
            {
                _transceiver.SendMessage(message);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">待发送的消息</param>
        /// <param name="waitEventOver">是否等待事件处理结束，如果为true，则必须等事件队列中所有的被处理完才能发送</param>
        public void SendMessage(MessageBase message, bool waitEventOver)
        {
            if (waitEventOver && _statusPackageThd.IsAlive)
            {
                // 如果事件队列长度大于0，或者当前处于事件处理的过程，暂停发送。
                // TODO 这里的多线程处理并不完善，在获取到消息但是状态1还未被配置时调用时仍会出现问题，后续优化
                while (_context.StatusQueue.Count > 0 || _eventProcessFlag == 1)
                {
                    Thread.Yield();
                }
            }
            _transceiver.SendMessage(message);
            // 如果过程未结束，并且发送的是StatusMaintainer接收的消息，则重置心跳包发送timer
            if (!_cancellation.IsCancellationRequested &&
                (message.Type == MessageType.Status || message.Type == MessageType.TestGen))
            {
                _waitTimer.Change(_heartBeatInterval, _heartBeatInterval);
            }
        }


        public void Dispose()
        {
            _waitTimer.Dispose();
        }
    }
}