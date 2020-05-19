using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Testflow.SlaveCore.Common
{
    internal class StopWatchManager : IDisposable
    {
        private readonly SlaveContext _context;
        private readonly Dictionary<int, Stopwatch> _stopWatches;
        private long _frequency;

        public StopWatchManager(SlaveContext context)
        {
            this._context = context;
            this._stopWatches = new Dictionary<int, Stopwatch>(10);
            this._frequency = Stopwatch.Frequency;
        }

        public void RegisterStopWatch(int coroutineId)
        {
            //因为是串行操作，无需做竞争处理
            if (!_stopWatches.ContainsKey(coroutineId))
            {
                _stopWatches.Add(coroutineId, new Stopwatch());
            }
        }

        public void StartTiming(int coroutineId)
        {
            //因为是线程间Diction的只读操作或者同一个StopWatch的线程内操作，无需做竞争处理
            Stopwatch stopWatch = _stopWatches[coroutineId];
            stopWatch.Reset();
            stopWatch.Start();
        }

        public long EndTiming(int coroutineId)
        {
            //因为是线程间Diction的只读操作或者同一个StopWatch的线程内操作，无需做竞争处理
            Stopwatch stopWatch = _stopWatches[coroutineId];
            stopWatch.Stop();
            // 计算精确到微秒级别的数据
            return (long)((double) stopWatch.ElapsedTicks/_frequency*1E6);
        }

        private int _diposedFlag = 0;
        public void Dispose()
        {
            if (_diposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _diposedFlag, 1);
            Thread.MemoryBarrier();
            foreach (Stopwatch stopwatch in _stopWatches.Values)
            {
                stopwatch.Reset();
            }
            _stopWatches.Clear();
        }
    }
}