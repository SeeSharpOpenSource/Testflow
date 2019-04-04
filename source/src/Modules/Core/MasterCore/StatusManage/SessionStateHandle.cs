using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.Common;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data.EventInfos;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using PerformanceData = Testflow.CoreCommon.Data.PerformanceData;

namespace Testflow.MasterCore.StatusManage
{
    internal class SessionStateHandle
    {
        private readonly EventDispatcher _eventDispatcher;
        private readonly StateManageContext _stateManageContext;
        private readonly ISequenceFlowContainer _sequenceData; 
        private readonly Dictionary<int, SequenceStateHandle> _sequenceHandles;

        public SessionStateHandle(ITestProject testProject, StateManageContext stateManageContext)
        {
            this._sequenceData = testProject;
            this.Session = CommonConst.TestGroupSession;
            this.RuntimeHash = stateManageContext.GlobalInfo.RuntimeHash;
            this.State = RuntimeState.NotAvailable;
            this._eventDispatcher = stateManageContext.EventDispatcher;
            this._stateManageContext = stateManageContext;
            this.StartGenTime = DateTime.MaxValue;
            this.EndGenTime = DateTime.MaxValue;
            this.StartTime = DateTime.MaxValue;
            this.EndTime = DateTime.MaxValue;
            this.CurrentTime = DateTime.MaxValue;
            this.ElapsedTime = TimeSpan.Zero;

            this._sequenceHandles = new Dictionary<int, SequenceStateHandle>(Constants.DefaultRuntimeSize);
            _sequenceHandles.Add(CommonConst.SetupIndex, new SequenceStateHandle(Session,
                testProject.SetUp, _stateManageContext));
            _sequenceHandles.Add(CommonConst.TeardownIndex, new SequenceStateHandle(Session,
                testProject.TearDown, _stateManageContext));

            _testResults = _stateManageContext.GetSessionResults(Session);
            _generationInfo = _stateManageContext.GetGenerationInfo(Session);
        }

        public SessionStateHandle(int session, ISequenceGroup sequenceGroup, StateManageContext stateManageContext)
        {
            // 配置基本信息
            this._sequenceData = sequenceGroup;
            this.Session = session;
            this.RuntimeHash = stateManageContext.GlobalInfo.RuntimeHash;
            this.State = RuntimeState.NotAvailable;
            this._eventDispatcher = stateManageContext.EventDispatcher;
            this._stateManageContext = stateManageContext;

            this.StartGenTime = DateTime.MaxValue;
            this.EndGenTime = DateTime.MaxValue;
            this.StartTime = DateTime.MaxValue;
            this.EndTime = DateTime.MaxValue;
            this.CurrentTime = DateTime.MaxValue;
            this.ElapsedTime = TimeSpan.Zero;
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
            // 获取测试结果对象和生成信息对象
            _testResults = _stateManageContext.GetSessionResults(Session);
            _generationInfo = _stateManageContext.GetGenerationInfo(Session);
        }

        public int Session { get; }

        public string RuntimeHash { get; }

        public ISequenceFlowContainer SequenceData => _sequenceData;

        private int _state;
        private readonly ITestResultCollection _testResults;
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

        public PerformanceData Performance { get; set; }

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
            this.StartGenTime = eventInfo.TimeStamp;
            // TODO 暂时不更新所有Sequence的状态，按照SequenceGroup为单位进行报告
            ISessionGenerationInfo generationInfo;
            switch (eventInfo.GenState)
            {
                case TestGenState.StartGeneration:
                    RefreshTime(eventInfo, false);
                    this.State = RuntimeState.TestGen;
                    generationInfo = SetGenerationInfo(eventInfo, GenerationStatus.InProgress);
                    _eventDispatcher.RaiseEvent(CoreConstants.TestGenerationStart, eventInfo.Session, generationInfo);
                    break;
                case TestGenState.GenerationOver:
                    RefreshTime(eventInfo, false);
                    generationInfo = SetGenerationInfo(eventInfo, GenerationStatus.Success);
                    _eventDispatcher.RaiseEvent(CoreConstants.TestGenerationEnd, eventInfo.Session, generationInfo);
                    break;
                case TestGenState.Error:
                    // 更新Handle状态
                    RefreshTime(eventInfo, true);
                    this.State = RuntimeState.Error;
                    // 持久化会话失败信息
                    this.WriteSessionResult(StepResult.Failed, eventInfo.ErrorInfo);
                    // 更新数据失败
                    foreach (SequenceStateHandle sequenceStateHandle in _sequenceHandles.Values)
                    {
                        sequenceStateHandle.StopStateHandle(eventInfo.TimeStamp, State, eventInfo.ErrorInfo);
                    }
                    _testResults.FailedCount = _testResults.Count;
                    // 触发生成失败的事件
                    generationInfo = SetGenerationInfo(eventInfo, GenerationStatus.Failed);
                    _eventDispatcher.RaiseEvent(CoreConstants.TestGenerationEnd, eventInfo.Session, generationInfo);
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
                RefreshTime(eventInfo, true);
                this.State = RuntimeState.Abort;
                foreach (SequenceStateHandle sequenceStateHandle in _sequenceHandles.Values)
                {
                    sequenceStateHandle.AbortEventProcess(eventInfo);
                }
                // todo
            }
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

        public bool HandleTestGenMessage(TestGenMessage message)
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

        public bool HandleStatusMessage(StatusMessage message)
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

        public void RefreshTime(MessageBase message, RuntimeState newState)
        {
            this.ElapsedTime = message.Time - this.StartTime;
            this.CurrentTime = message.Time;
//            Interlocked.Increment(ref _statusIndex);
            if (newState == RuntimeState.Abort || newState == RuntimeState.Error || newState == RuntimeState.Failed ||
                newState == RuntimeState.Success)
            {
                this.EndTime = message.Time;
            }
        }

        public void RefreshTime(EventInfoBase eventInfo, bool isOver)
        {
            this.ElapsedTime = eventInfo.TimeStamp - this.StartTime;
            this.CurrentTime = eventInfo.TimeStamp;
            if (isOver)
            {
                this.EndTime = eventInfo.TimeStamp;
            }
        }

        private ISessionGenerationInfo SetGenerationInfo(TestGenEventInfo eventInfo, GenerationStatus status)
        {
            _generationInfo.Status = status;
            foreach (int sequenceIndex in _generationInfo.SequenceStatus.Keys)
            {
                _generationInfo.SequenceStatus[sequenceIndex] = status;
            }
            return _generationInfo;
        }

        private SessionResultData WriteSessionResult(StepResult result, string failedInfo)
        {
            SessionResultData resultData = new SessionResultData()
            {
                Name = _sequenceData.Name,
                Description = _sequenceData.Description,
                ElapsedTime = ElapsedTime.TotalMilliseconds,
                StartTime = StartTime,
                EndTime = EndTime,
                RuntimeHash = _stateManageContext.RuntimeHash,
                SequenceHash = (_sequenceData as ISequenceGroup)?.Info.Hash ?? string.Empty,
                Session = Session
            };
            _stateManageContext.DatabaseProxy.WriteData(resultData);
            return resultData;
        }
    }
}