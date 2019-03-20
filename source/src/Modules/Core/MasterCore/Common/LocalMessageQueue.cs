using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.CoreCommon.Messages;

namespace Testflow.MasterCore.Common
{
    internal class LocalMessageQueue<TMessageType> : Queue<TMessageType> where TMessageType : MessageBase
    {
        private SpinLock _operationLock;

        private SpinLock _blockLock;

        private readonly AutoResetEvent _blockEvent;

        private int _isblocked;
        private int _forceFree;

        public LocalMessageQueue(int capacity) : base(capacity)
        {
            _operationLock = new SpinLock();
            _blockLock = new SpinLock();
            _blockEvent = new AutoResetEvent(false);
            _isblocked = 0;
            _forceFree = 0;
        }
        
        public TMessageType WaitUntilMessageCome()
        {
            TMessageType message = null;
            bool getLock = false;
            _operationLock.Enter(ref getLock);

            if (base.Count > 0)
            {
                _operationLock.Exit();
                return base.Dequeue();
            }
            _operationLock.Exit();
            BlockThread();
            _operationLock.Enter(ref getLock);
            // 如果为null，则意味着该阻塞是被停止操作触发的
            if (0 != base.Count)
            {
                message = base.Dequeue();
            }
            _operationLock.Exit();
            return message;
        }

        public new void Enqueue(TMessageType item)
        {
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            base.Enqueue(item);
            // 如果被阻塞，则释放等待线程
            FreeThread();
            Interlocked.Exchange(ref _isblocked, 0);
            _operationLock.Exit();
        }

        public new TMessageType Dequeue()
        {
            TMessageType message = null;
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            message = base.Dequeue();
            _operationLock.Exit();
            return message;
        }

        public new void Clear()
        {
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            base.Clear();
            _operationLock.Exit();
        }

        /// <summary>
        /// 强制释放lock，让线程继续执行
        /// </summary>
        public void FreeLock()
        {
            bool getLock = false;
            _blockLock.Enter(ref getLock);
            if (_isblocked == 1)
            {
                _blockEvent.Set();
                Interlocked.Exchange(ref _isblocked, 0);
            }
            Thread.VolatileWrite(ref _forceFree, 1);
            _blockLock.Exit();
        }

        private void BlockThread()
        {
            bool getLock = false;
            _blockLock.Enter(ref getLock);
            // 如果未被block，并且消息数大于0，并且没有申请强制释放锁则阻塞线程
            if (0 == _isblocked && 0 == Count && 0 == _forceFree)
            {
                _blockEvent.WaitOne(Timeout.Infinite);
                Thread.VolatileWrite(ref _isblocked, 1);
            }
            _blockLock.Exit();
        }

        private void FreeThread()
        {
            bool getLock = false;
            _blockLock.Enter(ref getLock);
            if (0 != _isblocked && 0 <= Count)
            {
                _blockEvent.Set();
                Thread.VolatileWrite(ref _isblocked, 0);
            }
            _blockLock.Exit();
        }
    }
}