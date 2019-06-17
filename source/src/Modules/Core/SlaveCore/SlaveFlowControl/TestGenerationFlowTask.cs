﻿using System;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class TestGenerationFlowTask : SlaveFlowTaskBase
    {
        public TestGenerationFlowTask(SlaveContext context) : base(context)
        {
        }
        /// <summary>
        /// <localize>
        ///<zh-CHS>中文</zh-CHS>
        ///<en>English</en>
        ///</localize>
        /// </summary>
        protected override void FlowTaskAction()
        {
            Context.State = RuntimeState.TestGen;

            // 发送测试开始消息
            TestGenMessage testGenStartMessage = new TestGenMessage(MessageNames.TestGenName, Context.SessionId,
                CommonConst.PlatformSession, GenerationStatus.InProgress)
            {
                Index = Context.MsgIndex
            };
            Context.UplinkMsgProcessor.SendMessage(testGenStartMessage);

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Test generation started.");

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId, "Construct variable mapper start.");

            // 构造变量映射器
            Context.VariableMapper = new VariableMapper(Context);

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId, "Construct AssemblyInvoker start.");

            switch (Context.SequenceType)
            {
                case RunnerType.TestProject:
                    ITestProject testProject = (ITestProject)Context.Sequence;
                    Context.TypeInvoker = new AssemblyInvoker(Context, testProject.Assemblies,
                        testProject.TypeDatas);
                    break;
                case RunnerType.SequenceGroup:
                    ISequenceGroup sequenceGroup = (ISequenceGroup)Context.Sequence;
                    Context.TypeInvoker = new AssemblyInvoker(Context, sequenceGroup.Assemblies,
                        sequenceGroup.TypeDatas);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            
            // 打印状态日志
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId, "Load assemblies.");

            // 加载用到的程序集
            Context.TypeInvoker.LoadAssemblyAndType();

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId, "Create session task entity start.");

            // 创建序列执行实体
            Context.SessionTaskEntity = new SessionTaskEntity(Context);
            // 生成执行实体的调用对象
            Context.SessionTaskEntity.Generate();

            // 发送生成结束的消息
            TestGenMessage testGenOverMessage = new TestGenMessage(MessageNames.TestGenName, Context.SessionId,
                CommonConst.PlatformSession, GenerationStatus.Success)
            {
                Index = Context.MsgIndex
            };
            Context.UplinkMsgProcessor.SendMessage(testGenOverMessage);

            // 发送远程运行器生成结束的消息
            RmtGenMessage rmtGenMessage = new RmtGenMessage(MessageNames.UpRmtGenMsgName, Context.SessionId,
                RunnerType.SequenceGroup);
            rmtGenMessage.Params.Add("MsgType", "Success");
            Context.UplinkMsgProcessor.SendMessage(rmtGenMessage);

            // 打印状态日志
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, "Test generation over.");

            this.Next = new CtrlStartProcessFlowTask(Context);
        }

        protected override void TaskAbortAction()
        {
            TestGenMessage testGenMessage = new TestGenMessage(MessageNames.TestGenName, Context.SessionId,
                CommonConst.PlatformLogSession, GenerationStatus.Failed)
            {
                Index = Context.MsgIndex
            };
            Context.UplinkMsgProcessor.SendMessage(testGenMessage);

            // 发送远程运行器生成失败的消息
            RmtGenMessage rmtGenMessage = new RmtGenMessage(MessageNames.UpRmtGenMsgName, Context.SessionId,
                RunnerType.SequenceGroup);
            rmtGenMessage.Params.Add("MsgType", "Failed");
            Context.UplinkMsgProcessor.SendMessage(rmtGenMessage);

            base.TaskAbortAction();
        }

        protected override void TaskErrorAction(Exception ex)
        {
            Context.LogSession.Print(LogLevel.Error, CommonConst.PlatformLogSession, "Test Generation failed.");
            TestGenMessage testGenFailMessage = new TestGenMessage(MessageNames.TestGenName, Context.SessionId,
                CommonConst.PlatformSession, GenerationStatus.Failed)
            {
                Index = Context.MsgIndex
            };
            Context.UplinkMsgProcessor.SendMessage(testGenFailMessage);

            // 发送远程运行器生成失败的消息
            RmtGenMessage rmtGenMessage = new RmtGenMessage(MessageNames.UpRmtGenMsgName, Context.SessionId,
                RunnerType.SequenceGroup);
            rmtGenMessage.Params.Add("MsgType", "Failed");
            Context.UplinkMsgProcessor.SendMessage(rmtGenMessage);
        }
        
        public override MessageBase GetHeartBeatMessage()
        {
            // 发送生成结束的消息
            TestGenMessage testGenOverMessage = new TestGenMessage(MessageNames.TestGenName, Context.SessionId,
                CommonConst.PlatformSession, GenerationStatus.InProgress);
            return testGenOverMessage;
        }

        public override SlaveFlowTaskBase Next { get; protected set; }

        public override void Dispose()
        {
        }
    }
}