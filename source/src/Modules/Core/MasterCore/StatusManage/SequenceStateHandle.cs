using System;
using System.Linq;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
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
        private ISequenceTestResult _sequenceTestResult;

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

        public string RunStack { get; set; }

        // 处理事件和消息。完成的工作有：
        // 更新SequenceStateHandle的状态、生成RuntimeStatusData并持久化、序列执行结束后生成SequenceResultData并持久化
        #region 事件消息处理

        public void AbortEventProcess(AbortEventInfo eventInfo)
        {
            RuntimeStatusData statusData;
            if (eventInfo.IsRequest)
            {
                RefreshCommonStatus(eventInfo, RuntimeState.AbortRequested);
                statusData = CreateTestStatusData();
            }
            else
            {
                RefreshCommonStatus(eventInfo, RuntimeState.Abort);

                ITestResultCollection testResult =
                    _stateManageContext.GetSessionResults(eventInfo.Session);
                testResult.TestOver = true;
                testResult.AbortCount++;
                testResult[eventInfo.Session].ResultState = RuntimeState.Abort;
                testResult[eventInfo.Session].ElapsedTime = (ulong) this.ElapsedTime.TotalMilliseconds;
                _eventDispatcher.RaiseEvent(CoreConstants.TestOver, eventInfo.Session, testResult);

                statusData = CreateTestStatusData();
            }
            return statusData;
        }

        public void DebugEventProcess(DebugEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

        public void ExceptionEventProcess(ExceptionEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

        public void SyncEventProcess(SyncEventInfo eventInfo)
        {
            throw new NotImplementedException();
        }

        public void HandleTestGenMessage(TestGenMessage message)
        {
            switch (message.State)
            {
                case RuntimeState.Idle:
                    break;
                case RuntimeState.TestGen:

                    break;
                case RuntimeState.Error:
                    break;
                case RuntimeState.AbortRequested:
                    break;
                case RuntimeState.Abort:
                    break;
                default:
                    throw new InvalidProgramException();
            }
            return true;
        }

        public void HandleStatusMessage(StatusMessage message, int index)
        {
            switch (message.State)
            {
                case RuntimeState.Running:
                    break;
                case RuntimeState.Blocked:
                    break;
                case RuntimeState.DebugBlocked:
                    break;
                case RuntimeState.Skipped:
                    break;
                case RuntimeState.Success:
                    break;
                case RuntimeState.Failed:
                    break;
                case RuntimeState.Error:
                    break;
                case RuntimeState.Over:
                    break;
                case RuntimeState.AbortRequested:
                    break;
                case RuntimeState.Abort:
                    break;
                default:
                    throw new InvalidProgramException();
            }
            return true;
        }

        #endregion


        private void RefreshCommonStatus(MessageBase message, RuntimeState newState)
        {
            // 如果阻塞开始时间无效并且最新状态是非阻塞状态，则继续执行
            if (_blockedStart == DateTime.MaxValue &&
                (newState != RuntimeState.Blocked && newState != RuntimeState.DebugBlocked))
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
            else if (_blockedStart == DateTime.MaxValue &&
                     (newState == RuntimeState.Blocked || newState == RuntimeState.DebugBlocked))
            {
                this._blockedStart = message.Time;
            }
            this.ElapsedTime = message.Time - this.StartTime - this.BlockedTime;
            this.CurrentTime = message.Time;
            if (newState == RuntimeState.Abort || newState == RuntimeState.Error || newState == RuntimeState.Failed ||
                newState == RuntimeState.Success)
            {
                this.EndTime = message.Time;
            }
            this.State = newState;
        }

        private void RefreshCommonStatus(EventInfoBase eventInfo, RuntimeState newState)
        {
            // 如果阻塞开始时间无效并且最新状态是非阻塞状态，则继续执行
            if (_blockedStart == DateTime.MaxValue &&
                (newState != RuntimeState.Blocked && newState != RuntimeState.DebugBlocked))
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
            else if (_blockedStart == DateTime.MaxValue &&
                     (newState == RuntimeState.Blocked || newState == RuntimeState.DebugBlocked))
            {
                this._blockedStart = eventInfo.TimeStamp;
            }
            this.ElapsedTime = eventInfo.TimeStamp - this.StartTime - this.BlockedTime;
            this.CurrentTime = eventInfo.TimeStamp;
            this.State = newState;
            if (newState == RuntimeState.Abort || newState == RuntimeState.Error || newState == RuntimeState.Failed ||
                newState == RuntimeState.Success)
            {
                this.EndTime = eventInfo.TimeStamp;
            }
        }

        private RuntimeStatusData WriteRuntimeStatusData(StepResult result, string watchData)
        {
            RuntimeStatusData statusData = new RuntimeStatusData()
            {
                Stack = this.RunStack,
                ElapsedTime = this.ElapsedTime.TotalMilliseconds,
                RuntimeHash = _stateManageContext.RuntimeHash,
                Sequence = this.SequenceIndex,
                Time = CurrentTime,
                Result = result,
                WatchData = watchData,
                Session = Session,
            };
            _stateManageContext.DatabaseProxy.WriteData(statusData);
            return statusData;
        }

        private SequenceResultData WriteSequenceResult(StepResult result, string failedInfo)
        {
            SequenceResultData resultData = new SequenceResultData()
            {
                Name = _sequence.Name,
                Description = _sequence.Description,
                ElapsedTime = this.ElapsedTime.TotalMilliseconds,
                StartTime = StartTime,
                EndTime = EndTime,
                Result = result,
                FailInfo = failedInfo,
                FailStack = RunStack,
                RuntimeHash = _stateManageContext.RuntimeHash,
                SequenceIndex = SequenceIndex,
            };
            _stateManageContext.DatabaseProxy.WriteData(resultData);
            return resultData;
        }

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
            _sequenceTestResult.FailedInfo.Description = failedInfo;
        }
    }
}