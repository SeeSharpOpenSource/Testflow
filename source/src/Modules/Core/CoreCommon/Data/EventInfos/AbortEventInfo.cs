using System;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Data.EventInfos
{
    public class AbortEventInfo : EventInfoBase
    {
        /// <summary>
        /// 为取消请求指令
        /// </summary>
        public bool IsRequest { get; set; }

        public bool AbortSuccess { get; set; }

        public string FailInfo { get; set; }

        public AbortEventInfo(int session, bool isRequest, bool abortSuccess) : base(session, EventType.Abort, DateTime.Now)
        {
            this.IsRequest = isRequest;
            this.AbortSuccess = abortSuccess;
        }
    }
}