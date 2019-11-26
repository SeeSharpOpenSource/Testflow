using System;
using System.Threading;

namespace Testflow.SlaveCore.Debugger
{
    internal class CoroutineRuntimeHandle : IDisposable
    {
        public CoroutineState State { get; set; }
        public int CoroutineId { get; }
        private readonly AutoResetEvent _blockEvent;

        public CoroutineRuntimeHandle(int coroutineId)
        {
            this.State = CoroutineState.Idle;
            this.CoroutineId = coroutineId;
            this._blockEvent = new AutoResetEvent(false);
        }

        public void WaitSignal()
        {
            this.State = CoroutineState.Blocked;
            _blockEvent.WaitOne();
            this.State = CoroutineState.Running;
        }

        public void SetSignal()
        {
            _blockEvent.Set();
        }

        public void Dispose()
        {
            _blockEvent.Dispose();
        }
    }
}