using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using Testflow.Usr;
using Testflow.MasterCore.Common;
using Testflow.Modules;

namespace Testflow.MasterCore.TestMaintain.Container
{
    internal class AppDomainRuntimeContainer : RuntimeContainer
    {
        private readonly AppDomain _appDomain;
        private readonly Thread _testThd;

        public AppDomainRuntimeContainer(int sessionId, ModuleGlobalInfo globalInfo,
            params object[] extraParam) : base(sessionId, globalInfo)
        {
//            RuntimeType runtimeType = globalInfo.ConfigData.GetProperty<RuntimeType>("RuntimeType");
            string testflowHome = globalInfo.ConfigData.GetProperty<string>("TestflowHome");
            AppDomainSetup domainSetup = new AppDomainSetup()
            {
                ApplicationBase = testflowHome
            };
            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            _appDomain = AppDomain.CreateDomain(GetAppDomainName(), evidence, domainSetup);
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
            dynamic launcherInstance = null;
            try
            {
                //                _appDomain.Load(launcherType.Assembly.GetName());
                string testflowHome = GlobalInfo.ConfigData.GetProperty<string>("TestflowHome");
                string corePath = $"{testflowHome}SlaveCore.dll";
                launcherInstance = _appDomain.CreateInstanceFromAndUnwrap(
                    corePath, Constants.AppDomainLauncherName, false, BindingFlags.Instance | BindingFlags.Public,
                    null, new object[] { configStr }, CultureInfo.CurrentCulture, null);
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