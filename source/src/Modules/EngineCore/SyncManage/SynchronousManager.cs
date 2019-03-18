using Testflow.EngineCore.Common;
using Testflow.EngineCore.Message;
using Testflow.Utility.MessageUtil;

namespace Testflow.EngineCore.SyncManage
{
    /// <summary>
    /// 资源同步管理模块
    /// </summary>
    internal class SynchronousManager : IMessageHandler
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
    }
}