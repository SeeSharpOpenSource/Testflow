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
            throw new System.NotImplementedException();
        }

        protected override void StopTaskAction()
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