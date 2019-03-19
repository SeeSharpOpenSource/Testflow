using System;
using System.Threading;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
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
            _appDomain = AppDomain.CreateDomain(GetAppDomainName());
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
            Type launcherType = typeof(AppDomainTestLauncher);
            string launcherFullName = $"{launcherType.Namespace}.{launcherType.Name}";
            AppDomainTestLauncher launcherInstance = (AppDomainTestLauncher)_appDomain.CreateInstanceAndUnwrap(
                launcherType.Assembly.GetName().Name, launcherFullName, new object[] { configStr });
            launcherInstance.Start();
        }

        public override void Dispose()
        {
            if (ThreadState.Running == _testThd.ThreadState)
            {
                bool over = _testThd.Join(200);
                if (!over)
                {
                    _testThd.Abort();
                }
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