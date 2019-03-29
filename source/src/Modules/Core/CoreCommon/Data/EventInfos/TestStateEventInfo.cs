using System;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class TestStateEventInfo : EventInfoBase
    {
        public TestState State { get; }

        public string ErrorInfo { get; set; }
        
        public TestStateEventInfo(RmtGenMessage message, TestState state) : base(message.Id, EventType.TestGen, message.Time)
        {
            this.State = state;
        }

        public TestStateEventInfo(int id, TestState state, DateTime time) : base(id, EventType.TestGen, time)
        {
            this.State = state;
        }
    }
}