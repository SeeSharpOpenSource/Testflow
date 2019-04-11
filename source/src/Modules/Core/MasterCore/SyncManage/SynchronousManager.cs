using System;
using Testflow.CoreCommon.Messages;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;

namespace Testflow.MasterCore.SyncManage
{
    /// <summary>
    /// 资源同步管理模块
    /// </summary>
    internal class SynchronousManager : IMessageHandler, IDisposable
    {
        private readonly ModuleGlobalInfo _globalInfo;
        public SynchronousManager(ModuleGlobalInfo globalInfo)
        {
            this._globalInfo = globalInfo;
        }

        public bool HandleMessage(MessageBase message)
        {
            // TODO
            return true;
        }

        public void AddToQueue(MessageBase message)
        {
            // TODO
        }

        public void Start()
        {
            // TODO
        }

        public void Stop()
        {
            // TODO
        }

        public void Dispose()
        {
            // TODO
        }
    }
}