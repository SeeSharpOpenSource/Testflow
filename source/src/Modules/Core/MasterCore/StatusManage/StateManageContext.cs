using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.EventData;
using Testflow.MasterCore.StatusManage.StatePersistance;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.MasterCore.StatusManage
{
    internal class StateManageContext
    {
        private readonly ISequenceFlowContainer _sequenceData;

        public StateManageContext(ModuleGlobalInfo globalInfo, ISequenceFlowContainer sequenceData)
        {
            this.GlobalInfo = globalInfo;
            this.RuntimeHash = globalInfo.RuntimeHash;
            this.EventDispatcher = new EventDispatcher(globalInfo, sequenceData);
            this.DatabaseProxy = new PersistenceProxy(globalInfo);
            this.TestGenerationInfo = new TestGenerationInfo(sequenceData);
            this.TestResults = new TestProjectResults(sequenceData);
            this.TestInstance = new TestInstanceData()
            {
                Name = GlobalInfo.ConfigData.GetProperty<string>("TestName"),
                Description = GlobalInfo.ConfigData.GetProperty<string>("TestDescription"),
                TestProjectName = sequenceData.Name,
                TestProjectDescription = sequenceData.Description,
                RuntimeHash = globalInfo.RuntimeHash,
                StartGenTime = DateTime.MaxValue,
                EndGenTime = DateTime.MaxValue,
                StartTime = DateTime.MaxValue,
                EndTime = DateTime.MinValue,
                ElapsedTime = 0
            };

            this._eventStatusIndex = -1;
            this._dataStatusIndex = -1;
            this._perfStatusIndex = -1;
        }

        public string RuntimeHash { get; }

        private long _eventStatusIndex;
        /// <summary>
        /// 事件触发的状态索引号
        /// </summary>
        public long EventStatusIndex {get { return Interlocked.Increment(ref _eventStatusIndex); } }

        private long _dataStatusIndex;

        /// <summary>
        /// 写入数据库的状态索引号
        /// </summary>
        public long DataStatusIndex { get { return Interlocked.Increment(ref _dataStatusIndex); } }

        private long _perfStatusIndex;
        /// <summary>
        /// 写入数据库的性能数据索引号
        /// </summary>
        public long PerfStatusIndex { get { return Interlocked.Increment(ref _perfStatusIndex); } }

        /// <summary>
        /// 测试生成状态信息集合
        /// </summary>
        public ITestGenerationInfo TestGenerationInfo { get; }

        /// <summary>
        /// 测试状态信息结合
        /// </summary>
        public TestProjectResults TestResults { get; }

        /// <summary>
        /// 测试实例信息(写入数据库)
        /// </summary>
        public TestInstanceData TestInstance { get; }

        /// <summary>
        /// 事件收发器
        /// </summary>
        public EventDispatcher EventDispatcher { get; }

        /// <summary>
        /// 数据操作代理
        /// </summary>
        public PersistenceProxy DatabaseProxy { get; }

        public ModuleGlobalInfo GlobalInfo { get; }

        public bool IsAllTestOver => TestResults.All(item => item.TestOver);

        public ISessionGenerationInfo GetGenerationInfo(int session)
        {
            if (session == CommonConst.TestGroupSession)
            {
                return TestGenerationInfo.RootGenerationInfo;
            }
            else
            {
                return TestGenerationInfo.GenerationInfos.First(item => item.Session == session);
            }
        }

        public ITestResultCollection GetSessionResults(int session)
        {
            return TestResults.First(item => item.Session == session);
        }

        public ISequenceTestResult GetSequenceResults(int session, int sequenceIndex)
        {
            return TestResults.First(item => item.Session == session)[sequenceIndex];
        }

    }
}