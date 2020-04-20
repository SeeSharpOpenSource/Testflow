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
using PerformanceData = Testflow.CoreCommon.Data.PerformanceData;

namespace Testflow.MasterCore.StatusManage
{
    internal class SessionStateHandle
    {
        private readonly StateManageContext _stateManageContext;
        private ISequenceFlowContainer _sequenceData; 
        private readonly Dictionary<int, SequenceStateHandle> _sequenceHandles;

        public SessionStateHandle(ITestProject testProject, StateManageContext stateManageContext)
        {
            this._stateManageContext = stateManageContext;
            InitializeBasicInfo(CommonConst.TestGroupSession, testProject);

            this._sequenceHandles = new Dictionary<int, SequenceStateHandle>(Constants.DefaultRuntimeSize);
            _sequenceHandles.Add(CommonConst.SetupIndex, new SequenceStateHandle(Session,
                testProject.SetUp, _stateManageContext));
            _sequenceHandles.Add(CommonConst.TeardownIndex, new SequenceStateHandle(Session,
                testProject.TearDown, _stateManageContext));
        }

        public SessionStateHandle(int session, ISequenceGroup sequenceGroup, StateManageContext stateManageContext)
        {
            this._stateManageContext = stateManageContext;
            InitializeBasicInfo(session, sequenceGroup);

            // 初始化SequenceHandles
            this._sequenceHandles = new Dictionary<int, SequenceStateHandle>(Constants.DefaultRuntimeSize);
            _sequenceHandles.Add(CommonConst.SetupIndex, new SequenceStateHandle(Session, 
                sequenceGroup.SetUp, _stateManageContext));
            _sequenceHandles.Add(CommonConst.TeardownIndex, new SequenceStateHandle(Session, 
                sequenceGroup.TearDown, _stateManageContext));
            for (int i = 0; i < sequenceGroup.Sequences.Count; i++)
            {
                _sequenceHandles.Add(i, new SequenceStateHandle(Session, sequenceGroup.Sequences[i], _stateManageContext));
            }
        }

        private void InitializeBasicInfo(int session, ISequenceFlowContainer sequenceData)
        {
            // 配置基本信息
            this._sequenceData = sequenceData;
            this.Session = session;
            this.RuntimeHash = _stateManageContext.GlobalInfo.RuntimeHash;
            this.State = RuntimeState.NotAvailable;

            this.StartGenTime = DateTime.MaxValue;
            this.EndGenTime = DateTime.MaxValue;
            this.StartTime = DateTime.MaxValue;
            this.EndTime = DateTime.MaxValue;
            this.CurrentTime = DateTime.MaxValue;
            this.ElapsedTime = TimeSpan.Zero;

            // 获取测试结果对象和生成信息对象
            _testResults = _stateManageContext.GetSessionResults(Session);
            _generationInfo = _stateManageContext.GetGenerationInfo(Session);
            _sessionResults = new SessionResultData()
            {
                Name = SequenceData.Name,
                Description = SequenceData.Description,
                RuntimeHash = _stateManageContext.RuntimeHash,
                Session = this.Session,
                SequenceHash = (sequenceData is ISequenceGroup) ? ((ISequenceGroup)sequenceData).Info.Hash : string.Empty,
                State = RuntimeState.NotAvailable,
                FailedInfo = null
            };
            _performanceStatus = new PerformanceStatus()
            {
                RuntimeHash = _stateManageContext.RuntimeHash,
                Session = this.Session,
            };

            _stateManageContext.DatabaseProxy.WriteData(_sessionResults);
        }

        public int Session { get; private set; }

        public string RuntimeHash { get; private set; }

        public ISequenceFlowContainer SequenceData => _sequenceData;

        private int _state;
        private ITestResultCollection _testResults;
        private SessionResultData _sessionResults;
        private PerformanceStatus _performanceStatus;
        private ISessionGenerationInfo _generationInfo;

        public RuntimeState State
        {
            get { return (RuntimeState) _state; }
            set
            {
                if (_state == (int) value)
                {
                    return;
                }
                Thread.VolatileWrite(ref _state, (int)value);
            }
        }

        public DateTime StartGenTime { get; set; }

        public DateTime EndGenTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime CurrentTime { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public IEnumerable<int> SequenceIndexes => _sequenceHandles.Keys;

//        public PerformanceData Performance { get; set; }

        public void Start()
        {
            State = RuntimeState.Idle;
        }

        public SequenceStateHandle this[int sequenceIndex] => _sequenceHandles[sequenceIndex];

        public int SequenceCount => _sequenceHandles.Count;

        // 事件处理和消息处理需要完成：Handle状态更新、外部事件触发、数据库写入三个功能
        // SeesionHandle主要实现会话级别的管理(SequenceGroup)，例如TestGen、全局状态维护、会话时间统计、性能统计、全局结束和全局终止。
        // SequenceStateHandle实现序列级别的管理，例如当前栈维护、序列级别的时间统计、序列的时间维护。
        // 写入数据库的状态数据包含两部分，分别是以Sequence为单位和Session为单位执行统计
        // 该类的处理方法完成的工作有：
        // 更新SessionStateHandle的状态、生成PerformanceData并持久化、序列执行结束后生成SessionResultData并持久化、更新TestResult、整体执行结束后触发结束事件
        #region 消息处理和内部事件处理

        public void TestGenEventProcess(TestGenEventInfo eventInfo)
        {
            // TODO 暂时不更新所有Sequence的状态，按照SequenceGroup为单位进行报告
            switch (eventInfo.GenState)
            {
                case TestGenState.StartGeneration:
                    this.StartGenTime = eventInfo.TimeStamp;
                    RefreshTime(eventInfo);
                    SetGenerationInfo(eventInfo, GenerationStatus.InProgress);
                    SetStateHandleRuntimeState(RuntimeState.TestGen);
                    break;
                case TestGenState.GenerationOver:
                    this.EndGenTime = eventInfo.TimeStamp;
                    RefreshTime(eventInfo);
                    SetGenerationInfo(eventInfo, GenerationStatus.Success);
                    SetStateHandleRuntimeState(RuntimeState.StartIdle);
                    break;
                case TestGenState.Error:
                    // 更新Handle状态
                    this.State = RuntimeState.Error;
                    RefreshTime(eventInfo);
                    SetGenerationInfo(eventInfo, GenerationStatus.Failed);
                    SetStateHandleRuntimeState(RuntimeState.Error);
                    // 停止所有Handle，写入错误数据
                    foreach (SequenceStateHandle sequenceStateHandle in _sequenceHandles.Values)
                    {
                        sequenceStateHandle.StopStateHandle(eventInfo.TimeStamp, State, eventInfo.ErrorInfo);
                    }
                    // 持久化会话失败信息
                    UpdateSessionResultData(new FailedInfo(eventInfo.ErrorInfo));
                    // 更新TestResults信息
                    SetTestResultStatistics(null);
                    // 触发生成失败的事件
                    SetGenerationInfo(eventInfo, GenerationStatus.Failed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AbortEventProcess(AbortEventInfo eventInfo)
        {
            foreach (SequenceStateHandle sequenceStateHandle in _sequenceHandles.Values)
            {
                sequenceStateHandle.AbortEventProcess(eventInfo);
            }
            if (!eventInfo.IsRequest)
            {
                this.State = RuntimeState.AbortRequested;
            }
            else
            {
                this.State = RuntimeState.Abort;
                RefreshTime(eventInfo);

                SetTestResultStatistics(null);
                UpdateSessionResultData(null);
                _stateManageContext.EventDispatcher.RaiseEvent(Constants.SessionOver, Session, _testResults);
            }
        }

        public void DebugEventProcess(DebugEventInfo eventInfo)
        {
            _sequenceHandles[eventInfo.BreakPoint.Sequence].DebugEventProcess(eventInfo, this.SequenceData);
        }

        public void ExceptionEventProcess(ExceptionEventInfo eventInfo)
        {
        }

        public void SyncEventProcess(SyncEventInfo eventInfo)
        {
            // TODO
        }

        public bool HandleTestGenMessage(TestGenMessage message)
        {
            GenerationStatus generationState = message.State;
            _generationInfo.Status = generationState;
            ICollection<int> sequenceIndexes = new List<int>(_generationInfo.SequenceStatus.Keys);
            foreach (int sequenceIndex in sequenceIndexes)
            {
                _generationInfo.SequenceStatus[sequenceIndex] = generationState;
            }
            return true;
        }

        public bool HandleStatusMessage(StatusMessage message)
        {
            this.State = message.State;
            switch (message.Name)
            {
                case MessageNames.StartStatusName:
                    HandleStartStatusMessage(message);
                    break;
                case MessageNames.ReportStatusName:
                    HandleReportStatusMessage(message);
                    break;
                case MessageNames.ResultStatusName:
                    HandleResultStatusMessage(message);
                    break;
                case MessageNames.ErrorStatusName:
                    HandleErrorStatusMessage(message);
                    break;
                case MessageNames.HeartBeatStatusName:
                    HandleHeartBeatStatusMessage(message);
                    break;
                default:
                    throw new InvalidOperationException();
                    break;
            }
            return true;
        }

        private void HandleStartStatusMessage(StatusMessage message)
        {
            this.StartTime = message.Time;
            RefreshTime(message);

            UpdateSessionResultData(null);

            SetTestResultStatistics(message.WatchData);
            // 写入性能记录条目
            if (null != message.Performance)
            {
                WritePerformanceStatus(message.Performance);
            }
            for (int i = 0; i < message.Stacks.Count; i++)
            {
                if (message.SequenceStates[i] == RuntimeState.Running)
                {
                    _sequenceHandles[message.Stacks[i].Sequence].HandleStatusMessage(message, i);
                }
            }
            _testResults.Performance = _stateManageContext.DatabaseProxy.GetPerformanceResult(Session);

            _stateManageContext.EventDispatcher.RaiseEvent(Constants.SessionStart, Session, _testResults);
        }

        private void HandleReportStatusMessage(StatusMessage message)
        {
            IRuntimeStatusInfo runtimeStatusInfo;
            RefreshTime(message);
            for (int i = 0; i < message.Stacks.Count; i++)
            {
                SequenceStateHandle sequenceStateHandle = _sequenceHandles[message.Stacks[i].Sequence];
                RuntimeState sequenceState = sequenceStateHandle.State;
                if (sequenceState == RuntimeState.StartIdle || sequenceState == RuntimeState.Running ||
                    RuntimeState.Blocked == sequenceState || RuntimeState.DebugBlocked == sequenceState)
                {
                    sequenceStateHandle.HandleStatusMessage(message, i);
                }
            }

            runtimeStatusInfo = CreateRuntimeStatusInfo(message);

            // 写入性能记录条目
            if (null != message.Performance)
            {
                WritePerformanceStatus(message.Performance);
            }
            _stateManageContext.EventDispatcher.RaiseEvent(Constants.StatusReceived, Session, runtimeStatusInfo);
        }

        private void HandleResultStatusMessage(StatusMessage message)
        {
            this.EndTime = message.Time;
            RefreshTime(message);

            //foreach (SequenceStateHandle handle in _sequenceHandles.Values)
            //{
            //    if (!ModuleUtils.IsOver(handle.State))
            //    {
            //        handle.HandleStatusMessage(message, 0);
            //    }
            //}

            SetTestResultStatistics(message.WatchData);

            UpdateSessionResultData(null);

            _testResults.Performance = _stateManageContext.DatabaseProxy.GetPerformanceResult(Session);
            _testResults.TestOver = true;
            _stateManageContext.EventDispatcher.RaiseEvent(Constants.SessionOver, Session, _testResults);
        }

        private void HandleErrorStatusMessage(StatusMessage message)
        {
            this.EndTime = message.Time;
            RefreshTime(message);
            for (int i = 0; i < message.Stacks.Count; i++)
            {
                SequenceStateHandle sequenceStateHandle = _sequenceHandles[message.Stacks[i].Sequence];
                // 只更新未结束的状态
                if (!ModuleUtils.IsOver(sequenceStateHandle.State))
                {
                    sequenceStateHandle.HandleStatusMessage(message, i);
                }
            }

            UpdateSessionResultData(message.ExceptionInfo);

            SetTestResultStatistics(message.WatchData);
            // 写入性能记录条目
            if (null != message.Performance)
            {
                WritePerformanceStatus(message.Performance);
            }
            _testResults.Performance = _stateManageContext.DatabaseProxy.GetPerformanceResult(Session);
            _testResults.TestOver = true;
            _stateManageContext.EventDispatcher.RaiseEvent(Constants.SessionOver, Session, _testResults);
        }

        private void HandleHeartBeatStatusMessage(StatusMessage message)
        {
            IRuntimeStatusInfo runtimeStatusInfo;
            RefreshTime(message);
            for (int i = 0; i < message.Stacks.Count; i++)
            {
                if (message.SequenceStates[i] == RuntimeState.Running)
                {
                    _sequenceHandles[message.Stacks[i].Sequence].HandleStatusMessage(message, i);
                }
            }
            // 写入性能记录条目
            if (null != message.Performance)
            {
                WritePerformanceStatus(message.Performance);
            }
            runtimeStatusInfo = CreateRuntimeStatusInfo(message);
            _stateManageContext.EventDispatcher.RaiseEvent(Constants.StatusReceived, Session, runtimeStatusInfo);
            return;
        }

        #endregion

        private void RefreshTime(MessageBase message)
        {
            this.CurrentTime = message.Time;
            if (this.StartTime != DateTime.MaxValue)
            {
                this.ElapsedTime = message.Time - this.StartTime;
            }
            if (this.State > RuntimeState.AbortRequested)
            {
                this.EndTime = message.Time;
            }
        }

        private void RefreshTime(EventInfoBase eventInfo)
        {
            this.CurrentTime = eventInfo.TimeStamp;
            if (this.StartTime != DateTime.MaxValue)
            {
                this.ElapsedTime = eventInfo.TimeStamp - this.StartTime;
            }
            if (this.State > RuntimeState.AbortRequested)
            {
                this.EndTime = eventInfo.TimeStamp;
            }
        }

        #region 更新各个数据结构的值

        private ISessionGenerationInfo SetGenerationInfo(TestGenEventInfo eventInfo, GenerationStatus status)
        {
            _generationInfo.Status = status;
            IList<int> sequenceIndexes = new List<int>(_generationInfo.SequenceStatus.Keys);
            foreach (int sequenceIndex in sequenceIndexes)
            {
                _generationInfo.SequenceStatus[sequenceIndex] = status;
            }
            _generationInfo.ErrorStack = eventInfo.ErrorStack;
            _generationInfo.ErrorInfo = eventInfo.ErrorInfo;
            return _generationInfo;
        }

        private void SetStateHandleRuntimeState(RuntimeState state)
        {
            this.State = state;
            IList<int> sequenceIndexes = new List<int>(_generationInfo.SequenceStatus.Keys);
            foreach (int sequenceIndex in sequenceIndexes)
            {
                _sequenceHandles[sequenceIndex].State = state;
            }
        }

        private void UpdateSessionResultData(IFailedInfo failedInfo)
        {
            if (StartTime == DateTime.MaxValue)
            {
                StartTime = EndTime;
            }
            _sessionResults.StartTime = StartTime;
            _sessionResults.EndTime = EndTime;
            _sessionResults.ElapsedTime = ElapsedTime.TotalMilliseconds;
            _sessionResults.State = State;
            if (State == RuntimeState.Error)
            {
                _sessionResults.FailedInfo = failedInfo;
            }
            _stateManageContext.DatabaseProxy.UpdateData(_sessionResults);
        }

        private void WritePerformanceStatus(PerformanceData performance)
        {
            this._performanceStatus.Index = _stateManageContext.PerfStatusIndex;
            this._performanceStatus.TimeStamp = this.CurrentTime;
            this._performanceStatus.MemoryAllocated = performance.MemoryAllocated;
            this._performanceStatus.MemoryUsed = performance.MemoryUsed;
            this._performanceStatus.ProcessorTime = performance.ProcessorTime;

            this._stateManageContext.DatabaseProxy.WriteData(this._performanceStatus);
        }

        private void SetTestResultStatistics(IDictionary<string, string> watchData)
        {
            _testResults.WatchData.Clear();
            _testResults.SetUpSuccess = _testResults[CommonConst.SetupIndex].ResultState == RuntimeState.Success;
            _testResults.SuccessCount = (from result in _testResults.Values where (result.SequenceIndex >= 0 && result.ResultState == RuntimeState.Success) select result).Count();
            _testResults.FailedCount = (from result in _testResults.Values where (result.SequenceIndex >= 0 && result.ResultState == RuntimeState.Failed || result.ResultState == RuntimeState.Error) select result).Count();
            _testResults.TimeOutCount = (from result in _testResults.Values where (result.SequenceIndex >= 0 && result.ResultState == RuntimeState.Timeout) select result).Count();
            _testResults.TearDownSuccess = _testResults[CommonConst.TeardownIndex].ResultState == RuntimeState.Success;
            _testResults.AbortCount = (from result in _testResults.Values where (result.SequenceIndex >= 0 && result.ResultState == RuntimeState.Abort) select result).Count();
            _testResults.TestOver = _testResults.Values.All(item => item.ResultState > RuntimeState.AbortRequested);
            if (null != watchData)
            {
                Regex varNameRegex = new Regex(CoreUtils.GetVariableNameRegex(_sequenceData, Session));
                foreach (KeyValuePair<string, string> varToValue in watchData)
                {
                    if (varNameRegex.IsMatch(varToValue.Key))
                    {
                        IVariable variable = CoreUtils.GetVariable(_sequenceData, varToValue.Key);
                        _testResults.WatchData.Add(variable, varToValue.Value);
                    }
                }
            }
        }

        private IRuntimeStatusInfo CreateRuntimeStatusInfo(StatusMessage message)
        {
            Dictionary<string, string> watchData = message.WatchData;
            Dictionary<IVariable, string> varValues;
            if (null != watchData)
            {
                varValues = new Dictionary<IVariable, string>(watchData.Count);
                Regex varNameRegex = new Regex(CoreUtils.GetVariableNameRegex(_sequenceData, Session));
                foreach (KeyValuePair<string, string> varToValue in watchData)
                {
                    if (varNameRegex.IsMatch(varToValue.Key))
                    {
                        IVariable variable = CoreUtils.GetVariable(_sequenceData, varToValue.Key);
                        varValues.Add(variable, varToValue.Value);
                    }
                }
            }
            else
            {
                varValues = new Dictionary<IVariable, string>(1);
            }
            Dictionary<ICallStack, StepResult> stepResults = null;
            if (null != message.Results && message.Results.Count > 0)
            {
                stepResults = new Dictionary<ICallStack, StepResult>(message.Results.Count);
                List<CallStack> callStacks = message.Stacks;
                List<StepResult> results = message.Results;
                for (int i = 0; i < results.Count; i++)
                {
                    stepResults.Add(callStacks[i], results[i]);
                }
            }
            ulong dataStatusIndex = (ulong) _stateManageContext.EventStatusIndex;
            return new RuntimeStatusInfo(this, dataStatusIndex, message.FailedInfo, varValues, 
                message.Performance, stepResults);
        }

        #endregion

//        private void SetErrorTestResults()

//        {

//            _testResults.SetUpSuccess = false;

//            _testResults.SuccessCount = 0;

//            _testResults.FailedCount = _testResults.Count;

//            _testResults.TimeOutCount = 0;

//            _testResults.TearDownSuccess = false;

//            _testResults.TestOver = true;

//            _testResults.AbortCount = 0;

//        }
    }
}