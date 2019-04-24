using System;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.Runner
{
    internal abstract class TestRunner : IDisposable
    {
        // 根节点线程。在序列执行时运行所有测试，在并行执行时运行SetUp和TearDown
        private readonly SlaveContext _context;
        
        public static TestRunner CreateRunner(SlaveContext context)
        {
            TestRunner controller = null;
            switch (context.ExecutionModel)
            {
                case ExecutionModel.SequentialExecution:
                    controller =  new SequentialTestRunner(context);
                    break;
                case ExecutionModel.ParallelExecution:
                    controller = new ParallelTestRunner(context);
                    break;
                default:
                    throw new InvalidProgramException();
            }
            return controller;
        }

        protected TestRunner(SlaveContext context)
        {
            _context = context;
        }

        public abstract void Start(SessionTaskEntity sessionExecutionModel);

        private void HandleDownlinkMessage()
        {
            try
            {
                // TODO
            }
            catch (ApplicationException ex)
            {
                StatusMessage statusMessage = new StatusMessage(MessageNames.ErrorStatusName, RuntimeState.Error, _context.SessionId);
                statusMessage.ExceptionInfo = new SequenceFailedInfo(ex);
            }
        }

        public void FillStatusMessageInfo(StatusMessage message)
        {
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}