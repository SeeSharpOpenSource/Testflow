using System.Threading;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.SlaveFlowControl
{
    internal class StatusMonitor
    {
        private Thread _testRuningThread;

        private readonly SlaveContext _context;
        public StatusMonitor(SlaveContext context)
        {
            this._context = context;
        }

        public void StartMonitoring()
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
            throw new System.NotImplementedException();
        }
    }
}