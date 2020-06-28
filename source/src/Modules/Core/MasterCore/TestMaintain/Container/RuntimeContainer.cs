using System;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.TestMaintain.Container
{
    /// <summary>
    /// 测试运行时容器
    /// </summary>
    internal abstract class RuntimeContainer : IDisposable
    {
        public static RuntimeContainer CreateContainer(int session, RuntimePlatform platform,
            ModuleGlobalInfo globalInfo, params object[] extraParam)
        {
            switch (platform)
            {
                case RuntimePlatform.Clr:
                    AppDomain.MonitoringIsEnabled = true;
                    return new AppDomainRuntimeContainer(session, globalInfo, extraParam);
                    break;
                case RuntimePlatform.LocalProcess:
                    return new ProcessRuntimeContainer(session, globalInfo, extraParam);
                    break;
                case RuntimePlatform.RemoteHost:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public event Action RuntimeExited;

        public int Session { get; }
        protected ModuleGlobalInfo GlobalInfo { get; }

        public bool HostReady { get; set; }

        private int _avaiableFlag = 0;
        public bool IsAvailable
        {
            get { return _avaiableFlag != 0; }
            protected set { Thread.VolatileWrite(ref _avaiableFlag, value ? 1 : 0); }
        }

        protected RuntimeContainer(int session, ModuleGlobalInfo globalInfo)
        {
            this.Session = session;
            this.GlobalInfo = globalInfo;
            this.HostReady = false;
        }

        protected void OnRuntimeExited()
        {
            RuntimeExited?.Invoke();
        }

        public abstract void Start(string startConfigData);

        public abstract void Dispose();
    }
}