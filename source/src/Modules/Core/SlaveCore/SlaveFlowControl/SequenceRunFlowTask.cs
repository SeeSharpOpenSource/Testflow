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

        protected override void StopTaskAction()
        {
            throw new System.NotImplementedException();
        }

        public override void AbortAction()
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