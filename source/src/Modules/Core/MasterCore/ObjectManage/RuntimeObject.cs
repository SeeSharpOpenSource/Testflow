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

        public string ObjectName { get; }

        public long Id { get; }

        protected RuntimeObject(string objectName)
        {
            this.Id = Interlocked.Increment(ref _nextId);
            this.ObjectName = objectName;
        }
    }
}