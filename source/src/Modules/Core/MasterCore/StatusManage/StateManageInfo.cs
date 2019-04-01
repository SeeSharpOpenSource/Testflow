using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.EventData;
using Testflow.MasterCore.StatusManage.StatePersistance;
using Testflow.Runtime;

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
            this._statusIndex = -1;
        }

        public string RuntimeHash { get; }

        private long _statusIndex;
        /// <summary>
        /// 写入数据库的状态索引号
        /// </summary>
        public long StatusIndex {get { return Interlocked.Increment(ref _statusIndex); } }

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

        public ISequenceGenerationInfo GetGenerationInfo(int session)
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