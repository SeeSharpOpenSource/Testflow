using System;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class AbortEventInfo : EventInfoBase
    {
        public AbortEventInfo(int session) : base(session, EventType.Abort, DateTime.Now)
        {
        }
    }
}