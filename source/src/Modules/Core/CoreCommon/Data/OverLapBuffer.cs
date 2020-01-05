using System;
using System.Threading;

namespace Testflow.CoreCommon.Data
{
    public class OverLapBuffer<TDataType> : IDisposable where TDataType : class
    {
        private TDataType[] _innerBuffer;
        private readonly int _capacity;
        private int _nextIndex;
        private int _startIndex;
        private ReaderWriterLockSlim _operationLock;
        // buffer满了以后一次删除的个数
        private readonly int _removeCount;

        public OverLapBuffer(int capacity)
        {
            _innerBuffer = new TDataType[capacity];
            _capacity = capacity;
            _removeCount = capacity/5;
            _nextIndex = 0;
            _startIndex = 1;
            _operationLock = new ReaderWriterLockSlim();
        }

        public void Enqueue(TDataType data)
        {
            _operationLock.EnterWriteLock();

            if (_nextIndex == _startIndex)
            {
                _startIndex = (_removeCount + _startIndex) % _capacity;
            }
            _innerBuffer[_nextIndex] = data;
            _nextIndex = (_nextIndex + 1)%_capacity;

            _operationLock.ExitWriteLock();
        }

        public TDataType GetLastElement(int offset)
        {
            TDataType dataValue = null;
            _operationLock.EnterReadLock();

            int startIndex = Thread.VolatileRead(ref _startIndex);
            int readIndex = (_nextIndex - offset)%_capacity;
            if (readIndex <= startIndex)
            {
                dataValue = _innerBuffer[readIndex];
            }
            _operationLock.ExitReadLock();

            return dataValue;
        }

        public void Dispose()
        {
            _innerBuffer = null;
            _operationLock?.Dispose();
            _operationLock = null;
        }
    }
}