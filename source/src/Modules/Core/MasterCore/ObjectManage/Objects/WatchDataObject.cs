using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.ObjectManage.Objects
{
    internal class WatchDataObject : RuntimeObject
    {
        public int Session { get; }

        public int Sequence { get; }

        public string WatchData { get; }

        public WatchDataObject(int session, int sequence, string watchData) : base(Constants.WatchDataObjectName)
        {
            this.Session = session;
            this.Sequence = sequence;
            this.WatchData = watchData;
        }
    }
}