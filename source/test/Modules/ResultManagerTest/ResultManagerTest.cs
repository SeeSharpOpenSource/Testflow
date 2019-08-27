using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Testflow.Runtime.Data;
using Testflow.Runtime;
using Testflow.Data;

namespace Testflow.ResultManagerTest
{
    [TestClass]
    public class ResultManagerTest
    {
        private DataMaintainer.DataMaintainer _dataMaintainer;
        private ResultManager.ResultManager _resultManager;
        private const string TestProjectFile = @"Test\report.txt";
        private const string TestProjectDirectory = @"Test\";

        #region 创造测试用数据包括:TestInstanceData, SessionResultData, SequenceResultData, RuntimeStatusData, PerformanceStatus
        private TestInstanceData _testInstanceData1;
        private TestInstanceData _testInstanceData2;
        private SessionResultData _sessionResultData11;
        private SequenceResultData _sequenceResultData111;
        private SequenceResultData _sequenceResultData112;
        private RuntimeStatusData _runtimeStatusData1111;
        private RuntimeStatusData _runtimeStatusData1112;
        private SessionResultData _sessionResultData12;
        private SequenceResultData _sequenceResultData121;
        private SequenceResultData _sequenceResultData122;
        private RuntimeStatusData _runtimeStatusData1211;
        private RuntimeStatusData _runtimeStatusData1212;
        private PerformanceStatus _performanceStatus111;
        private PerformanceStatus _performanceStatus112;
        private PerformanceStatus _performanceStatus121;
        private PerformanceStatus _performanceStatus122;
        private SessionResultData _sessionResultData21;
        private SequenceResultData _sequenceResultData211;
        private SequenceResultData _sequenceResultData212;
        private RuntimeStatusData _runtimeStatusData2111;
        private RuntimeStatusData _runtimeStatusData2112;
        private SessionResultData _sessionResultData22;
        private SequenceResultData _sequenceResultData221;
        private SequenceResultData _sequenceResultData222;
        private RuntimeStatusData _runtimeStatusData2211;
        private RuntimeStatusData _runtimeStatusData2212;
        private PerformanceStatus _performanceStatus211;
        private PerformanceStatus _performanceStatus212;
        private PerformanceStatus _performanceStatus221;
        private PerformanceStatus _performanceStatus222;
        #endregion

        #region 创建假数据方法
        private static TestInstanceData CreateTestInstance(int id)
        {
            return new TestInstanceData()
            {
                Name = $"TestInstance{id}",
                Description = $"InstanceDescription{id}",
                TestProjectName = $"TestProject{id}",
                TestProjectDescription = $"TestProject Description{id}",
                RuntimeHash = $"RuntimeHash{id}",
                StartGenTime = DateTime.Now - new TimeSpan(1, 1, 1, 1),
                EndGenTime = DateTime.Now - new TimeSpan(0, 1, 1, 1),
                StartTime = DateTime.Now - new TimeSpan(0, 0, 1, 1),
                EndTime = DateTime.Now - new TimeSpan(0, 0, 0, 1),
                ElapsedTime = 10000
            };
        }

        private static SessionResultData CreateSessionResult(int instanceId, int sessionid)
        {
            return new SessionResultData()
            {
                RuntimeHash = $"RuntimeHash{instanceId}",
                Name = $"Session{sessionid}",
                Description = $"InstanceDescription{sessionid}",
                Session = sessionid,
                SequenceHash = $"SequenceHash{instanceId}_{sessionid}",
                StartTime = DateTime.Now - new TimeSpan(0, 0, 1, 1),
                EndTime = DateTime.Now - new TimeSpan(0, 0, 0, 1),
                ElapsedTime = 10000,
                State = RuntimeState.Over,
                FailedInfo = ""
            };
        }

        private static SequenceResultData CreateSequenceResult(int instanceId, int sessionid, int sequence)
        {
            return new SequenceResultData()
            {
                RuntimeHash = $"RuntimeHash{instanceId}",
                Name = $"Sequence{sequence}",
                Description = $"InstanceDescription for sequence {sequence} of session {sessionid} of {instanceId}",
                Session = sessionid,
                SequenceIndex = sequence,
                Result = RuntimeState.Success,
                StartTime = DateTime.Now - new TimeSpan(0, 0, 1, 1),
                EndTime = DateTime.Now - new TimeSpan(0, 0, 0, 1),
                ElapsedTime = 10000,
                FailInfo = "",
                FailStack = ""
            };
        }

        private RuntimeStatusData CreateRuntimeStatusData(int instanceId, int sessionid, int sequence, int dataIndex)
        {
            return new RuntimeStatusData()
            {
                RuntimeHash = $"RuntimeHash{instanceId}",
                Session = sessionid,
                Sequence = sequence,
                StatusIndex = dataIndex,
                Time = DateTime.Now,
                Result = StepResult.Pass,
                ElapsedTime = 10000,
                Stack = "",
                WatchData = ""
            };
        }

        private PerformanceStatus CreatePerformanceData(int instanceId, int sessionid, int dataIndex)
        {
            return new PerformanceStatus()
            {
                RuntimeHash = $"RuntimeHash{instanceId}",
                Session = sessionid,
                Index = dataIndex,
                TimeStamp = DateTime.Now,
                MemoryUsed = 1000000,
                MemoryAllocated = 2000000,
                ProcessorTime = 500000,
            };
        }
        #endregion

        public ResultManagerTest()
        {
            #region 创建并初始化假的TestFlowRunner, 将创建的DataMaintainer赋予它
            Type runnerType = typeof(TestflowRunner);
            //默认options
            TestflowRunnerOptions option = new TestflowRunnerOptions();
            //创建假的TestFlowRunner
            FakeTestflowRunner fakeTestflowRunner = new FakeTestflowRunner(option);
            Type intType = typeof(int);
            //用反射将获取private fieldInfo，然后赋值fake
            FieldInfo fieldInfo = runnerType.GetField("_runnerInst", BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, fakeTestflowRunner);
            _dataMaintainer = new DataMaintainer.DataMaintainer();
            fakeTestflowRunner.SetDataMaintainer(_dataMaintainer);

            fakeTestflowRunner.Initialize();
            #endregion

            _resultManager = new ResultManager.ResultManager();

            #region 用此类里的方法创建测试用数据
            _testInstanceData1 = CreateTestInstance(1);
            _sessionResultData11 = CreateSessionResult(1, 1);
            _sequenceResultData111 = CreateSequenceResult(1, 1, 1);
            _sequenceResultData112 = CreateSequenceResult(1, 1, 2);
            _runtimeStatusData1111 = CreateRuntimeStatusData(1, 1, 1, 1);
            _runtimeStatusData1112 = CreateRuntimeStatusData(1, 1, 2, 2);
            _sessionResultData12 = CreateSessionResult(1, 2);
            _sequenceResultData121 = CreateSequenceResult(1, 2, 1);
            _sequenceResultData122 = CreateSequenceResult(1, 2, 2);
            _runtimeStatusData1211 = CreateRuntimeStatusData(1, 2, 1, 3);
            _runtimeStatusData1212 = CreateRuntimeStatusData(1, 2, 2, 4);
            _performanceStatus111 = CreatePerformanceData(1, 1, 1);
            _performanceStatus112 = CreatePerformanceData(1, 1, 2);
            _performanceStatus121 = CreatePerformanceData(1, 2, 3);
            _performanceStatus122 = CreatePerformanceData(1, 2, 4);

            _testInstanceData2 = CreateTestInstance(2);
            _sessionResultData21 = CreateSessionResult(2, 1);
            _sequenceResultData211 = CreateSequenceResult(2, 1, 1);
            _sequenceResultData212 = CreateSequenceResult(2, 1, 2);
            _runtimeStatusData2111 = CreateRuntimeStatusData(2, 1, 1, 1);
            _runtimeStatusData2112 = CreateRuntimeStatusData(2, 1, 2, 2);
            _sessionResultData22 = CreateSessionResult(2, 2);
            _sequenceResultData221 = CreateSequenceResult(2, 2, 1);
            _sequenceResultData222 = CreateSequenceResult(2, 2, 2);
            _runtimeStatusData2211 = CreateRuntimeStatusData(2, 2, 1, 3);
            _runtimeStatusData2212 = CreateRuntimeStatusData(2, 2, 2, 4);
            _performanceStatus211 = CreatePerformanceData(2, 1, 1);
            _performanceStatus212 = CreatePerformanceData(2, 1, 2);
            _performanceStatus221 = CreatePerformanceData(2, 2, 3);
            _performanceStatus222 = CreatePerformanceData(2, 2, 4);
            #endregion

            #region 初始化_dataMaintainer
            // 只有DesigntimeInitialize可以删掉记录
            _dataMaintainer.DesigntimeInitialize();
            // 删除记录
            _dataMaintainer.DeleteTestInstance($"RuntimeHash='{_testInstanceData1.RuntimeHash}'");
            _dataMaintainer.DeleteTestInstance($"RuntimeHash='{_testInstanceData2.RuntimeHash}'");

            // 只有RuntimeInitialize可以新增记录
            _dataMaintainer.RuntimeInitialize();
            #endregion

            #region 添加数据到数据库
            _dataMaintainer.AddData(_testInstanceData1);
            _dataMaintainer.AddData(_sessionResultData11);
            _dataMaintainer.AddData(_sequenceResultData111);
            _dataMaintainer.AddData(_sequenceResultData112);
            _dataMaintainer.AddData(_sessionResultData12);
            _dataMaintainer.AddData(_sequenceResultData121);
            _dataMaintainer.AddData(_sequenceResultData122);
            _dataMaintainer.AddData(_runtimeStatusData1211);
            _dataMaintainer.AddData(_runtimeStatusData1212);
            _dataMaintainer.AddData(_runtimeStatusData1111);
            _dataMaintainer.AddData(_runtimeStatusData1112);
            _dataMaintainer.AddData(_performanceStatus111);
            _dataMaintainer.AddData(_performanceStatus112);
            _dataMaintainer.AddData(_performanceStatus121);
            _dataMaintainer.AddData(_performanceStatus122);

            _dataMaintainer.AddData(_testInstanceData2);
            _dataMaintainer.AddData(_sessionResultData21);
            _dataMaintainer.AddData(_sequenceResultData211);
            _dataMaintainer.AddData(_sequenceResultData212);
            _dataMaintainer.AddData(_sessionResultData22);
            _dataMaintainer.AddData(_sequenceResultData221);
            _dataMaintainer.AddData(_sequenceResultData222);
            _dataMaintainer.AddData(_runtimeStatusData2211);
            _dataMaintainer.AddData(_runtimeStatusData2212);
            _dataMaintainer.AddData(_runtimeStatusData2111);
            _dataMaintainer.AddData(_runtimeStatusData2112);
            _dataMaintainer.AddData(_performanceStatus211);
            _dataMaintainer.AddData(_performanceStatus212);
            _dataMaintainer.AddData(_performanceStatus221);
            _dataMaintainer.AddData(_performanceStatus222);
            #endregion

        }

        [TestMethod]
        public void FilePrintResult()
        {
            _resultManager.DesigntimeInitialize();
            _resultManager.PrintReport(TestProjectFile, _testInstanceData1.RuntimeHash, ReportType.txt);
        }

        [TestMethod]
        public void DirectoryPrintResult()
        {
            _resultManager.DesigntimeInitialize();
            _resultManager.PrintReport(TestProjectDirectory, _testInstanceData2.RuntimeHash, ReportType.txt);
        }



        [TestInitialize]
        public void Initialize()
        {
            Assert.IsTrue(true);
        }

        [TestCleanup]
        public void TearDown()
        {
            // 只有DesigntimeInitialize可以删掉记录
            _dataMaintainer.DesigntimeInitialize();
            // 删除记录
            _dataMaintainer.DeleteTestInstance($"RuntimeHash='{_testInstanceData1.RuntimeHash}'");
            _dataMaintainer.DeleteTestInstance($"RuntimeHash='{_testInstanceData2.RuntimeHash}'");
            _resultManager?.Dispose();
        }


    }
}
