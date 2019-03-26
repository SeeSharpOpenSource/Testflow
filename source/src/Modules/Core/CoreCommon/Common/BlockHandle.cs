using System.Threading;

namespace Testflow.CoreCommon.Common
{
    public class BlockHandle
    {
        private SemaphoreSlim _waitEvent;
        private int _waitState;

        public BlockHandle()
        {
            _waitEvent = new SemaphoreSlim(1);
            this._waitState = int.MaxValue;
            this.Timeout = System.Threading.Timeout.Infinite;
        }

        public int Timeout { get; set; }

        public void Wait(int waitState)
        {
            Thread.VolatileWrite(ref _waitState, waitState);
            _waitEvent.Wait(Timeout);
        }

        public void Free(int waitState)
        {
            if (waitState == _waitState)
            {
                _waitEvent.Release();
            }
        }
    }
}