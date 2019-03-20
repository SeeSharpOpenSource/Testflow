using System;
using Testflow.CoreCommon.Messages;
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

        public bool HandleMessage(MessageBase message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(MessageBase message)
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