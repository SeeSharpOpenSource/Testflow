using System;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class TestGenEventInfo : EventInfoBase
    {
        public TestGenState State { get; }

        public TestGenEventInfo(TestGenMessage message) : base(message.Id, EventType.TestGen, message.Time)
        {
            this.State = message.State;
        }
    }
}