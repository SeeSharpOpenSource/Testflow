using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Usr;
using Testflow.Runtime;

namespace Testflow.MasterCore.Core
{
    internal class RuntimeStateMachine
    {
        public delegate void StateChangedDelegate();

        public event StateChangedDelegate StateIdle;
        public event StateChangedDelegate StateRunning;
        public event StateChangedDelegate StateError;
        public event StateChangedDelegate StateOver;
        public event StateChangedDelegate StateAbortRequested;
        public event StateChangedDelegate StateAbort;
        public event StateChangedDelegate StateCollapsed;
        public event StateChangedDelegate TimeOut;

        private readonly Dictionary<RuntimeState, Action> _stateActions;
        private int _runtimeState;

        /// <summary>
        /// 全局状态。配置规则：哪里最早获知全局状态变更就在哪里更新。
        /// </summary>
        public RuntimeState State
        {
            get { return (RuntimeState) _runtimeState; }
            set
            {
                // 如果当前状态大于等于待更新状态则不执行。因为在一次运行的实例中，状态的迁移是单向的。
                if ((int)value <= _runtimeState)
                {
                    return;
                }
                Thread.VolatileWrite(ref _runtimeState, (int)value);
                this.EventRunning = true;
                Thread.MemoryBarrier();
                try
                {
                    if (_stateActions.ContainsKey(value))
                    {
                        _stateActions[value].Invoke();
                    }
                    else
                    {
//                    throw new TestflowInternalException(CommonErrorCode.InternalError, $"Unsupported state {value}");
                    }
                    Thread.MemoryBarrier();
                }
                finally
                {
                    this.EventRunning = false;
                }
            }
        }


        private int _eventRunFlag = 0;
        /// <summary>
        /// 事件是否正在运行的标识。true时状态切换后的事件还未执行结束；false时状态切换后的事件已执行结束
        /// </summary>
        public bool EventRunning
        {
            get { return _eventRunFlag == 1; }
            private set
            {
                Thread.VolatileWrite(ref _eventRunFlag, value ? 1 : 0);
            }
        }

        public RuntimeStateMachine()
        {
            _runtimeState = (int) RuntimeState.NotAvailable;
            _stateActions = new Dictionary<RuntimeState, Action>(10)
            {
                {
                    RuntimeState.Idle, () => { StateIdle?.Invoke(); }
                },
                {
                    RuntimeState.Running, () => { StateRunning?.Invoke(); }
                },
                {
                    RuntimeState.Error, () => { StateError?.Invoke(); }
                },
                {
                    RuntimeState.Over, () => { StateOver?.Invoke(); }
                },
                {
                    RuntimeState.AbortRequested, () => { StateAbortRequested?.Invoke(); }
                },
                {
                    RuntimeState.Abort, () => { StateAbort?.Invoke(); }
                },
                {
                    RuntimeState.Collapsed, () => { StateCollapsed?.Invoke(); }
                },
                {
                    RuntimeState.Timeout, () => { TimeOut?.Invoke(); }
                }
            };
        }

    }
}