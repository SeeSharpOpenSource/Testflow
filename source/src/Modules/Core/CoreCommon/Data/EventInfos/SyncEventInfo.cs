using System;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Data.EventInfos
{
    // TODO 目前仅关心是否被阻塞，不关心其他细节。
    public class SyncEventInfo : EventInfoBase
    {
        public bool IsAcquireOperation { get; }
        public SyncEventInfo(ResourceSyncMessage message) : base(message.Id, EventType.Sync, message.Time)
        {
            IsAcquireOperation = message.Acquired;
        }
    }
}