using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Data.Sequence;

namespace Testflow.SequenceManagerTest
{
    [TestClass]
    public class RuntimeDeserializeTest
    {
        public RuntimeDeserializeTest()
        {
            Type runnerType = typeof(TestflowRunner);
            TestflowRunnerOptions option = new TestflowRunnerOptions();
            FieldInfo fieldInfo = runnerType.GetField("_runnerInst", BindingFlags.Static | BindingFlags.NonPublic);
            FakeTestflowRunner fakeTestflowRunner = new FakeTestflowRunner(option);
            fieldInfo.SetValue(null, fakeTestflowRunner);

            _sequenceManager = new SequenceManager.SequenceManager();
            _configData = new ModuleConfigData();
            _configData.InitExtendProperties();
            _sequenceManager.ApplyConfig(_configData);
            Directory.CreateDirectory("Test");
        }

        private TestContext testContextInstance;
        private SequenceManager.SequenceManager _sequenceManager;
        private ModuleConfigData _configData;

        [TestMethod]
        public void TestProjectDeserialize()
        {
            ITestProject testProject = _sequenceManager.RuntimeDeserializeTestProject(JsonStrResource.testProject1Json);
        }

        [TestMethod]
        public void SequenceGroupDeserialize()
        {
            ISequenceGroup sequenceGroup = _sequenceManager.RuntimeDeserializeSequenceGroup(JsonStrResource.sequenceGroup1Json);
        }

        [TestCleanup]
        public void TearDown()
        {
            _sequenceManager.Dispose();
        }
    }
}