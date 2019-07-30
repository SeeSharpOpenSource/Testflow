using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Testflow.Data.Sequence;

namespace Testflow.EngineCoreTest
{
    [TestClass]
    public class CallBackTest
    {
        private SequenceCreator _sequenceCreator;

        public CallBackTest()
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
        public void CallBackWithoutParameters()
        {
            ITestProject testProjectData = _sequenceCreator.GetCallBackTestProjectData1();
            TestflowRunner runnerInstance = TestflowRunner.GetInstance();
            runnerInstance.EngineController.SetSequenceData(testProjectData);
            runnerInstance.EngineController.Start();
        }
        

        [TestMethod]
        public void CallBackWithParameters()
        {
            ITestProject testProjectData = _sequenceCreator.GetCallBackTestProjectData2();
            TestflowRunner runnerInstance = TestflowRunner.GetInstance();
            runnerInstance.EngineController.SetSequenceData(testProjectData);
            runnerInstance.EngineController.Start();
        }
    }
}
