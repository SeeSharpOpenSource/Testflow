using System;
using System.Threading;
using Testflow.Usr;
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

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId, "Wait for RmtGenMessage.");

            RmtGenMessage rmtGenMessage;
            // 等待接收到RmtGenMessage为止
            while (null == (rmtGenMessage = Context.RmtGenMessage))
            {
                Thread.Sleep(10);
            }
            // 打印状态日志
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId, "RmtGenMessage received.");

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
            Context.RmtGenMessage = null;

            this.Next = new TestGenerationFlowTask(Context);
        }

        protected override void TaskAbortAction()
        {
            TestGenMessage testGenMessage = new TestGenMessage(MessageNames.TestGenName, Context.SessionId,
                CommonConst.PlatformLogSession, GenerationStatus.Failed)
            {
                Index = Context.MsgIndex
            };
            Context.UplinkMsgProcessor.SendMessage(testGenMessage);
            base.TaskAbortAction();
        }

        protected override void TaskErrorAction(Exception ex)
        {
            // ignore
        }
        
        public override MessageBase GetHeartBeatMessage()
        {
            return new TestGenMessage(MessageNames.TestGenName, Context.SessionId, CommonConst.PlatformSession,
                GenerationStatus.Idle)
            {
                Index = Context.MsgIndex
            };
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {

        }
    }
}