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
    internal class RunTestProjectFlowTask : SlaveFlowTaskBase
    {
        private readonly ManualResetEvent _blockEvent;
        private Timer _wakeTimer;

        public RunTestProjectFlowTask(SlaveContext context) : base(context)
        {
            _blockEvent = new ManualResetEvent(false);
        }

        protected override void FlowTaskAction()
        {
            SendStartMessage();

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Start test project setup.");

            Context.State = RuntimeState.Running;

            SessionTaskEntity sessionTaskEntity = Context.SessionTaskEntity;

            sessionTaskEntity.InvokeSetUp();
            RuntimeState setUpState = sessionTaskEntity.GetSequenceTaskState(CommonConst.SetupIndex);
            // 如果SetUp执行失败，则执行TearDown，且配置所有序列为失败状态，并发送所有序列都失败的信息
            if (setUpState > RuntimeState.Success)
            {
                // 打印状态日志
                Context.LogSession.Print(LogLevel.Error, Context.SessionId, "Run testproject setup failed.");
                for (int i = 0; i < sessionTaskEntity.SequenceCount; i++)
                {
                    sessionTaskEntity.GetSequenceTaskEntity(i).State = RuntimeState.Failed;

                    FailedInfo failedInfo = new FailedInfo(Context.I18N.GetStr("SetUpFailed"), FailedType.SetUpFailed);
                    SequenceStatusInfo statusInfo = new SequenceStatusInfo(i, ModuleUtils.GetSequenceStack(i, 0), 
                        StatusReportType.Failed, StepResult.NotAvailable, failedInfo)
                    {
                        ExecutionTime = DateTime.Now,
                        ExecutionTicks = -1,
                        CoroutineId = 0
                    };
                    Context.StatusQueue.Enqueue(statusInfo);
                }

                sessionTaskEntity.InvokeTearDown();
                // 打印状态日志
                Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Teardown execution over.");

                return;
            }

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Testproject setup execution over.");

            this._wakeTimer = new Timer(WakeThreadWhenCtrlMessageCome, null, Constants.WakeTimerInterval, 
                Constants.WakeTimerInterval);
            _blockEvent.WaitOne();

            if (null == Context.CtrlStartMessage)
            {
                Context.LogSession.Print(LogLevel.Error, Context.SessionId,
                    "Receive CtrlMessage without RunTearDown parameter.");
            }

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Teardown execution start.");

            sessionTaskEntity.InvokeTearDown();

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Teardown execution over.");

            SendOverMessage();

            Context.State = RuntimeState.Over;
            this.Next = null;
        }

        private void WakeThreadWhenCtrlMessageCome(object state)
        {
            if ((null != Context.CtrlStartMessage && Context.CtrlStartMessage.Params.ContainsKey("RunTearDown") &&
                Context.CtrlStartMessage.Params["RunTearDown"].Equals(true.ToString())) || 
                Context.Cancellation.IsCancellationRequested)
            {
                _blockEvent.Set();
                _wakeTimer.Dispose();
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
            _wakeTimer?.Dispose();
            _blockEvent.Reset();
            _blockEvent.Dispose();
        }
    }
}