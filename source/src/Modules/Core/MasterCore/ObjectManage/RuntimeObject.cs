using System.Threading;

namespace Testflow.MasterCore.ObjectManage
{
    public abstract class RuntimeObject
    {
        private static long _nextId;

        static RuntimeObject()
        {
            _nextId = -1;
        }

        public long Id { get; }

        protected RuntimeObject()
        {
            this.Id = Interlocked.Increment(ref _nextId);
        }
    }
}