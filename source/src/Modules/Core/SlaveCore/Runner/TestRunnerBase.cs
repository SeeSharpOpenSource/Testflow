using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Runtime;
using Testflow.SlaveCore.Controller;

namespace Testflow.SlaveCore.Runner
{
    internal abstract class TestRunnerBase : IDisposable
    {
        // 根节点线程。在序列执行时运行所有测试，在并行执行时运行SetUp和TearDown
        private Thread _testRuningThread;
        private readonly SlaveContext _context;
        private readonly SlaveController _controller;

        public TestRunnerBase(SlaveContext context)
        {
            _context = context;
            _controller = context.Controller;
        }

        public void Start()
        {
            _testRuningThread = new Thread(HandleDownlinkMessage)
            {
                IsBackground = true,
                Name = "RunnerRootThread"
            };
            _testRuningThread.Start();
        }

        private void HandleDownlinkMessage()
        {
            try
            {

            }
            catch (ApplicationException ex)
            {
                StatusMessage statusMessage = new StatusMessage(MessageNames.ErrorStatusName, RuntimeState.Error,
                    _context.SessionId);
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