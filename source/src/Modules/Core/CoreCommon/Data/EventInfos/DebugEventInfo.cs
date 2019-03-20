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

        public DebugEventInfo(int session, DateTime time, bool isDebugHit) : base(session, EventType.Debug, time)
        {
            this.IsDebugHit = isDebugHit;
        }
    }
}