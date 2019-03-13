using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.SequenceManagerTest
{
    [TestClass]
    public class RuntimeSerializeTest
    {
        private SequenceManager.SequenceManager _sequenceManager;
        private ITestProject _testProject;
        private IModuleConfigData _configData;

        public RuntimeSerializeTest()
        {
            Type runnerType = typeof(TestflowRunner);
            TestflowRunnerOptions option = new TestflowRunnerOptions();
            FieldInfo fieldInfo = runnerType.GetField("_runnerInst", BindingFlags.Static | BindingFlags.NonPublic);
            FakeTestflowRunner fakeTestflowRunner = new FakeTestflowRunner(option);
            fieldInfo.SetValue(null, fakeTestflowRunner);

            _sequenceManager = new SequenceManager.SequenceManager();
            _configData = new TestConfigData();
            _configData.InitExtendProperties();
            _sequenceManager.ApplyConfig(_configData);
            Directory.CreateDirectory("Test");
        }

        [TestInitialize]
        public void Initialize()
        {
            _testProject = _sequenceManager.CreateTestProject();

            _testProject.Initialize(null);

            Assert.AreEqual(_testProject.ModelVersion, _configData.GetProperty<string>("ModelVersion"));
            ISequenceGroup sequenceGroup1 = _sequenceManager.CreateSequenceGroup();
            ISequenceGroup sequenceGroup2 = _sequenceManager.CreateSequenceGroup();
            ISequenceGroup sequenceGroup3 = _sequenceManager.CreateSequenceGroup();
            sequenceGroup1.Initialize(_testProject);
            _testProject.SequenceGroups.Add(sequenceGroup1);


            sequenceGroup2.Initialize(_testProject);
            _testProject.SequenceGroups.Add(sequenceGroup2);

            sequenceGroup3.Initialize(_testProject);
            _testProject.SequenceGroups.Add(sequenceGroup3);

            ISequence sequence1 = _sequenceManager.CreateSequence();
            ISequence sequence2 = _sequenceManager.CreateSequence();
            ISequence sequence3 = _sequenceManager.CreateSequence();
            sequence1.Initialize(sequenceGroup1);
            sequenceGroup1.Sequences.Add(sequence1);

            sequence2.Initialize(sequenceGroup1);
            sequenceGroup1.Sequences.Add(sequence2);

            sequence3.Initialize(sequenceGroup1);
            sequenceGroup1.Sequences.Add(sequence3);

            Assert.AreEqual(sequence1.Index, 0);
            Assert.AreEqual(sequence2.Index, 1);
            Assert.AreEqual(sequence3.Index, 2);

            ISequenceStep sequenceStep1 = _sequenceManager.CreateSequenceStep();
            ISequenceStep sequenceStep2 = _sequenceManager.CreateSequenceStep();
            ISequenceStep sequenceStep3 = _sequenceManager.CreateSequenceStep();

            sequenceStep1.Initialize(sequence1);
            sequence1.Steps.Add(sequenceStep1);

            sequenceStep2.Initialize(sequence1);
            sequence1.Steps.Add(sequenceStep2);

            sequenceStep3.Initialize(sequence1);
            sequence1.Steps.Add(sequenceStep3);

            Assert.AreEqual(sequenceStep1.Index, 0);
            Assert.AreEqual(sequenceStep2.Index, 1);
            Assert.AreEqual(sequenceStep3.Index, 2);

            IVariable varialbe1 = _sequenceManager.CreateVarialbe();
            varialbe1.Initialize(sequence1);
            sequence1.Variables.Add(varialbe1);
            varialbe1.Value = "10";

            IVariable varialbe2 = _sequenceManager.CreateVarialbe();
            varialbe2.Initialize(sequence1);
            sequence1.Variables.Add(varialbe2);
            varialbe2.Value = "20";

            IVariable varialbe3 = _sequenceManager.CreateVarialbe();
            varialbe3.Initialize(sequence1);
            sequence1.Variables.Add(varialbe3);
            varialbe3.Value = "30";

            IVariable varialbe4 = _sequenceManager.CreateVarialbe();
            varialbe4.Initialize(sequence1);
            sequence1.Variables.Add(varialbe4);

            IFunctionData functionData = _sequenceManager.CreateFunctionData(new TestFuncDescription());
            sequenceStep1.Function = functionData;
            sequenceStep3.Function = functionData;
            functionData.Instance = "Variable1";
            functionData.Return = "Variable3";

            sequenceStep1.RetryCounter = new RetryCounter()
            {
                CounterVariable = "Variable4",
                MaxRetryTimes = 10,
                Name = "RetryCounterDemo",
                RetryEnabled = true
            };

            sequenceStep3.LoopCounter = new LoopCounter()
            {
                CounterVariable = "Variable2",
                CounterEnabled = true,
                MaxValue = 20,
                Name = "LoopCounterDemo"
            };
        }

        [TestMethod]
        public void TestProjectToJson()
        {
            string runtimeSerialize = _sequenceManager.RuntimeSerialize(_testProject);
            Assert.AreEqual(runtimeSerialize, JsonStrResource.testProject1Json);
        }

        [TestMethod]
        public void SequenceGroupToJson()
        {
            string runtimeSerialize = _sequenceManager.RuntimeSerialize(_testProject.SequenceGroups[0]);
            Assert.AreEqual(runtimeSerialize, JsonStrResource.sequenceGroup1Json);
        }

        [TestCleanup]
        public void TearDown()
        {
            _sequenceManager.Dispose();
        }
    }
}