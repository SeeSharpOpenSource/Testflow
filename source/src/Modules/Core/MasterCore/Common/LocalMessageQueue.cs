using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.CoreCommon.Messages;

namespace Testflow.MasterCore.Common
{
    internal class LocalMessageQueue<TMessageType> : Queue<TMessageType> where TMessageType : MessageBase
    {
        private SpinLock _innerLock;

        private readonly AutoResetEvent _blockEvent;
        private int _isblocked;

        public LocalMessageQueue(int capacity) : base(capacity)
        {
            _innerLock = new SpinLock();
            _blockEvent = new AutoResetEvent(false);
            _isblocked = 0;
        }

        public TMessageType WaitUntilMessageCome()
        {
            TMessageType message = null;
            bool getLock = false;
            _innerLock.Enter(ref getLock);

            if (base.Count > 0)
            {
                _innerLock.Exit();
                return base.Dequeue();
            }
            Interlocked.Exchange(ref _isblocked, 1);
            _blockEvent.WaitOne(Timeout.Infinite);
            message = base.Dequeue();
            _innerLock.Exit();
            return message;
        }

        public new void Enqueue(TMessageType item)
        {
            bool getLock = false;
            _innerLock.Enter(ref getLock);
            base.Enqueue(item);
            // 如果被阻塞，则释放等待线程
            if (_isblocked == 1)
            {
                _blockEvent.Set();
            }
            Interlocked.Exchange(ref _isblocked, 0);
            _innerLock.Exit();
        }

        public new TMessageType Dequeue()
        {
            TMessageType message = null;
            bool getLock = false;
            _innerLock.Enter(ref getLock);
            message = base.Dequeue();
            _innerLock.Exit();
            return message;
        }

        public new void Clear()
        {
            bool getLock = false;
            _innerLock.Enter(ref getLock);
            base.Clear();
            _innerLock.Exit();
        }
    }
}