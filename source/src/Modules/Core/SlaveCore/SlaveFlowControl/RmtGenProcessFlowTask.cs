using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class RmtGenProcessFlowTask : SlaveFlowTaskBase
    {
        public RmtGenProcessFlowTask(SlaveContext context) : base(context)
        {
        }

        protected override void FlowTaskAction()
        {
            LocalMessageQueue<MessageBase> messageQueue = Context.MessageTransceiver.MessageQueue;
            // 首先接收RmtGenMessage
            MessageBase message = messageQueue.WaitUntilMessageCome();
            RmtGenMessage rmtGenMessage = (RmtGenMessage)message;

            SequenceManager.SequenceManager sequenceManager = new SequenceManager.SequenceManager();
            Context.SequenceType = rmtGenMessage.SequenceType;
            if (rmtGenMessage.SequenceType == RunnerType.TestProject)
            {
                ITestProject testProject = sequenceManager.RuntimeDeserializeTestProject(rmtGenMessage.Sequence);
                Context.Sequence = testProject;
                Context.ExecutionModel = ExecutionModel.SequentialExecution;
            }
            else
            {
                ISequenceGroup sequenceGroup = sequenceManager.RuntimeDeserializeSequenceGroup(rmtGenMessage.Sequence);
                Context.Sequence = sequenceGroup;
                Context.ExecutionModel = sequenceGroup.ExecutionModel;
            }

            sequenceManager.Dispose();

            this.Next = new TestGenerationFlowTask(Context);
        }

        protected override void StopTaskAction()
        {
            throw new System.NotImplementedException();
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {

        }
    }
}