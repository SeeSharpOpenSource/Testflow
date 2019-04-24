using System;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class RunAllSequenceFlowTask : SlaveFlowTaskBase
    {
        private AutoResetEvent _blockEvent;
        private int _overTaskCount;

        public RunAllSequenceFlowTask(SlaveContext context) : base(context)
        {
        }

        protected override void FlowTaskAction()
        {
            Context.State = RuntimeState.Running;

            SessionTaskEntity sessionTaskEntity = Context.SessionTaskEntity;

            sessionTaskEntity.InvokeSetUp();
            RuntimeState setUpState = sessionTaskEntity.GetSequenceTaskState(CommonConst.SetupIndex);
            // 如果SetUp执行失败，则发送所有序列都失败的信息
            if (setUpState > RuntimeState.Success)
            {
                for (int i = 0; i < sessionTaskEntity.SequenceCount; i++)
                {
                    TaskFailedException failedException = new TaskFailedException(i, Context.I18N.GetStr("SetUpFailed"));
                    SequenceStatusInfo statusInfo = new SequenceStatusInfo(i, null, StatusReportType.Failed,
                        failedException);
                    Context.StatusQueue.Enqueue(statusInfo);
                }
                return;
            }

            switch (Context.ExecutionModel)
            {
                case ExecutionModel.SequentialExecution:
                    for (int i = 0; i < sessionTaskEntity.SequenceCount; i++)
                    {
                        Context.SessionTaskEntity.InvokeSequence(i);
                    }
                    break;
                case ExecutionModel.ParallelExecution:
                    _blockEvent = new AutoResetEvent(false);
                    Thread.VolatileWrite(ref _overTaskCount, 0);
                    for (int i = 0; i < sessionTaskEntity.SequenceCount; i++)
                    {
                        ThreadPool.QueueUserWorkItem(RunSingleSequence, i);
                    }
                    _blockEvent.WaitOne();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            sessionTaskEntity.InvokeTearDown();

            Context.State = RuntimeState.Over;
            this.Next = null;
        }

        private void RunSingleSequence(object state)
        {
            int sequenceIndex = (int) state;
            Context.SessionTaskEntity.InvokeSequence(sequenceIndex);
            Interlocked.Increment(ref _overTaskCount);
            if (_overTaskCount == Context.SessionTaskEntity.SequenceCount)
            {
                _blockEvent.Set();
            }
        }

        protected override void TaskErrorAction(Exception ex)
        {
            StatusMessage errorMessage = new StatusMessage(MessageNames.ErrorStatusName, Context.State, Context.SessionId)
            {
                ExceptionInfo = new SequenceFailedInfo(ex),
            };
            Context.SessionTaskEntity.FillSequenceInfo(errorMessage, Context.I18N.GetStr("RuntimeError"));
            Context.UplinkMsgPacker.SendMessage(errorMessage);
            ;
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