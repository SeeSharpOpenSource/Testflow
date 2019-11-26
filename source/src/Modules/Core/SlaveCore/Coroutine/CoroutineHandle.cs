using System;
using System.Threading;
using Testflow.SlaveCore.Debugger;

namespace Testflow.SlaveCore.Coroutine
{
    internal class CoroutineHandle : IDisposable
    {
        public CoroutineState State { get; set; }
        public int Id { get; }
        private readonly AutoResetEvent _blockEvent;

        public event Action PreListener;
        public event Action PostListener;

        public CoroutineHandle(int id)
        {
            this.State = CoroutineState.Idle;
            this.Id = id;
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

        public void OnPreListener()
        {
            PreListener?.Invoke();
        }

        public void OnPostListener()
        {
            PostListener?.Invoke();
        }

        public void Dispose()
        {
            _blockEvent.Dispose();
        }
    }
}