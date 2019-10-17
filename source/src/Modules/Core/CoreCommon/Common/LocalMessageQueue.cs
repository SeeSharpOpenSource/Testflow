using System.Collections.Generic;
using System.Threading;
using Testflow.CoreCommon.Messages;

namespace Testflow.CoreCommon.Common
{
    public class LocalMessageQueue<TMessageType> : Queue<TMessageType> where TMessageType : MessageBase
    {
        private SpinLock _operationLock;

        private int _stopEnqueueFlag;

        private readonly AutoResetEvent _blockEvent;

        private int _blockCount;

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

        public LocalMessageQueue(int capacity) : base(capacity)
        {
            _operationLock = new SpinLock();
            _blockEvent = new AutoResetEvent(false);
            _blockCount = 0;
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
                // 如果队列中没有数据，并且已经停止入列，则返回null
                if (_stopEnqueueFlag == 1)
                {
                    _operationLock.Exit();
                    return null;
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
            try
            {
                base.Enqueue(item);
                // 如果被阻塞，则释放等待线程
                FreeBlockThread();
            }
            finally
            {
                _operationLock.Exit();
            }
        }

        public new void Clear()
        {
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            base.Clear();
            _operationLock.Exit();
        }

        /// <summary>
        /// 停止入列，强制释放lock，让线程继续执行
        /// </summary>
        public void FreeBlocks()
        {
            bool getLock = false;
            _operationLock.Enter(ref getLock);
            Thread.VolatileWrite(ref _stopEnqueueFlag, 1);
            try
            {
                while (_blockCount > 0)
                {
                    _blockEvent.Set();
                    Thread.VolatileWrite(ref _blockCount, --_blockCount);
                }
            }
            finally
            {
                _operationLock.Exit();
            }
        }

        private void BlockThread()
        {
            _blockEvent.WaitOne(Timeout.Infinite);
        }

        private void FreeBlockThread()
        {
            if (0 < _blockCount && 0 < base.Count)
            {
                Thread.VolatileWrite(ref _blockCount, --_blockCount);
                _blockEvent.Set();
            }
        }
    }
}