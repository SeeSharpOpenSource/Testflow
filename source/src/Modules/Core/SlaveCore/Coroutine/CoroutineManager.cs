﻿using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.SlaveCore.Common;
using Testflow.Usr;

namespace Testflow.SlaveCore.Coroutine
{
    internal class CoroutineManager : IDisposable
    {
        // 协程ID到对应协程运行时句柄的映射
        private Dictionary<int, CoroutineHandle> _coroutineHandles;

        private readonly SlaveContext _context;
        private int _currentIndex;
        public CoroutineManager(SlaveContext context)
        {
            this._context = context;
            _coroutineHandles = new Dictionary<int, CoroutineHandle>(Constants.DefaultRuntimeSize);
            _currentIndex = -1*CommonConst.SequenceCoroutineCapacity;
        }

        public CoroutineHandle GetNextCoroutine()
        {
            int coroutineId = Interlocked.Add(ref _currentIndex, CommonConst.SequenceCoroutineCapacity);
            CoroutineHandle coroutineHandle = new CoroutineHandle(coroutineId);
            _coroutineHandles.Add(coroutineId, coroutineHandle);
            return coroutineHandle;
        }

        public CoroutineHandle GetCoroutineHandle(int coroutineId)
        {
            return _coroutineHandles[coroutineId];
        }

        public void Dispose()
        {
            foreach (CoroutineHandle resetEvent in _coroutineHandles.Values)
            {
                resetEvent.Dispose();
            }
            _coroutineHandles.Clear();
        }
    }
}