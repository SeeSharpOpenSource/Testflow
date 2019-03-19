using System.Collections.Generic;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.RemoteRunner.Common;
using Testflow.Utility.MessageUtil;

namespace Testflow.SlaveCore
{
    internal class MessageTransceiver
    {
        private readonly Messenger _uplinkMessenger;
        private readonly Messenger _downLinkMessenger;
        private FormatterType _formatterType;
        private readonly ContextManager _contextManager;

        public MessageTransceiver(ContextManager contextManager)
        {
            // 创建上行队列
            _formatterType = contextManager.GetProperty<FormatterType>("EngineQueueFormat");
            MessengerOption receiveOption = new MessengerOption(CoreConstants.UpLinkMQName, typeof(ControlMessage),
                typeof(DebugMessage), typeof(RmtGenMessage), typeof(StatusMessage), typeof(TestGenMessage))
            {
                Type = contextManager.GetProperty<MessengerType>("MessengerType")
            };
            _uplinkMessenger = Messenger.GetMessenger(receiveOption);
            // 创建下行队列
            MessengerOption sendOption = new MessengerOption(CoreConstants.DownLinkMQName, typeof(ControlMessage),
                typeof(DebugMessage), typeof(RmtGenMessage), typeof(StatusMessage), typeof(TestGenMessage))
            {
                Type = contextManager.GetProperty<MessengerType>("MessengerType")
            };
            _downLinkMessenger = Messenger.GetMessenger(sendOption);
        }
    }
}