using System;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    /// <summary>
    /// slave端流程控制元素
    /// </summary>
    internal abstract class SlaveFlowTaskBase : IDisposable
    {
        public static SlaveFlowTaskBase GetFlowTaskEntrance(SlaveContext context)
        {
            return new RmtGenProcessFlowTask(context);
        }

        protected readonly SlaveContext Context;

        protected SlaveFlowTaskBase(SlaveContext context)
        {
            this.Context = context;
        }

        public void DoFlowTask()
        {
            try
            {
                // 配置心跳包生成委托
                Context.UplinkMsgProcessor.HeartbeatMsgGenerator = GetHeartBeatMessage;

                // 打印状态日志
                Context.LogSession.Print(LogLevel.Debug, Context.SessionId, $"{this.GetType().Name} task action started.");

                FlowTaskAction();

                // 打印状态日志
                Context.LogSession.Print(LogLevel.Debug, Context.SessionId, $"{this.GetType().Name} task action over.");
                
                Next?.DoFlowTask();
            }
            catch (ThreadAbortException ex)
            {
                Context.State = RuntimeState.Abort;
                Context.LogSession.Print(LogLevel.Warn, CommonConst.PlatformSession, ex, "Task aborted.");
                TaskAbortAction();
            }
            catch (Exception ex)
            {
                Context.State = RuntimeState.Error;
                // 失败后打印日志并发送错误信息
                Context.LogSession.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex,
                    "Runtime exception occured.");
                TaskErrorAction(ex);
                // 发送运行时异常错误
                RuntimeErrorMessage errorMessage = new RuntimeErrorMessage(Context.SessionId, ex)
                {
                    Index = Context.MsgIndex
                };
                Context.UplinkMsgProcessor.SendMessage(errorMessage, true);
            }
        }

        /// <summary>
        /// 流程执行的代码
        /// </summary>
        protected abstract void FlowTaskAction();

        /// <summary>
        /// 任务中间出现异常的处理
        /// </summary>
        protected abstract void TaskErrorAction(Exception ex);

        /// <summary>
        /// 返回心跳消息的代码
        /// </summary>
        public abstract MessageBase GetHeartBeatMessage();

        /// <summary>
        /// 下一个流程节点
        /// </summary>
        public abstract SlaveFlowTaskBase Next { get; protected set; }

        public abstract void Dispose();

        /// <summary>
        /// 任务被Abort时处理的代码
        /// </summary>
        protected virtual void TaskAbortAction()
        {
            ControlMessage abortMessage = new ControlMessage(MessageNames.CtrlAbort, Context.SessionId)
            {
                Index = Context.MsgIndex,
            };
            abortMessage.Params.Add("AbortSuccess", true.ToString());
            Context.UplinkMsgProcessor.SendMessage(abortMessage, true);
        }

        protected void SendStartMessage()
        {
            StatusMessage statusMessage = new StatusMessage(MessageNames.StartStatusName, RuntimeState.Running,
                Context.SessionId)
            {
                Index = Context.MsgIndex
            };
            ModuleUtils.FillPerformance(statusMessage);
            Context.UplinkMsgProcessor.SendMessage(statusMessage, false);
        }

        protected void SendOverMessage()
        {
            StatusMessage statusMessage = new StatusMessage(MessageNames.ResultStatusName, RuntimeState.Over,
                Context.SessionId)
            {
                Index = Context.MsgIndex
            };
            ModuleUtils.FillPerformance(statusMessage);
            statusMessage.WatchData = Context.VariableMapper.GetReturnDataValues(Context.Sequence);
            Context.UplinkMsgProcessor.SendMessage(statusMessage, true);
        }
    }
}