using System;
using Testflow.Common;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Runner;
using Testflow.SlaveCore.Runner.Model;
using Testflow.SlaveCore.SlaveFlowControl;

namespace Testflow.SlaveCore
{
    internal class SlaveController : IDisposable
    {
        private readonly MessageTransceiver _transceiver;
        private readonly SlaveContext _context;
        private TestRunner _runner;

        public SlaveController(SlaveContext context)
        {
            this._transceiver = context.MessageTransceiver;
            this._context = context;
        }

        public void StartslaveTask()
        {
            

        }

        private void TaskFlowAction()
        {
            
        }

        private void StartTestWork(SessionTaskEntity sessionExecutionModel)
        {
            _runner = TestRunner.CreateRunner(_context);
            _runner.Start(sessionExecutionModel);
        }

        private void StateMonitoring()
        {
            throw new NotImplementedException();
        }

        private void SendGenerationOverMessage()
        {
            throw new NotImplementedException();
        }

        private void StartDownLinkMessageListening()
        {
            throw new NotImplementedException();
        }

        public void StopSlaveTask()
        {

        }

        public void Dispose()
        {
            _transceiver.Dispose();
            _context.Dispose();
        }
    }
}