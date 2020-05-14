using System;
using System.Diagnostics;
using System.Threading;
using Testflow.MasterCore.Common;
using Testflow.Runtime;

namespace Testflow.MasterCore.TestMaintain.Container
{
    internal class ProcessRuntimeContainer : RuntimeContainer
    {
        private Process _slaveProcess;

        private readonly ProcessStartInfo _startInfo;

        public ProcessRuntimeContainer(int session, ModuleGlobalInfo globalInfo,
            params object[] extraParam) : base(session, globalInfo)
        {
            string testflowHome = GlobalInfo.ConfigData.GetProperty<string>("TestFlowHome");
            string fileName = GetExecutiveFilePath(testflowHome, (RunnerPlatform) extraParam[0]);
            _startInfo = new ProcessStartInfo(fileName)
            {
                CreateNoWindow = true,
                WorkingDirectory = testflowHome
            };
        }

        private string GetExecutiveFilePath(string testflowHome, RunnerPlatform platform)
        {
            string fileName = platform == RunnerPlatform.X64
                ? Constants.X64SlaveRunnerFile
                : Constants.X86SlaveRunnerFile;
            return testflowHome + fileName;
        }

        public override void Start(string startConfigData)
        {
            _startInfo.Arguments = startConfigData;
            _slaveProcess = new Process()
            {
                StartInfo = _startInfo,
                EnableRaisingEvents = true
            };
            _slaveProcess.Exited += SlaveProcessOnExited;
            IsAvailable = true;
            Thread.MemoryBarrier();
            _slaveProcess.Start();
        }

        private void SlaveProcessOnExited(object sender, EventArgs eventArgs)
        {
            if (IsAvailable)
            {
                OnRuntimeExited();
            }
            IsAvailable = false;
        }

        private int _disposedFlag = 0;

        public override void Dispose()
        {
            if (_disposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _disposedFlag, 1);
            Thread.MemoryBarrier();
            _slaveProcess.Exited -= SlaveProcessOnExited;
            int timeout = GlobalInfo.ConfigData.GetProperty<int>("AbortTimeout") * 2;
            if (null != _slaveProcess && !_slaveProcess.HasExited && !_slaveProcess.WaitForExit(timeout))
            {
                _slaveProcess.Kill();
                OnRuntimeExited();
            }
            _slaveProcess?.Dispose();
        }
    }
}