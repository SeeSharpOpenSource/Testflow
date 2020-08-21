using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using Testflow.Usr;
using Testflow.MasterCore.Common;
using Testflow.Modules;
using Testflow.SlaveCore;

namespace Testflow.MasterCore.TestMaintain.Container
{
    internal class AppDomainRuntimeContainer : RuntimeContainer
    {
        private AppDomainLauncherParser _launcherParser;
        private readonly Thread _testThd;

        public AppDomainRuntimeContainer(int sessionId, ModuleGlobalInfo globalInfo,
            params object[] extraParam) : base(sessionId, globalInfo)
        {
//            RuntimeType runtimeType = globalInfo.ConfigData.GetProperty<RuntimeType>("RuntimeType");
            IsAvailable = true;
            _testThd = new Thread(StartLauncher)
            {
                Name = GetThreadName(),
                IsBackground = true
            };
        }

        public override void Start(string startConfigData)
        {
            _testThd.Start(startConfigData);
        }

        private void StartLauncher(object param)
        {
            string configStr = (string)param;
            AppDomainTestLauncher launcherInstance = null;
            try
            {
                _launcherParser = new AppDomainLauncherParser();
                launcherInstance = _launcherParser.CreateAppDomainLauncher(GlobalInfo, Session, configStr);
                launcherInstance.Start();
            }
            catch (ThreadAbortException ex)
            {
                ILogService logService = TestflowRunner.GetInstance().LogService;
                logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, "Appdomain thread aborted.");
            }
            catch (Exception ex)
            {
                ILogService logService = TestflowRunner.GetInstance().LogService;
                logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, ex,
                    "Exception raised in appdomain thread.");
            }
            finally
            {
                _launcherParser.LauncherOver();
                IsAvailable = false;
                launcherInstance?.Dispose();
            }
        }

        private int _diposedFlag = 0;
        public override void Dispose()
        {
            if (_diposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _diposedFlag, 1);
            Thread.MemoryBarrier();
            int timeout = GlobalInfo.ConfigData.GetProperty<int>("AbortTimeout")*2;
            if (_testThd.IsAlive && !_testThd.Join(timeout))
            {
                _testThd.Abort();
                IsAvailable = false;
                Thread.MemoryBarrier();
                OnRuntimeExited();
                _launcherParser?.Unload();
                _launcherParser = null;
            }
            
        }

        private string GetThreadName()
        {
            return $"TestflowSlaveThread{Session}";
        }

        private string GetAppDomainName()
        {
            return $"TestflowSlaveAppDomain{Session}";
        }
    }
}