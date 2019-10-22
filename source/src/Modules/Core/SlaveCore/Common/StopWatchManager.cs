using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Testflow.SlaveCore.Common
{
    internal class StopWatchManager : IDisposable
    {
        private readonly SlaveContext _context;
        private readonly Dictionary<int, Stopwatch> _stopWatches;

        public StopWatchManager(SlaveContext context)
        {
            this._context = context;
            this._stopWatches = new Dictionary<int, Stopwatch>(10);
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
            return stopWatch.ElapsedTicks;
        }

        public void Dispose()
        {
            foreach (Stopwatch stopwatch in _stopWatches.Values)
            {
                stopwatch.Reset();
            }
            _stopWatches.Clear();
        }
    }
}