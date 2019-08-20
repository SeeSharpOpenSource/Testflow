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
                _waitEvent.Release();
            }
        }

        public void Dispose()
        {
            _waitEvent?.Dispose();
        }
    }
}