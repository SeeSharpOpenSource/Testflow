using System.Runtime.Serialization;
using Testflow.EngineCore.Data;

namespace Testflow.EngineCore.Message.Messages
{
    /// <summary>
    /// 资源同步消息请求
    /// </summary>
    public class ResourceSyncMessage : MessageBase
    {
        public SyncResourceInfo SyncInfo { get; set; }

        public bool Acquired { get; set; }


        public ResourceSyncMessage(string name, int id, SyncResourceInfo syncInfo, bool acquired) : base(name, id, MessageType.Sync)
        {
            this.SyncInfo = syncInfo;
            this.Acquired = acquired;
        }

        public ResourceSyncMessage(string name, int id, SyncResourceInfo syncInfo) : base(name, id, MessageType.Sync)
        {
            this.SyncInfo = syncInfo;
            this.Acquired = false;
        }

        public ResourceSyncMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.SyncInfo = info.GetValue("SyncInfo", typeof (SyncResourceInfo)) as SyncResourceInfo;
            this.Acquired = (bool) info.GetValue("Acquired", typeof (SyncResourceInfo));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("SyncInfo", SyncInfo);
            info.AddValue("Acquired", Acquired);
        }
    }
}