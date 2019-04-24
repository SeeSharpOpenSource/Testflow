using System;
using Testflow.CoreCommon.Messages;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class RunSingleSequenceFlowTask : SlaveFlowTaskBase
    {
        public RunSingleSequenceFlowTask(SlaveContext context) : base(context)
        {
        }

        protected override void FlowTaskAction()
        {
            throw new NotImplementedException();
        }

        protected override void TaskErrorAction(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override MessageBase GetHeartBeatMessage()
        {
            throw new NotImplementedException();
        }

        public override SlaveFlowTaskBase Next { get; protected set; }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}