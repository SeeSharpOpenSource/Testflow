using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Data.Sequence;

namespace Testflow.EngineCoreTest
{
    [TestClass]
    public class RuntimeEngineTest
    {
        private SequenceCreator _sequenceCreator;

        public RuntimeEngineTest()
        {
            Type runnerType = typeof(TestflowRunner);
            TestflowRunnerOptions option = new TestflowRunnerOptions();
            FakeTestflowRunner fakeTestflowRunner = new FakeTestflowRunner(option);
            FieldInfo fieldInfo = runnerType.GetField("_runnerInst", BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, fakeTestflowRunner);

            fakeTestflowRunner.Initialize();

            _sequenceCreator = new SequenceCreator();
        }

        [TestMethod]
        public void TestProjectSequentialTes2t()
        {
            ITestProject testProjectData = _sequenceCreator.GetSequencialTestProjectData();
            TestflowRunner runnerInstance = TestflowRunner.GetInstance();
            runnerInstance.EngineController.SetSequenceData(testProjectData);
            runnerInstance.EngineController.Start();
        }
    }
}