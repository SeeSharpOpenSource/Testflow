using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Utility.Collections;

namespace Testflow.MasterCore.EventData
{
    public class TestResultCollection : SerializableMap<int, ISequenceTestResult>, ITestResultCollection
    {
        public TestResultCollection(ISequenceGroup sequenceGroup, int session) : base(sequenceGroup.Sequences.Count + 2)
        {
            this.Session = session;
            this.SetUpSuccess = false;
            this.TearDownSuccess = false;
            this.SuccessCount = 0;
            this.FailedCount = 0;
            this.TimeOutCount = 0;
            this.AbortCount = 0;
            this.TestOver = false;
            this.WatchData = new Dictionary<IVariable, string>(Constants.DefaultRuntimeSize);

            this.Add(CommonConst.SetupIndex, new SequenceTestResult(session, CommonConst.SetupIndex));
            this.Add(CommonConst.TeardownIndex, new SequenceTestResult(session, CommonConst.TeardownIndex));
            for (int i = 0; i < sequenceGroup.Sequences.Count; i++)
            {
                this.Add(i, new SequenceTestResult(session, i));
            }
            this.Performance = null;
        }

        public TestResultCollection(ITestProject testProject) : base(2)
        {
            this.Session = CommonConst.TestGroupSession;
            this.SetUpSuccess = false;
            this.TearDownSuccess = false;
            this.SuccessCount = 0;
            this.FailedCount = 0;
            this.TimeOutCount = 0;
            this.AbortCount = 0;
            this.TestOver = false;
            this.WatchData = new Dictionary<IVariable, string>(Constants.DefaultRuntimeSize);
            this.Add(CommonConst.SetupIndex, new SequenceTestResult(Session, CommonConst.SetupIndex));
            this.Add(CommonConst.TeardownIndex, new SequenceTestResult(Session, CommonConst.TeardownIndex));
            this.Performance = null;
        }

        public TestResultCollection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int Session { get; set; }
        public bool SetUpSuccess { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int TimeOutCount { get; set; }
        public int AbortCount { get; set; }
        public bool TearDownSuccess { get; set; }
        public bool TestOver { get; set; }
        public IPerformanceResult Performance { get; set; }
        public IDictionary<IVariable, string> WatchData { get; set; }
    }
}