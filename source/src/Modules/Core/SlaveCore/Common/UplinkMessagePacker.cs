using System;
using System.Threading;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;

namespace Testflow.SlaveCore.Common
{
    /// <summary>
    /// 上行消息包装器，如果指定时间内无消息上报，则自动上传中间状态消息
    /// </summary>
    internal class StatusMonitor : IDisposable
    {
        private readonly SlaveContext _context;
        private readonly MessageTransceiver _transceiver;
        private readonly Timer _checkTimer;
        private readonly int _heartBeatInterval;

        public Func<MessageBase> HeartbeatMsgGenerator { get; set; }

        private int _stopFlag;

        const int SenderStop = 0;
        const int SenderStart = 1;

        public StatusMonitor(SlaveContext context)
        {
            this._transceiver = _context.MessageTransceiver;
            this._context = context;
            this._checkTimer = new Timer(SendHeartBeatMessage, null, Timeout.Infinite, Timeout.Infinite);
            this._heartBeatInterval = _context.GetProperty<int>("StatusUploadInterval");
            Thread.VolatileWrite(ref _stopFlag, SenderStop);
        }

        public void Start()
        {
            _checkTimer.Change(_heartBeatInterval, _heartBeatInterval);
            Thread.VolatileWrite(ref _stopFlag, SenderStart);
        }

        public void Stop()
        {
            Thread.VolatileWrite(ref _stopFlag, SenderStop);
            _checkTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void SendHeartBeatMessage(object state)
        {
            if (SenderStop == _stopFlag || null == HeartbeatMsgGenerator)
            {
                return;
            }
            MessageBase message = HeartbeatMsgGenerator.Invoke();
            _transceiver.SendMessage(message);
        }

        public void SendMessage(MessageBase message)
        {
            _transceiver.SendMessage(message);
            if (SenderStart == _stopFlag)
            {
                _checkTimer.Change(_heartBeatInterval, _heartBeatInterval);
            }
        }

        public void Dispose()
        {
            _checkTimer.Dispose();
        }
    }
}