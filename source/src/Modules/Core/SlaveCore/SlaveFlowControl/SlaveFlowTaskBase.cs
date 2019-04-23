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

        public SlaveFlowTaskBase(SlaveContext context)
        {
            this.Context = context;
        }

        public void DoFlowTask()
        {
            try
            {
                FlowTaskAction();
            }
            catch (TestflowException ex)
            {
                // TODO
            }
            catch (Exception ex)
            {
                Context.LogSession.Print(LogLevel.Fatal, CommonConst.PlatformLogSession, ex, "Runtime exception occured.");
                StatusMessage errorMessage = new StatusMessage(MessageNames.ErrorStatusName, RuntimeState.Error,
                    Context.SessionId)
                {
                    
                };
                errorMessage.ExceptionInfo = new SequenceFailedInfo(ex);




                Context.MessageTransceiver.SendMessage(errorMessage);

            }
            Next.DoFlowTask();
        }

        /// <summary>
        /// 完成当前流程的任务
        /// </summary>
        protected abstract void FlowTaskAction();

        protected abstract void StopTaskAction();

        public abstract SlaveFlowTaskBase Next { get; protected set; }

        public abstract void Dispose();
    }
}