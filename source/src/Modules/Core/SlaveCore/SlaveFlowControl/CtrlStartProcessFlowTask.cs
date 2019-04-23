using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class CtrlStartProcessFlowTask : SlaveFlowTaskBase
    {
        public CtrlStartProcessFlowTask(SlaveContext context) : base(context)
        {
        }

        protected override void FlowTaskAction()
        {
            // 发送测试结束消息
            Context.State = RuntimeState.StartIdle;


            Next = new SequenceRunFlowTask(Context);
        }

        protected override void StopTaskAction()
        {
            // ignore
        }

        public override void AbortAction()
        {
            // ignore
        }

        public override MessageBase GetHeartBeatMessage()
        {
            return new StatusMessage(MessageNames.HearBeatStatusName, Context.State, Context.SessionId);
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}