using System;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.Message;
using Testflow.Utility.MessageUtil;

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

        public void HandleMessage(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}