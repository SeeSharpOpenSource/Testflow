using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class RmtGenProcessFlowTask : SlaveFlowTaskBase
    {
        public RmtGenProcessFlowTask(SlaveContext context) : base(context)
        {
        }

        protected override void FlowTaskAction()
        {
            Context.State = RuntimeState.Idle;

            LocalMessageQueue<MessageBase> messageQueue = Context.MessageTransceiver.MessageQueue;
            // 首先接收RmtGenMessage
            MessageBase message = messageQueue.WaitUntilMessageCome();
            RmtGenMessage rmtGenMessage = (RmtGenMessage)message;

            if (null == rmtGenMessage)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidMessageReceived,
                    Context.I18N.GetFStr("InvalidMessageReceived", message.GetType().Name));
            }

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
            // ignore
        }

        public override void AbortAction()
        {
            // ignore
        }

        public override MessageBase GetHeartBeatMessage()
        {
            return new TestGenMessage(MessageNames.TestGenName, Context.SessionId, CommonConst.PlatformSession,
                GenerationStatus.Idle);
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {

        }
    }
}