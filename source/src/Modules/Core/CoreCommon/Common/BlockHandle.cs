using System;
using System.Threading;

namespace Testflow.CoreCommon.Common
{
    public class BlockHandle : IDisposable
    {
        private SemaphoreSlim _waitEvent;
        private int _waitState;

        public BlockHandle()
        {
            _waitEvent = new SemaphoreSlim(0, 1);
            this._waitState = int.MaxValue;
            this.Timeout = System.Threading.Timeout.Infinite;
        }

        public int Timeout { get; set; }

        public bool Wait(int waitState)
        {
            Thread.VolatileWrite(ref _waitState, waitState);
            return _waitEvent.Wait(Timeout);
        }

        public void Free(int waitState)
        {
            if (waitState == _waitState)
            {
                Interlocked.Exchange(ref _waitState, int.MaxValue);
                _waitEvent.Release();
            }
        }

        private int _diposedFlag = 0;
        public void Dispose()
        {
            if (_diposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _diposedFlag, 1);
            Thread.MemoryBarrier();
            _waitEvent?.Dispose();
        }
    }
}