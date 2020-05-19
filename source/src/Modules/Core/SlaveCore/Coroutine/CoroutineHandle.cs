using System;
using System.Diagnostics;
using System.Threading;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;
using Testflow.SlaveCore.Debugger;
using Testflow.SlaveCore.Runner.Model;

namespace Testflow.SlaveCore.Coroutine
{
    internal class CoroutineHandle : IDisposable
    {
        private int _stateValue;

        public CoroutineState State
        {
            get { return (CoroutineState) _stateValue; }
            set
            {
                int newStateValue = (int)value;
                // 除了新旧状态都是运行态以外，协程的状态只能从前向后
                if (newStateValue <= _stateValue && !IsRunState(_stateValue) && !IsRunState(newStateValue))
                {
                    return;
                }
                Thread.VolatileWrite(ref _stateValue, newStateValue);
            }
        }

        public int Id { get; }
        private readonly AutoResetEvent _blockEvent;

        public ExecutionTrack ExecutionTracker { get; }

        public event Action<StepTaskEntityBase> PreListener;
        public event Action<StepTaskEntityBase> PostListener;

        /// <summary>
        /// 协程开始执行时间
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// 协程执行结束时间
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// 协程执行时间
        /// </summary>
        public long ElapsedTicks { get; private set; }

        private readonly Stopwatch _stopWatch;

        public CoroutineHandle(int id)
        {
            this.State = CoroutineState.Idle;
            this.Id = id;
            this._blockEvent = new AutoResetEvent(false);
//            this.ExecutionTracker = new ExecutionTrack(Constants.ExecutionTrackerSize);
            this.StartTime = DateTime.MinValue;
            this.EndTime = DateTime.MinValue;
            this._stopWatch = new Stopwatch();
            this.ElapsedTicks = -1;
        }

        public void Start()
        {
            this.State = CoroutineState.Running;
            this.StartTime = DateTime.Now;
            this.ElapsedTicks = -1;
            this._stopWatch.Reset();
            this._stopWatch.Start();
        }

        public void Pause()
        {
            this.State = CoroutineState.Blocked;
            this._stopWatch.Stop();
        }

        public void Continue()
        {
            this.State = CoroutineState.Running;
            this._stopWatch.Start();
        }

        public void Stop()
        {
            this._stopWatch.Stop();
            this.ElapsedTicks = this._stopWatch.ElapsedTicks;
            this.EndTime = DateTime.Now;
            this.State = CoroutineState.Over;
        }

        public void WaitSignal()
        {
            Pause();
            _blockEvent.WaitOne();
            Continue();
        }

        public void SetSignal()
        {
            _blockEvent.Set();
        }

        public void OnPreListener(StepTaskEntityBase stepEntity)
        {
            PreListener?.Invoke(stepEntity);
        }

        public void OnPostListener(StepTaskEntityBase stepEntity)
        {
            PostListener?.Invoke(stepEntity);
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
            if (IsRunState(_stateValue))
            {
                Stop();
            }
            _blockEvent?.Dispose();
            ExecutionTracker?.Dispose();
        }

        private static bool IsRunState(int stateValue)
        {
            return stateValue == (int) CoroutineState.Running || stateValue == (int) CoroutineState.Blocked;
        }
    }
}