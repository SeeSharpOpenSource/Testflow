using System;
using System.Threading;
using Testflow.CoreCommon.Common;
using Testflow.Usr;
using Testflow.CoreCommon.Messages;
using Testflow.MasterCore.Common;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.Message
{
    internal class ZombieMessageCleaner : IDisposable
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private readonly Messenger _messenger;
        private readonly Timer _cleanTimer;
        private long _lastMessageIndex;

        public ZombieMessageCleaner(Messenger messenger, ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
            _messenger = messenger;
            _cleanTimer = new Timer(CleanZombieMessage, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            _lastMessageIndex = long.MaxValue;
            int timeout = _globalInfo.ConfigData.GetProperty<int>("MessageReceiveTimeout");
            _cleanTimer.Change(0, timeout);
        }

        private void CleanZombieMessage(object state)
        {
            if (0 == _messenger.MessageCount)
            {
                _lastMessageIndex = long.MaxValue;
                return;
            }
            IMessage message = _messenger.Peak();
            MessageBase runtimeMessage = message as MessageBase;
            if (null == runtimeMessage)
            {
                _lastMessageIndex = long.MaxValue;
                return;
            }
            // 超过超时时间后该消息被取出
            if (_lastMessageIndex == runtimeMessage.Index && runtimeMessage.Type != MessageType.RmtGen)
            {
                // 取出僵尸消息
                _messenger.Receive();
                _globalInfo.LogService.Print(LogLevel.Debug, CommonConst.PlatformLogSession,
                    $"Zoombie message detected. SessionId:{runtimeMessage.Id}, Type:{runtimeMessage}, Name:{runtimeMessage.Name}, Index:{runtimeMessage.Index}.");
                
                // 更新最新的消息
                message = _messenger.Peak();
                runtimeMessage = message as MessageBase;
                if (null == runtimeMessage)
                {
                    _lastMessageIndex = long.MaxValue;
                    return;
                }
            }
            _lastMessageIndex = runtimeMessage.Index;
        }

        public void Stop()
        {
            _cleanTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            _cleanTimer.Dispose();
        }
    }
}