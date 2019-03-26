using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore
{
    public class ExceptionManager
    {
        private readonly Queue<Exception> _exceptions;
        private SpinLock _operationLock;

        internal ExceptionManager()
        {
            this._exceptions = new Queue<Exception>(64);
            this._operationLock = new SpinLock();
            this.EnableEvent = true;
        }

        public int Count => _exceptions.Count;

        public bool EnableEvent { get; set; }

        public void Append(Exception exception)
        {
            if (EnableEvent)
            {
                OnExceptionRaised(exception);
            }
            else
            {
                bool getLock = false;
                _operationLock.Enter(ref getLock);
                _exceptions.Enqueue(exception);
                _operationLock.Exit();
            }
        }

        public Exception Dequeue()
        {
            Exception exception = null;
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            if (_exceptions.Count > 0)
            {
                exception = _exceptions.Dequeue();
            }
            _operationLock.Exit();
            return exception;
        }

        public Exception Peek()
        {
            Exception exception = null;
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            exception = _exceptions.Peek();
            _operationLock.Exit();
            return exception;
        }

        public event Action<Exception> ExceptionRaised;

        private void OnExceptionRaised(Exception exception)
        {
            ExceptionRaised?.Invoke(exception);
        }
    }
}