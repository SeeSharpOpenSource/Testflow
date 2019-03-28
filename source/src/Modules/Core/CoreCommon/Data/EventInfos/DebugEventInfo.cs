using System;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class DebugEventInfo : EventInfoBase
    {
        /// <summary>
        /// 是否为命中消息，如果为false，则为断点继续执行的操作。
        /// </summary>
        public bool IsDebugHit { get; }

        public DebugWatchData WatchData { get; }

        public DebugEventInfo(int session, DateTime time, bool isDebugHit) : base(session, EventType.Debug, time)
        {
            this.IsDebugHit = isDebugHit;
        }

        public DebugEventInfo(DebugMessage message) : base(message.Id, EventType.Debug, message.Time)
        {
            
            if (message.DebugMsgType == DebugMessageType.BreakPointHitted)
            {
                this.IsDebugHit = true;
                this.WatchData = message.WatchData;
            }
            else if (message.DebugMsgType == DebugMessageType.FreeDebugBlock)
            {
                this.IsDebugHit = false;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}