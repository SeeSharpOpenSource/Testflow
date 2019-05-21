using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Testflow.Usr;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.EventData;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.MasterCore.StatusManage
{
    internal class SequenceStateHandle
    {
        private int _state;
        private readonly EventDispatcher _eventDispatcher;
        private readonly StateManageContext _stateManageContext;
        private readonly ISequence _sequence;
        private DateTime _blockedStart;
        private readonly ISequenceTestResult _sequenceTestResult;
        private readonly SequenceResultData _sequenceResultData;
        private RuntimeStatusData _statusData;

        public SequenceStateHandle(int session, ISequence sequence, StateManageContext stateManageContext)
        {
            this.Session = session;
            this._sequence = sequence;
            this.SequenceIndex = sequence.Index;
            this.State = RuntimeState.Idle;
            this._eventDispatcher = stateManageContext.EventDispatcher;
            this._stateManageContext = stateManageContext;
            this.BlockedTime = TimeSpan.Zero;
            this._blockedStart = DateTime.MaxValue;

            _sequenceTestResult = this._stateManageContext.GetSequenceResults(Session, SequenceIndex);
            _sequenceResultData = new SequenceResultData()
            {
                Name = sequence.Name,
                Description = sequence.Description,
                StartTime = DateTime.MaxValue,
                EndTime = DateTime.MaxValue,
                ElapsedTime = 0,
                RuntimeHash = stateManageContext.RuntimeHash,
                FailInfo = string.Empty,
                Result = RuntimeState.Idle,
                FailStack = string.Empty,
                Session = session,
                SequenceIndex = sequence.Index,
            };
            _stateManageContext.DatabaseProxy.WriteData(_sequenceResultData);
            _statusData = new RuntimeStatusData()
            {
                RuntimeHash = _stateManageContext.RuntimeHash,
                Sequence = this.SequenceIndex,
                Session = Session,
                Stack = this.RunStack.ToString(),
                Time = CurrentTime,
                ElapsedTime = this.ElapsedTime.TotalMilliseconds,
                Result = StepResult.NotAvailable,
                WatchData = string.Empty,
                StatusIndex = -1
            };
        }

        public RuntimeState State
        {
            get { return (RuntimeState)_state; }
            set
            {
                if (_state == (int)value)
                {
                    return;
                }
                Thread.VolatileWrite(ref _state, (int)value);
            }
        }

        public int Session { get; }

        public int SequenceIndex { get; }
        
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CurrentTime { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public TimeSpan BlockedTime { get; set; }

        public CallStack RunStack { get; set; }

        public StepResult StepResult { get; set; }

        // 处理事件和消息。完成的工作有：
        // 更新SequenceStateHandle的状态、生成RuntimeStatusData并持久化、序列执行结束后生成SequenceResultData并持久化
        #region 事件消息处理

        public void AbortEventProcess(AbortEventInfo eventInfo)
        {
            if (eventInfo.IsRequest)
            {
                RefreshCommonStatus(eventInfo, RuntimeState.AbortRequested, StepResult.NotAvailable);
            }
            else
            {
                RefreshCommonStatus(eventInfo, RuntimeState.Abort, StepResult.Abort);

                SequenceFailedInfo failedInfo = new SequenceFailedInfo(_stateManageContext.GlobalInfo.I18N.GetStr("UserAbort"), FailedType.Abort);
                UpdateSequenceTestResult(failedInfo, null);
                _eventDispatcher.RaiseEvent(CoreConstants.SequenceOver, eventInfo.Session, _sequenceTestResult);

                WriteRuntimeStatusData(StepResult.Failed, string.Empty);
            }
        }

        public void DebugEventProcess(DebugEventInfo eventInfo, ISequenceFlowContainer parentSequenceData)
        {
            if (eventInfo.IsDebugHit)
            {
                RefreshCommonStatus(eventInfo, RuntimeState.DebugBlocked, StepResult.NotAvailable);
                
                DebugInformation debugInformation = new DebugInformation(eventInfo, parentSequenceData);
                _stateManageContext.EventDispatcher.RaiseEvent(CoreConstants.BreakPointHitted, Session,
                    _stateManageContext.GlobalInfo.DebugHandle, debugInformation);
            }
            else
            {
                RefreshCommonStatus(eventInfo, RuntimeState.Running, StepResult.NotAvailable);
            }
        }

        public void ExceptionEventProcess(ExceptionEventInfo eventInfo)
        {
            // TODO
        }

        public void SyncEventProcess(SyncEventInfo eventInfo)
        {
            // TODO
        }

        public void HandleStatusMessage(StatusMessage message, int index)
        {
            RuntimeState newState = message.SequenceStates[index];
            this.RunStack = message.Stacks[index];
            StepResult stepResult = message.Results[index];
            switch (message.Name)
            {
                case MessageNames.StartStatusName:
                case MessageNames.ReportStatusName:
                    if (State == RuntimeState.StartIdle && newState == RuntimeState.Running)
                    {
                        // 序列刚开始执行
                        RefreshCommonStatus(message, newState, stepResult);
                        // 更新数据库中的测试数据条目
                        UpdateSequenceResultData(string.Empty);
                        // 触发SequenceStart事件
                        UpdateSequenceTestResult(null, null);
                        _stateManageContext.EventDispatcher.RaiseEvent(Constants.SequenceOver, Session,
                            _sequenceTestResult);

                    }
                    // 序列执行结束
                    else if ((State == RuntimeState.Running || State == RuntimeState.Blocked || State == RuntimeState.DebugBlocked)
                        && newState > RuntimeState.AbortRequested)
                    {
                        RefreshCommonStatus(message, newState, stepResult);

                        // 更新数据库中的测试数据条目
                        UpdateSequenceResultData(GetFailedInfoStr(message));
                        // 写入RuntimeStatusInfo条目
                        string watchDataStr = ModuleUtils.WatchDataToString(message.WatchData);
                        WriteRuntimeStatusData(stepResult, watchDataStr);

                        // 触发SequenceOver事件
                        ISequenceFailedInfo failedInfo = GetFailedInfo(message);
                        UpdateSequenceTestResult(failedInfo, message.WatchData);
                        _stateManageContext.EventDispatcher.RaiseEvent(Constants.SequenceOver, Session,
                            _sequenceTestResult);
                    }
                    // 关键节点状态更新
                    else
                    {
                        RefreshCommonStatus(message, newState, stepResult);
                        // 只有在StepResult处于结果节点时才会触发事件和
                        if (message.InterestedSequence.Contains(SequenceIndex))
                        {
                            // 写入RuntimeStatusInfo条目
                            string watchDataStr = ModuleUtils.WatchDataToString(message.WatchData);
                            WriteRuntimeStatusData(stepResult, watchDataStr);
                        }
                    }
                    break;
                case MessageNames.HearBeatStatusName:
                    RefreshCommonStatus(message, newState, stepResult);
                    break;
                case MessageNames.ErrorStatusName:
                    newState = RuntimeState.Error;
                    stepResult = StepResult.Error;
                    RefreshCommonStatus(message, newState, stepResult);

                    // 更新数据库中的测试数据条目
                    UpdateSequenceResultData(message.ExceptionInfo.ToString());
                    // 写入RuntimeStatusInfo条目
                    WriteRuntimeStatusData(stepResult, ModuleUtils.WatchDataToString(message.WatchData));

                    // 触发SequenceOver事件
                    UpdateSequenceTestResult(message.ExceptionInfo, message.WatchData);
                    _stateManageContext.EventDispatcher.RaiseEvent(Constants.SequenceOver, Session,
                        _sequenceTestResult);
                    break;
                case MessageNames.ResultStatusName:
                    break;
                default:
                    throw new InvalidOperationException();
                    break;
            }
        }

        #endregion

        #region 更新数据

        /// <summary>
        /// 更新事件、状态和StepResult
        /// </summary>
        private void RefreshCommonStatus(MessageBase message, RuntimeState newState, StepResult result)
        {
            // 如果阻塞开始时间无效并且最新状态是非阻塞状态，则继续执行
            if (_blockedStart == DateTime.MaxValue && (newState != RuntimeState.Blocked && newState != RuntimeState.DebugBlocked))
            {
                // ignore
            }
            // 如果阻塞开始时间有效，并且目前的状态为非阻塞状态，则更新阻塞时间并清空阻塞开始时间
            else if (_blockedStart != DateTime.MaxValue && (newState != RuntimeState.Blocked && newState != RuntimeState.DebugBlocked))
            {
                this.BlockedTime += message.Time - _blockedStart;

                this._blockedStart = DateTime.MaxValue;
            }
            // 如果阻塞开始事件无效，并且目前状态为阻塞状态，则更新阻塞开始时间
            else if (_blockedStart == DateTime.MaxValue && (newState == RuntimeState.Blocked || newState == RuntimeState.DebugBlocked))
            {
                this._blockedStart = message.Time;
            }
            this.ElapsedTime = message.Time - this.StartTime - this.BlockedTime;
            this.CurrentTime = message.Time;
            if (newState == RuntimeState.Abort || newState == RuntimeState.Error || newState == RuntimeState.Failed || newState == RuntimeState.Success)
            {
                this.EndTime = message.Time;
            }
            this.State = newState;
            this.StepResult = result;
        }

        private void RefreshCommonStatus(EventInfoBase eventInfo, RuntimeState newState, StepResult stepResult)
        {
            // 如果阻塞开始时间无效并且最新状态是非阻塞状态，则继续执行
            if (_blockedStart == DateTime.MaxValue && (newState != RuntimeState.Blocked && newState != RuntimeState.DebugBlocked))
            {
                // ignore
            }
            // 如果阻塞开始时间有效，并且目前的状态为非阻塞状态，则更新阻塞时间并清空阻塞开始时间
            else if (_blockedStart != DateTime.MaxValue && (newState != RuntimeState.Blocked && newState != RuntimeState.DebugBlocked))
            {
                this.BlockedTime += eventInfo.TimeStamp - _blockedStart;

                this._blockedStart = DateTime.MaxValue;
            }
            // 如果阻塞开始事件无效，并且目前状态为阻塞状态，则更新阻塞开始时间
            else if (_blockedStart == DateTime.MaxValue && (newState == RuntimeState.Blocked || newState == RuntimeState.DebugBlocked))
            {
                this._blockedStart = eventInfo.TimeStamp;
            }
            this.ElapsedTime = eventInfo.TimeStamp - this.StartTime - this.BlockedTime;
            this.CurrentTime = eventInfo.TimeStamp;
            this.State = newState;
            if (newState == RuntimeState.Abort || newState == RuntimeState.Error || newState == RuntimeState.Failed || newState == RuntimeState.Success)
            {
                this.EndTime = eventInfo.TimeStamp;
            }
            this.StepResult = stepResult;
        }

        private void UpdateSequenceTestResult(ISequenceFailedInfo failedInfo, Dictionary<string, string> watchData)
        {
            _sequenceTestResult.ResultState = this.State;
            _sequenceTestResult.StartTime = this.StartTime;
            _sequenceTestResult.EndTime = this.EndTime;
            _sequenceTestResult.ElapsedTime = this.ElapsedTime.TotalMilliseconds;
            _sequenceTestResult.FailedInfo = failedInfo;
            _sequenceTestResult.VariableValues.Clear();

            if (null != watchData)
            {
                Regex varNameRegex = new Regex(CoreUtils.GetVariableNameRegex(_sequence, Session));
                foreach (KeyValuePair<string, string> varToValue in watchData)
                {
                    if (varNameRegex.IsMatch(varToValue.Key))
                    {
                        IVariable variable = CoreUtils.GetVariable(_sequence, varToValue.Key);
                        _sequenceTestResult.VariableValues.Add(variable, varToValue.Value);
                    }
                }
            }
        }

        private void WriteRuntimeStatusData(StepResult result, string watchData)
        {
            _statusData.Stack = this.RunStack.ToString();
            _statusData.Time = CurrentTime;
            _statusData.ElapsedTime = this.ElapsedTime.TotalMilliseconds;
            _statusData.Result = result;
            _statusData.WatchData = watchData;
            _statusData.StatusIndex = _stateManageContext.DataStatusIndex;

            _stateManageContext.DatabaseProxy.WriteData(_statusData);
        }

        private void UpdateSequenceResultData(string failedInfo)
        {
            _sequenceResultData.StartTime = StartTime;
            _sequenceResultData.ElapsedTime = this.ElapsedTime.TotalMilliseconds;
            _sequenceResultData.EndTime = EndTime;
            _sequenceResultData.Result = State;
            _sequenceResultData.FailInfo = failedInfo;
            _sequenceResultData.FailStack = RunStack.ToString();
            _stateManageContext.DatabaseProxy.UpdateData(_sequenceResultData);
        }

        #endregion

        public void StopStateHandle(DateTime time, RuntimeState state, string failedInfo)
        {
            this.CurrentTime = time;
            this.EndTime = time;
            if (DateTime.MaxValue != _blockedStart)
            {
                BlockedTime += time - _blockedStart;
                _blockedStart = DateTime.MaxValue;
            }
            this.ElapsedTime = EndTime - StartTime - BlockedTime;
            this.State = state;

            _sequenceTestResult.ElapsedTime = ElapsedTime.TotalMilliseconds;
            _sequenceTestResult.ResultState = State;
            _sequenceTestResult.EndTime = EndTime;
            _sequenceTestResult.FailedInfo.Message = failedInfo;
        }

        private ISequenceFailedInfo GetFailedInfo(StatusMessage message)
        {
            return !message.FailedInfo.ContainsKey(SequenceIndex) ? 
                null : new SequenceFailedInfo(message.FailedInfo[SequenceIndex]);
        }

        private string GetFailedInfoStr(StatusMessage message)
        {
            return !message.FailedInfo.ContainsKey(SequenceIndex) ?
                string.Empty : message.FailedInfo[SequenceIndex];
        }
    }
}