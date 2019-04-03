using System;
using System.Collections.Generic;
using System.Threading;
using Testflow.Common;
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
                if (!_stateActions.ContainsKey(value))
                {
                    throw new TestflowInternalException(CommonErrorCode.InternalError, $"Unsupported state {value}");
                }
                _stateActions[value].Invoke();
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
                }
            };
        }

    }
}