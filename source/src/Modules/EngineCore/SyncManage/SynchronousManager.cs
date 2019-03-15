using Testflow.EngineCore.Common;
using Testflow.Utility.MessageUtil;
using IMessageConsumer = Testflow.EngineCore.Message.IMessageConsumer;

namespace Testflow.EngineCore.SyncManage
{
    /// <summary>
    /// 资源同步管理模块
    /// </summary>
    internal class SynchronousManager : IMessageConsumer
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