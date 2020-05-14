using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Testflow.Usr;
using Testflow.MasterCore.Common;
using Testflow.Modules;
using Testflow.SlaveCore;

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
            _appDomain = AppDomain.CreateDomain(GetAppDomainName());
            _appDomain.DomainUnload += AppDomainOnDomainUnload;
            IsAvailable = true;
            _testThd = new Thread(StartLauncher)
            {
                Name = GetThreadName(),
                IsBackground = true
            };
        }

        private void AppDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            if (IsAvailable)
            {
                OnRuntimeExited();
                IsAvailable = false;
            }
        }

        public override void Start(string startConfigData)
        {
            _testThd.Start(startConfigData);
        }

        private void StartLauncher(object param)
        {
            string configStr = (string)param;
            Type launcherType = typeof(AppDomainTestLauncher);
            string launcherFullName = $"{launcherType.Namespace}.{launcherType.Name}";

            try
            {
                _appDomain.Load(launcherType.Assembly.GetName());
                AppDomainTestLauncher launcherInstance = (AppDomainTestLauncher) _appDomain.CreateInstanceFromAndUnwrap(
                    launcherType.Assembly.Location, launcherFullName, false, BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new object[] {configStr}, CultureInfo.CurrentCulture, null);
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
            }
        }

        public override void Dispose()
        {
            int timeout = GlobalInfo.ConfigData.GetProperty<int>("AbortTimeout")*2;
            _appDomain.DomainUnload -= AppDomainOnDomainUnload;
            if (_testThd.IsAlive && !_testThd.Join(timeout))
            {
                _testThd.Abort();
                IsAvailable = false;
                Thread.MemoryBarrier();
                OnRuntimeExited();
            }
            AppDomain.Unload(_appDomain);
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