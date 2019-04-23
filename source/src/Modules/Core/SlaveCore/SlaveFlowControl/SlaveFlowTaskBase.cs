using System;
using Testflow.Common;
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
                Context.UplinkMsgPacker.HeartbeatMsgGenerator = GetHeartBeatMessage;
                FlowTaskAction();
                Next.DoFlowTask();
            }
            catch (TestflowException ex)
            {
                // TODO
            }
            catch (Exception ex)
            {
                Context.State = RuntimeState.Error;
                // 失败后打印日志并发送错误信息
                Context.LogSession.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Runtime exception occured.");
                StatusMessage errorMessage = new StatusMessage(MessageNames.ErrorStatusName, Context.State, Context.SessionId)
                {
                    ExceptionInfo = new SequenceFailedInfo(ex),
                };

                Context.SessionTaskEntity.FillSequenceInfo(errorMessage, Context.I18N.GetStr("RuntimeError"));
                Context.UplinkMsgPacker.SendMessage(errorMessage);
                StopTaskAction();
            }
        }

        /// <summary>
        /// 完成当前流程的任务
        /// </summary>
        protected abstract void FlowTaskAction();

        protected abstract void StopTaskAction();

        public abstract void AbortAction();

        public abstract MessageBase GetHeartBeatMessage();

        public abstract SlaveFlowTaskBase Next { get; protected set; }

        public abstract void Dispose();
    }
}