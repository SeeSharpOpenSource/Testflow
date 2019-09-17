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
                // 打印状态日志
                Context.LogSession.Print(LogLevel.Error, Context.SessionId, "Run setup failed.");
                SendSetupFailedEvents(sessionTaskEntity);
            }
            else
            {
                RunSequences(sessionTaskEntity);
            }

            sessionTaskEntity.InvokeTearDown();

            Context.State = RuntimeState.Over;
            this.Next = null;

            SendOverMessage();

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Test execution over.");
        }

        private void SendSetupFailedEvents(SessionTaskEntity sessionTaskEntity)
        {
            for (int i = 0; i < sessionTaskEntity.SequenceCount; i++)
            {
                sessionTaskEntity.GetSequenceTaskEntity(i).State = RuntimeState.Failed;

                FailedInfo failedInfo = new FailedInfo(Context.I18N.GetStr("SetUpFailed"),
                    FailedType.SetUpFailed);
                SequenceStatusInfo statusInfo = new SequenceStatusInfo(i, ModuleUtils.GetSequenceStack(i),
                    StatusReportType.Failed, StepResult.NotAvailable,
                    failedInfo);
                Context.StatusQueue.Enqueue(statusInfo);
            }
        }

        private void RunSequences(SessionTaskEntity sessionTaskEntity)
        {
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
        }

        private void RunSingleSequence(object state)
        {
            try
            {
                int sequenceIndex = (int)state;
                Context.SessionTaskEntity.InvokeSequence(sequenceIndex);
            }
            finally
            {
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
        }

        protected override void TaskErrorAction(Exception ex)
        {
            StatusMessage errorMessage = new StatusMessage(MessageNames.ErrorStatusName, Context.State, Context.SessionId)
            {
                ExceptionInfo = new FailedInfo(ex, FailedType.RuntimeError),
                Index = Context.MsgIndex
            };
            Context.SessionTaskEntity.FillSequenceInfo(errorMessage, Context.I18N.GetStr("RuntimeError"));
            if (Context.GetProperty<bool>("EnablePerformanceMonitor"))
            {
                ModuleUtils.FillPerformance(errorMessage);
            }
            errorMessage.WatchData = Context.VariableMapper.GetReturnDataValues();
            Context.UplinkMsgProcessor.SendMessage(errorMessage, true);
        }

        public override void TaskAbortAction()
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
                    }
                    _sequenceThreads.Clear();
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
            if (Context.GetProperty<bool>("EnablePerformanceMonitor"))
            {
                ModuleUtils.FillPerformance(statusMessage);
            }
            return statusMessage;
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {
            _blockEvent?.Set();
            _blockEvent?.Dispose();
            if (null != _sequenceThreads && _sequenceThreads.Count > 0)
            {
                TaskAbortAction();
            }
        }
    }
}