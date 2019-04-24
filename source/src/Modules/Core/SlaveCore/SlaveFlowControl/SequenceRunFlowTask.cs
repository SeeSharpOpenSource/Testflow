using System;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class SequenceRunFlowTask : SlaveFlowTaskBase
    {
        public SequenceRunFlowTask(SlaveContext context) : base(context)
        {
        }

        protected override void FlowTaskAction()
        {
            Context.State = RuntimeState.Running;



            Context.State = RuntimeState.Over;
            this.Next = null;
        }

        protected override void TaskErrorAction(Exception ex)
        {
            StatusMessage errorMessage = new StatusMessage(MessageNames.ErrorStatusName, Context.State, Context.SessionId)
            {
                ExceptionInfo = new SequenceFailedInfo(ex),
            };
            Context.SessionTaskEntity.FillSequenceInfo(errorMessage, Context.I18N.GetStr("RuntimeError"));
            Context.UplinkMsgPacker.SendMessage(errorMessage); ;
        }

        protected override void TaskAbortAction()
        {
            throw new System.NotImplementedException();
        }

        public override MessageBase GetHeartBeatMessage()
        {
            throw new System.NotImplementedException();
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}