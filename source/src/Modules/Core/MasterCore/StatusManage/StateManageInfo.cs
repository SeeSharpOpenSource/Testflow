using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.EventData;
using Testflow.MasterCore.StatusManage.StatePersistance;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.MasterCore.StatusManage
{
    internal class StateManageInfo
    {
        private readonly ISequenceFlowContainer _sequenceData;
        private readonly ModuleGlobalInfo _globalInfo;

        public StateManageInfo(ModuleGlobalInfo globalInfo, ISequenceFlowContainer sequenceData)
        {
            this.GlobalInfo = globalInfo;
            this.RuntimeHash = globalInfo.RuntimeHash;
            this.EventDispatcher = new EventDispatcher();
            this.DatabaseProxy = new PersistenceProxy(globalInfo);
            this.TestGenerationInfo = new TestGenerationInfo(sequenceData);
            this.TestStatus = new TestProjectResults(sequenceData);
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
        public List<ITestResultCollection> TestStatus { get; }

        /// <summary>
        /// 事件收发器
        /// </summary>
        public EventDispatcher EventDispatcher { get; }

        /// <summary>
        /// 数据操作代理
        /// </summary>
        public PersistenceProxy DatabaseProxy { get; }

        public ModuleGlobalInfo GlobalInfo { get; }

        public bool IsAllTestOver => TestStatus.All(item => item.TestOver);

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
            return TestStatus.First(item => item.Session == session);
        }

        public ISequenceTestResult GetSequenceResults(int session, int sequenceIndex)
        {
            return TestStatus.First(item => item.Session == session)[sequenceIndex];
        }

    }
}