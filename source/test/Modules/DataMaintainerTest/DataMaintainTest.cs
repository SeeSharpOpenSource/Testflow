using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testflow.Logger;
using Testflow.Runtime;
using Testflow.Runtime.Data;

namespace Testflow.DataMaintainerTest
{
    [TestClass]
    public class DataMaintainTest
    {
        private DataMaintainer.DataMaintainer _dataMaintainer;
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

        public DataMaintainTest()
        {
            Type runnerType = typeof(TestflowRunner);
            TestflowRunnerOptions option = new TestflowRunnerOptions();
            FakeTestflowRunner fakeTestflowRunner = new FakeTestflowRunner(option);
            Type intType = typeof(int);
            FieldInfo fieldInfo = runnerType.GetField("_runnerInst", BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, fakeTestflowRunner);
            fakeTestflowRunner.SetLogService(null);

            _dataMaintainer = new DataMaintainer.DataMaintainer();

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

            // 只有DesigntimeInitialize可以删掉记录
            _dataMaintainer.DesigntimeInitialize();
            // 删除记录
            _dataMaintainer.DeleteTestInstance($"RuntimeHash='{_testInstanceData1.RuntimeHash}'");
            _dataMaintainer.DeleteTestInstance($"RuntimeHash='{_testInstanceData2.RuntimeHash}'");

            // 只有RuntimeInitialize可以新增记录
            _dataMaintainer.RuntimeInitialize();

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
        }

        [TestInitialize]
        public void Initialize()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ReadInstanceCount()
        {
            _dataMaintainer.DesigntimeInitialize();
            int testInstanceCount = _dataMaintainer.GetTestInstanceCount(null);
            Assert.AreEqual(2, testInstanceCount);
        }

        [TestMethod]
        public void ReadInstance()
        {
            _dataMaintainer.DesigntimeInitialize();
            TestInstanceData testInstanceData1 = _dataMaintainer.GetTestInstance(_testInstanceData1.RuntimeHash);
            Assert.IsTrue(AreEqauls(_testInstanceData1, testInstanceData1));

            TestInstanceData testInstanceData2 = _dataMaintainer.GetTestInstance(_testInstanceData2.RuntimeHash);
            Assert.IsTrue(AreEqauls(_testInstanceData2, testInstanceData2));
        }

        [TestMethod]
        public void UpdateInstance()
        {
            _dataMaintainer.RuntimeInitialize();

            _testInstanceData1.EndTime = DateTime.Now;
            _dataMaintainer.UpdateData(_testInstanceData1);

            _testInstanceData2.EndTime = DateTime.Now;
            _dataMaintainer.UpdateData(_testInstanceData2);

            _dataMaintainer.DesigntimeInitialize();

            TestInstanceData testInstanceData1 = _dataMaintainer.GetTestInstance(_testInstanceData1.RuntimeHash);
            Assert.IsTrue(AreEqauls(_testInstanceData1, testInstanceData1));

            TestInstanceData testInstanceData2 = _dataMaintainer.GetTestInstance(_testInstanceData2.RuntimeHash);
            Assert.IsTrue(AreEqauls(_testInstanceData2, testInstanceData2));
        }

        [TestMethod]
        public void ReadSession()
        {
            _dataMaintainer.DesigntimeInitialize();

            IList<SessionResultData> sessionResultDatas1 = _dataMaintainer.GetSessionResults(_testInstanceData1.RuntimeHash);
            Assert.IsTrue(AreEqauls(_sessionResultData11, sessionResultDatas1[0]));
            Assert.IsTrue(AreEqauls(_sessionResultData12, sessionResultDatas1[1]));

            IList<SessionResultData> sessionResultDatas2 = _dataMaintainer.GetSessionResults(_testInstanceData2.RuntimeHash);
            Assert.IsTrue(AreEqauls(_sessionResultData21, sessionResultDatas2[0]));
            Assert.IsTrue(AreEqauls(_sessionResultData22, sessionResultDatas2[1]));

            SessionResultData sessionResultData1 = _dataMaintainer.GetSessionResult(_sessionResultData11.RuntimeHash, _sessionResultData11.Session);
            Assert.IsTrue(AreEqauls(_sessionResultData11, sessionResultData1));

            SessionResultData sessionResultData2 = _dataMaintainer.GetSessionResult(_sessionResultData12.RuntimeHash, _sessionResultData12.Session);
            Assert.IsTrue(AreEqauls(_sessionResultData12, sessionResultData2));

            SessionResultData sessionResultData3 = _dataMaintainer.GetSessionResult(_sessionResultData21.RuntimeHash, _sessionResultData21.Session);
            Assert.IsTrue(AreEqauls(_sessionResultData21, sessionResultData3));

            SessionResultData sessionResultData4 = _dataMaintainer.GetSessionResult(_sessionResultData22.RuntimeHash, _sessionResultData22.Session);
            Assert.IsTrue(AreEqauls(_sessionResultData22, sessionResultData4));
        }

        [TestMethod]
        public void UpdateSession()
        {
            _dataMaintainer.RuntimeInitialize();

            _sessionResultData11.State = RuntimeState.Failed;
            _dataMaintainer.UpdateData(_sessionResultData11);
            _sessionResultData12.State = RuntimeState.Failed;
            _dataMaintainer.UpdateData(_sessionResultData12);
            _sessionResultData21.State = RuntimeState.Failed;
            _dataMaintainer.UpdateData(_sessionResultData21);
            _sessionResultData22.State = RuntimeState.Failed;
            _dataMaintainer.UpdateData(_sessionResultData22);

            _dataMaintainer.DesigntimeInitialize();

            IList<SessionResultData> sessionResultDatas1 = _dataMaintainer.GetSessionResults(_testInstanceData1.RuntimeHash);
            Assert.IsTrue(AreEqauls(_sessionResultData11, sessionResultDatas1[0]));
            Assert.IsTrue(AreEqauls(_sessionResultData12, sessionResultDatas1[1]));

            IList<SessionResultData> sessionResultDatas2 = _dataMaintainer.GetSessionResults(_testInstanceData2.RuntimeHash);
            Assert.IsTrue(AreEqauls(_sessionResultData21, sessionResultDatas2[0]));
            Assert.IsTrue(AreEqauls(_sessionResultData22, sessionResultDatas2[1]));

            SessionResultData sessionResultData1 = _dataMaintainer.GetSessionResult(_sessionResultData11.RuntimeHash, _sessionResultData11.Session);
            Assert.IsTrue(AreEqauls(_sessionResultData11, sessionResultData1));

            SessionResultData sessionResultData2 = _dataMaintainer.GetSessionResult(_sessionResultData12.RuntimeHash, _sessionResultData12.Session);
            Assert.IsTrue(AreEqauls(_sessionResultData12, sessionResultData2));

            SessionResultData sessionResultData3 = _dataMaintainer.GetSessionResult(_sessionResultData21.RuntimeHash, _sessionResultData21.Session);
            Assert.IsTrue(AreEqauls(_sessionResultData21, sessionResultData3));

            SessionResultData sessionResultData4 = _dataMaintainer.GetSessionResult(_sessionResultData22.RuntimeHash, _sessionResultData22.Session);
            Assert.IsTrue(AreEqauls(_sessionResultData22, sessionResultData4));
        }

        [TestMethod]
        public void ReadSequence()
        {
            _dataMaintainer.RuntimeInitialize();

            _sequenceResultData111.FailStack = "1_1_2";
            _dataMaintainer.UpdateData(_sequenceResultData111);

            _sequenceResultData112.FailStack = "1_1_2";
            _dataMaintainer.UpdateData(_sequenceResultData112);

            _sequenceResultData121.FailStack = "1_1_2";
            _dataMaintainer.UpdateData(_sequenceResultData121);

            _sequenceResultData122.FailStack = "1_1_2";
            _dataMaintainer.UpdateData(_sequenceResultData122);

            _sequenceResultData211.FailStack = "1_1_2";
            _dataMaintainer.UpdateData(_sequenceResultData211);

            _sequenceResultData212.FailStack = "1_1_2";
            _dataMaintainer.UpdateData(_sequenceResultData212);

            _sequenceResultData221.FailStack = "1_1_2";
            _dataMaintainer.UpdateData(_sequenceResultData221);

            _sequenceResultData222.FailStack = "1_1_2";
            _dataMaintainer.UpdateData(_sequenceResultData222);

            _dataMaintainer.DesigntimeInitialize();

            IList<SequenceResultData> sequenceResultDatas1 = _dataMaintainer.GetSequenceResults(_testInstanceData1.RuntimeHash, 1);
            Assert.IsTrue(AreEqauls(_sequenceResultData111, sequenceResultDatas1[0]));
            Assert.IsTrue(AreEqauls(_sequenceResultData112, sequenceResultDatas1[1]));

            IList<SequenceResultData> sequenceResultDatas2 = _dataMaintainer.GetSequenceResults(_testInstanceData1.RuntimeHash, 2);
            Assert.IsTrue(AreEqauls(_sequenceResultData121, sequenceResultDatas2[0]));
            Assert.IsTrue(AreEqauls(_sequenceResultData122, sequenceResultDatas2[1]));

            IList<SequenceResultData> sequenceResultDatas3 = _dataMaintainer.GetSequenceResults(_testInstanceData2.RuntimeHash, 1);
            Assert.IsTrue(AreEqauls(_sequenceResultData211, sequenceResultDatas3[0]));
            Assert.IsTrue(AreEqauls(_sequenceResultData212, sequenceResultDatas3[1]));

            IList<SequenceResultData> sequenceResultDatas4 = _dataMaintainer.GetSequenceResults(_testInstanceData2.RuntimeHash, 2);
            Assert.IsTrue(AreEqauls(_sequenceResultData221, sequenceResultDatas4[0]));
            Assert.IsTrue(AreEqauls(_sequenceResultData222, sequenceResultDatas4[1]));

            SequenceResultData sequenceResultData = _dataMaintainer.GetSequenceResult(_sequenceResultData111.RuntimeHash, _sequenceResultData111.Session, _sequenceResultData111.SequenceIndex);
            Assert.IsTrue(AreEqauls(_sequenceResultData111, sequenceResultData));

            sequenceResultData = _dataMaintainer.GetSequenceResult(_sequenceResultData112.RuntimeHash, _sequenceResultData112.Session, _sequenceResultData112.SequenceIndex);
            Assert.IsTrue(AreEqauls(_sequenceResultData112, sequenceResultData));

            sequenceResultData = _dataMaintainer.GetSequenceResult(_sequenceResultData121.RuntimeHash, _sequenceResultData121.Session, _sequenceResultData121.SequenceIndex);
            Assert.IsTrue(AreEqauls(_sequenceResultData121, sequenceResultData));

            sequenceResultData = _dataMaintainer.GetSequenceResult(_sequenceResultData122.RuntimeHash, _sequenceResultData122.Session, _sequenceResultData122.SequenceIndex);
            Assert.IsTrue(AreEqauls(_sequenceResultData122, sequenceResultData));

            sequenceResultData = _dataMaintainer.GetSequenceResult(_sequenceResultData211.RuntimeHash, _sequenceResultData211.Session, _sequenceResultData211.SequenceIndex);
            Assert.IsTrue(AreEqauls(_sequenceResultData211, sequenceResultData));

            sequenceResultData = _dataMaintainer.GetSequenceResult(_sequenceResultData212.RuntimeHash, _sequenceResultData212.Session, _sequenceResultData212.SequenceIndex);
            Assert.IsTrue(AreEqauls(_sequenceResultData212, sequenceResultData));

            sequenceResultData = _dataMaintainer.GetSequenceResult(_sequenceResultData221.RuntimeHash, _sequenceResultData221.Session, _sequenceResultData221.SequenceIndex);
            Assert.IsTrue(AreEqauls(_sequenceResultData221, sequenceResultData));

            sequenceResultData = _dataMaintainer.GetSequenceResult(_sequenceResultData222.RuntimeHash, _sequenceResultData222.Session, _sequenceResultData222.SequenceIndex);
            Assert.IsTrue(AreEqauls(_sequenceResultData222, sequenceResultData));
        }

        [TestMethod]
        public void ReadRuntimeStatus()
        {
            _dataMaintainer.DesigntimeInitialize();

            IList<RuntimeStatusData> statusDatas = _dataMaintainer.GetRuntimeStatus(_sessionResultData11.RuntimeHash,
                _sessionResultData11.Session);
            Assert.IsTrue(AreEqauls(_runtimeStatusData1111, statusDatas[0]));
            Assert.IsTrue(AreEqauls(_runtimeStatusData1112, statusDatas[1]));

            statusDatas = _dataMaintainer.GetRuntimeStatus(_sessionResultData12.RuntimeHash,
                _sessionResultData12.Session);
            Assert.IsTrue(AreEqauls(_runtimeStatusData1211, statusDatas[0]));
            Assert.IsTrue(AreEqauls(_runtimeStatusData1212, statusDatas[1]));

            statusDatas = _dataMaintainer.GetRuntimeStatus(_sessionResultData21.RuntimeHash,
                _sessionResultData21.Session);
            Assert.IsTrue(AreEqauls(_runtimeStatusData2111, statusDatas[0]));
            Assert.IsTrue(AreEqauls(_runtimeStatusData2112, statusDatas[1]));

            statusDatas = _dataMaintainer.GetRuntimeStatus(_sessionResultData22.RuntimeHash,
                _sessionResultData22.Session);
            Assert.IsTrue(AreEqauls(_runtimeStatusData2211, statusDatas[0]));
            Assert.IsTrue(AreEqauls(_runtimeStatusData2212, statusDatas[1]));

            RuntimeStatusData statusData = _dataMaintainer.GetRuntimeStatusByIndex(_runtimeStatusData1111.RuntimeHash,
                _runtimeStatusData1111.StatusIndex);
            Assert.IsTrue(AreEqauls(_runtimeStatusData1111, statusData));

            statusData = _dataMaintainer.GetRuntimeStatusByIndex(_runtimeStatusData1112.RuntimeHash,
                _runtimeStatusData1112.StatusIndex);
            Assert.IsTrue(AreEqauls(_runtimeStatusData1112, statusData));

            statusData = _dataMaintainer.GetRuntimeStatusByIndex(_runtimeStatusData1211.RuntimeHash,
                _runtimeStatusData1211.StatusIndex);
            Assert.IsTrue(AreEqauls(_runtimeStatusData1211, statusData));

            statusData = _dataMaintainer.GetRuntimeStatusByIndex(_runtimeStatusData1212.RuntimeHash,
                _runtimeStatusData1212.StatusIndex);
            Assert.IsTrue(AreEqauls(_runtimeStatusData1212, statusData));

            statusData = _dataMaintainer.GetRuntimeStatusByIndex(_runtimeStatusData2111.RuntimeHash,
                _runtimeStatusData2111.StatusIndex);
            Assert.IsTrue(AreEqauls(_runtimeStatusData2111, statusData));

            statusData = _dataMaintainer.GetRuntimeStatusByIndex(_runtimeStatusData2211.RuntimeHash,
                _runtimeStatusData2211.StatusIndex);
            Assert.IsTrue(AreEqauls(_runtimeStatusData2211, statusData));

            statusData = _dataMaintainer.GetRuntimeStatusByIndex(_runtimeStatusData2211.RuntimeHash,
                _runtimeStatusData2211.StatusIndex);
            Assert.IsTrue(AreEqauls(_runtimeStatusData2211, statusData));

            statusData = _dataMaintainer.GetRuntimeStatusByIndex(_runtimeStatusData2212.RuntimeHash,
                _runtimeStatusData2212.StatusIndex);
            Assert.IsTrue(AreEqauls(_runtimeStatusData2212, statusData));
        }

        [TestMethod]
        public void ReadPerformance()
        {
            _dataMaintainer.DesigntimeInitialize();

            IList<PerformanceStatus> performanceStatus = _dataMaintainer.GetPerformanceStatus(
                _sessionResultData11.RuntimeHash, _sessionResultData11.Session);
            Assert.IsTrue(AreEqauls(_performanceStatus111, performanceStatus[0]));
            Assert.IsTrue(AreEqauls(_performanceStatus112, performanceStatus[1]));

            performanceStatus = _dataMaintainer.GetPerformanceStatus(_sessionResultData12.RuntimeHash,
                _sessionResultData12.Session);
            Assert.IsTrue(AreEqauls(_performanceStatus121, performanceStatus[0]));
            Assert.IsTrue(AreEqauls(_performanceStatus122, performanceStatus[1]));

            performanceStatus = _dataMaintainer.GetPerformanceStatus(_sessionResultData21.RuntimeHash,
                _sessionResultData21.Session);
            Assert.IsTrue(AreEqauls(_performanceStatus211, performanceStatus[0]));
            Assert.IsTrue(AreEqauls(_performanceStatus212, performanceStatus[1]));

            performanceStatus = _dataMaintainer.GetPerformanceStatus(_sessionResultData22.RuntimeHash,
                _sessionResultData22.Session);
            Assert.IsTrue(AreEqauls(_performanceStatus221, performanceStatus[0]));
            Assert.IsTrue(AreEqauls(_performanceStatus222, performanceStatus[1]));

            PerformanceStatus status = _dataMaintainer.GetPerformanceStatusByIndex(_performanceStatus111.RuntimeHash,
                _performanceStatus111.Index);
            Assert.IsTrue(AreEqauls(_performanceStatus111, status));

            status = _dataMaintainer.GetPerformanceStatusByIndex(_performanceStatus112.RuntimeHash,
                _performanceStatus112.Index);
            Assert.IsTrue(AreEqauls(_performanceStatus112, status));

            status = _dataMaintainer.GetPerformanceStatusByIndex(_performanceStatus121.RuntimeHash,
                _performanceStatus121.Index);
            Assert.IsTrue(AreEqauls(_performanceStatus121, status));

            status = _dataMaintainer.GetPerformanceStatusByIndex(_performanceStatus122.RuntimeHash,
                _performanceStatus122.Index);
            Assert.IsTrue(AreEqauls(_performanceStatus122, status));

            status = _dataMaintainer.GetPerformanceStatusByIndex(_performanceStatus211.RuntimeHash,
                _performanceStatus211.Index);
            Assert.IsTrue(AreEqauls(_performanceStatus211, status));

            status = _dataMaintainer.GetPerformanceStatusByIndex(_performanceStatus212.RuntimeHash,
                _performanceStatus212.Index);
            Assert.IsTrue(AreEqauls(_performanceStatus212, status));

            status = _dataMaintainer.GetPerformanceStatusByIndex(_performanceStatus221.RuntimeHash,
                _performanceStatus221.Index);
            Assert.IsTrue(AreEqauls(_performanceStatus221, status));

            status = _dataMaintainer.GetPerformanceStatusByIndex(_performanceStatus222.RuntimeHash,
                _performanceStatus222.Index);
            Assert.IsTrue(AreEqauls(_performanceStatus222, status));
        }


        [TestCleanup]
        public void TearDown()
        {
            // 只有DesigntimeInitialize可以删掉记录
            _dataMaintainer.DesigntimeInitialize();
            // 删除记录
            _dataMaintainer.DeleteTestInstance($"RuntimeHash='{_testInstanceData1.RuntimeHash}'");
            _dataMaintainer.DeleteTestInstance($"RuntimeHash='{_testInstanceData2.RuntimeHash}'");
            _dataMaintainer?.Dispose();
        }

        private static TestInstanceData CreateTestInstance(int id)
        {
            TestInstanceData testInstance = new TestInstanceData()
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
            return testInstance;
        }

        private static SessionResultData CreateSessionResult(int instanceId, int sessionid)
        {
            SessionResultData sessionResultData = new SessionResultData()
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
            return sessionResultData;
        }

        private static SequenceResultData CreateSequenceResult(int instanceId, int sessionid, int sequence)
        {
            SequenceResultData sequenceResult = new SequenceResultData()
            {
                RuntimeHash = $"RuntimeHash{instanceId}",
                Name = $"Session{sessionid}",
                Description = $"InstanceDescription{sessionid}",
                Session = sessionid,
                SequenceIndex = sequence,
                Result = RuntimeState.Success,
                StartTime = DateTime.Now - new TimeSpan(0, 0, 1, 1),
                EndTime = DateTime.Now - new TimeSpan(0, 0, 0, 1),
                ElapsedTime = 10000,
                FailInfo = "",
                FailStack = ""
            };
            return sequenceResult;
        }


        private RuntimeStatusData CreateRuntimeStatusData(int instanceId, int sessionid, int sequence, int dataIndex)
        {
            RuntimeStatusData runtimeStatusData = new RuntimeStatusData()
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

            return runtimeStatusData;
        }

        private PerformanceStatus CreatePerformanceData(int instanceId, int sessionid, int dataIndex)
        {
            PerformanceStatus performanceStatus = new PerformanceStatus()
            {
                RuntimeHash = $"RuntimeHash{instanceId}",
                Session = sessionid,
                Index = dataIndex,
                TimeStamp = DateTime.Now,
                MemoryUsed = 1000000,
                MemoryAllocated = 2000000,
                ProcessorTime = 500000,
            };

            return performanceStatus;
        }

        private bool AreEqauls(object expect, object real)
        {
            const string nullStr = "NULL";
            Type type = expect.GetType();
            if (type != real.GetType())
            {
                return false;
            }
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object value1 = propertyInfo.GetValue(expect);
                object value2 = propertyInfo.GetValue(real);
                if (null != value1 && !value1.Equals(value2))
                {
                    throw new AssertFailedException(
                        $"Property {propertyInfo.Name} not equal. Expect:<{value1}>; Real:<{value2 ?? nullStr}>");
                }
                else if (null != value2 && !value2.Equals(value1))
                {
                    throw new AssertFailedException(
                        $"Property {propertyInfo.Name} not equal. Expect:<{value1 ?? nullStr}>; Real:<{value2}>");
                }
            }
            return true;
        }
    }
}