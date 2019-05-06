using System;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class CtrlStartProcessFlowTask : SlaveFlowTaskBase
    {
        public CtrlStartProcessFlowTask(SlaveContext context) : base(context)
        {
        }

        protected override void FlowTaskAction()
        {
            // 发送测试结束消息
            Context.State = RuntimeState.StartIdle;

            ControlMessage message;
            while (null == (message = Context.CtrlStartMessage))
            {
                Thread.Sleep(10);
            }
            if (!MessageNames.CtrlStart.Equals(message.Name))
            {
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidMessageReceived,
                    Context.I18N.GetFStr("InvalidMessageReceived", message.GetType().Name));
            }

            if (IsOptionEnabled(message, "RunAll"))
            {
                this.Next = new RunAllSequenceFlowTask(Context);
            }
            else if (message.Params.ContainsKey("RunSequence"))
            {
                this.Next = new RunSingleSequenceFlowTask(int.Parse(message.Params["RunSequence"]), Context);
            }
            else if (IsOptionEnabled(message, "RunSetup"))
            {
                this.Next = new RunTestProjectFlowTask(Context);
            }
            else
            {
                Context.LogSession.Print(LogLevel.Fatal, CommonConst.PlatformSession, 
                    "Control start message does not contain any valid start params");
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidMessageReceived,
                    Context.I18N.GetFStr("InvalidMessageReceived", message.GetType().Name));
            }
            StatusMessage startMessage = new StatusMessage(MessageNames.StartStatusName, RuntimeState.Running,
                Context.SessionId)
            {
                Index = Context.MsgIndex
            };
            Context.UplinkMsgProcessor.SendMessage(startMessage);
            
            Context.CtrlStartMessage = null;
        }

        protected override void TaskErrorAction(Exception ex)
        {
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

        private bool IsOptionEnabled(ControlMessage message, string option)
        {
            return message.Params.ContainsKey(option) && true.ToString().Equals(message.Params[option]);
        }
        
        public override MessageBase GetHeartBeatMessage()
        {
            return new StatusMessage(MessageNames.HearBeatStatusName, Context.State, Context.SessionId);
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}