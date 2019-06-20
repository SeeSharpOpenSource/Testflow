using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class RunAllSequenceFlowTask : SlaveFlowTaskBase
    {
        private ManualResetEvent _blockEvent;
        private int _overTaskCount;
        private List<Thread> _sequenceThreads;
        
        public RunAllSequenceFlowTask(SlaveContext context) : base(context)
        {
        }

        protected override void FlowTaskAction()
        {
            SendStartMessage();

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Start run all sequence.");

            Context.State = RuntimeState.Running;

            SessionTaskEntity sessionTaskEntity = Context.SessionTaskEntity;

            sessionTaskEntity.InvokeSetUp();
            RuntimeState setUpState = sessionTaskEntity.GetSequenceTaskState(CommonConst.SetupIndex);
            // 如果SetUp执行失败，则执行TearDown，且配置所有序列为失败状态，并发送所有序列都失败的信息
            if (setUpState > RuntimeState.Success)
            {
                sessionTaskEntity.InvokeTearDown();
                for (int i = 0; i < sessionTaskEntity.SequenceCount; i++)
                {
                    sessionTaskEntity.GetSequenceTaskEntity(i).State = RuntimeState.Failed;

                    TaskFailedException failedException = new TaskFailedException(i, Context.I18N.GetStr("SetUpFailed"));
                    SequenceStatusInfo statusInfo = new SequenceStatusInfo(i, ModuleUtils.GetSequenceStack(i), StatusReportType.Failed, StepResult.NotAvailable,
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
                    _blockEvent = new ManualResetEvent(false);
                    _blockEvent.Reset();
                    Thread.VolatileWrite(ref _overTaskCount, 0);
                    _sequenceThreads = new List<Thread>(sessionTaskEntity.SequenceCount);
                    for (int i = 0; i < sessionTaskEntity.SequenceCount; i++)
                    {
                        Thread taskThread = new Thread(RunSingleSequence)
                        {
                            IsBackground = true,
                            Name = string.Format(Constants.TaskThreadNameFormt, Context.SessionId, i)
                        };
                        _sequenceThreads.Add(taskThread);
                    }
                    ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
                    {
                        Thread.Sleep(10);
                        for (int i = 0; i < _sequenceThreads.Count; i++)
                        {
                            _sequenceThreads[i].Start(i);
                        }
                    }));
                    Thread.MemoryBarrier();
                    _blockEvent.WaitOne();
                    _blockEvent.Dispose();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            sessionTaskEntity.InvokeTearDown();

            Context.State = RuntimeState.Over;
            this.Next = null;

            SendOverMessage();

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Test execution over.");
        }

        private void RunSingleSequence(object state)
        {
            int sequenceIndex = (int) state;
            Context.SessionTaskEntity.InvokeSequence(sequenceIndex);
            lock (_blockEvent)
            {
                Interlocked.Increment(ref _overTaskCount);
                Thread.VolatileRead(ref _overTaskCount);
                if (_overTaskCount == Context.SessionTaskEntity.SequenceCount)
                {
                    _blockEvent.Set();
                }
            }
        }

        protected override void TaskErrorAction(Exception ex)
        {
//            // 更新未执行结束的序列状态为Error.
//            for (int i = 0; i < Context.SessionTaskEntity.SequenceCount; i++)
//            {
//                SequenceTaskEntity sequenceTaskEntity = Context.SessionTaskEntity.GetSequenceTaskEntity(i);
//                if (sequenceTaskEntity.State <= RuntimeState.AbortRequested)
//                {
//                    sequenceTaskEntity.State = RuntimeState.Error;
//                }
//            }
            StatusMessage errorMessage = new StatusMessage(MessageNames.ErrorStatusName, Context.State, Context.SessionId)
            {
                ExceptionInfo = new SequenceFailedInfo(ex),
                Index = Context.MsgIndex
            };
            Context.SessionTaskEntity.FillSequenceInfo(errorMessage, Context.I18N.GetStr("RuntimeError"));
            Context.UplinkMsgProcessor.SendMessage(errorMessage);
            ModuleUtils.FillPerformance(errorMessage);
            errorMessage.WatchData = Context.VariableMapper.GetReturnDataValues();
        }

        protected override void TaskAbortAction()
        {
            switch (Context.ExecutionModel)
            {
                case ExecutionModel.SequentialExecution:
                    break;
                case ExecutionModel.ParallelExecution:
                    foreach (Thread sequenceThread in _sequenceThreads)
                    {
                        if (sequenceThread.IsAlive)
                        {
                            sequenceThread.Abort();
                        }
                        _sequenceThreads.Clear();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            base.TaskAbortAction();
        }

        public override MessageBase GetHeartBeatMessage()
        {
            StatusMessage statusMessage = new StatusMessage(MessageNames.HeartBeatStatusName, Context.State, Context.SessionId)
            {
                Index = Context.MsgIndex
            };
            SessionTaskEntity sessionTaskEntity = Context.SessionTaskEntity;
            sessionTaskEntity.FillSequenceInfo(statusMessage);

            ModuleUtils.FillPerformance(statusMessage);
//            statusMessage.WatchData = Context.VariableMapper.GetWatchDataValues();

            return statusMessage;
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {
            _blockEvent?.Reset();
            _blockEvent?.Dispose();
            if (null != _sequenceThreads)
            {
                TaskAbortAction();
            }
        }
    }
}