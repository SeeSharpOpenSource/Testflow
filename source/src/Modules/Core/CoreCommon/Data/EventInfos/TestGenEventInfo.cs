using System;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class TestGenEventInfo : EventInfoBase
    {
        public TestGenState GenState { get; }

        public string ErrorInfo { get; set; }
        
        public TestGenEventInfo(RmtGenMessage message, TestGenState genState) : base(message.Id, EventType.TestGen, message.Time)
        {
            this.GenState = genState;
        }

        public TestGenEventInfo(int id, TestGenState genState, DateTime time) : base(id, EventType.TestGen, time)
        {
            this.GenState = genState;
        }
    }
}