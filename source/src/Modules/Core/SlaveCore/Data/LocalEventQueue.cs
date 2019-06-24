using System.Collections.Generic;
using System.Threading;
using Testflow.CoreCommon.Data.EventInfos;

namespace Testflow.SlaveCore.Data
{
    internal class LocalEventQueue<TMessageType> : Queue<TMessageType> where TMessageType : class
    {
        private SpinLock _operationLock;

        private SpinLock _blockLock;

        private int _stopEnqueueFlag;

        private readonly AutoResetEvent _blockEvent;

        private int _blockCount;
        private int _forceFree;

        public new int Count
        {
            get
            {
                bool getLock = false;
                _operationLock.Enter(ref getLock);
                int count = base.Count;
                _operationLock.Exit();
                return count;
            }
        }

        public LocalEventQueue(int capacity) : base(capacity)
        {
            _operationLock = new SpinLock();
            _blockLock = new SpinLock();
            _blockEvent = new AutoResetEvent(false);
            _blockCount = 0;
            _forceFree = 0;
            _stopEnqueueFlag = 0;
        }
        
        public TMessageType WaitUntilMessageCome()
        {
            TMessageType message = null;
            while (true)
            {
                bool getLock = false;
                _operationLock.Enter(ref getLock);
                if (base.Count > 0)
                {
                    message = base.Dequeue();
                    _operationLock.Exit();
                    return message;
                }
                Thread.VolatileWrite(ref _blockCount, ++_blockCount);
                _operationLock.Exit();
                BlockThread();
            }
        }

        public new void Enqueue(TMessageType item)
        {
            if (1 == _stopEnqueueFlag)
            {
                return;
            }
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            base.Enqueue(item);
            // 如果被阻塞，则释放等待线程
            _operationLock.Exit();
            FreeThread();
        }

        public new void Clear()
        {
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            base.Clear();
            _operationLock.Exit();
        }

        public void StopEnqueue()
        {
            bool getLock = false;
            _blockLock.Enter(ref getLock);
            Thread.VolatileWrite(ref _stopEnqueueFlag, 1);
            _blockLock.Exit();
        }

        /// <summary>
        /// 强制释放lock，让线程继续执行
        /// </summary>
        public void FreeLock()
        {
            bool getLock = false;
            _blockLock.Enter(ref getLock);
            if (_blockCount > 0)
            {
                _blockEvent.Set();
                Interlocked.Exchange(ref _blockCount, 0);
            }
            Thread.VolatileWrite(ref _forceFree, 1);
            _blockLock.Exit();
        }

        private void BlockThread()
        {
            _blockEvent.WaitOne(Timeout.Infinite);
        }

        private void FreeThread()
        {
            bool getLock = false;
            _blockLock.Enter(ref getLock);
            if (0 < _blockCount && 0 < Count)
            {
                Thread.VolatileWrite(ref _blockCount, --_blockCount);
                _blockEvent.Set();
            }
            _blockLock.Exit();
        }
    }
}