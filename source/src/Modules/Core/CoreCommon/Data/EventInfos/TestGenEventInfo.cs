using System;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class TestGenEventInfo : EventInfoBase
    {
        public TestGenState GenState { get; }

        /// <summary>
        /// 测试生成错误信息
        /// </summary>
        public FailedInfo ErrorInfo { get; set; }

        /// <summary>
        /// 失败Step的堆栈信息
        /// </summary>
        public ICallStack ErrorStack { get; set; }
        
        public TestGenEventInfo(RmtGenMessage message, TestGenState genState) : base(message.Id, EventType.TestGen, message.Time)
        {
            this.GenState = genState;
            this.ErrorInfo = !string.IsNullOrWhiteSpace(message.ErrorInfo) ? new FailedInfo(message.ErrorInfo) : null;
            this.ErrorStack = message.ErrorStack;
        }

        public TestGenEventInfo(int id, TestGenState genState, DateTime time) : base(id, EventType.TestGen, time)
        {
            this.GenState = genState;
        }
    }
}