using System;
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
                    return new AppDomainRuntimeContainer(session, globalInfo, extraParam);
                    break;
                case RuntimePlatform.Common:
                    return new ProcessRuntimeContainer(session, globalInfo, extraParam);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public int Session { get; }
        protected ModuleGlobalInfo GlobalInfo { get; }

        public bool HostReady { get; set; }

        protected RuntimeContainer(int session, ModuleGlobalInfo globalInfo)
        {
            this.Session = session;
            this.GlobalInfo = globalInfo;
            this.HostReady = false;
        }

        public abstract void Start(string startConfigData);

        public abstract void Dispose();
    }
}