using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Modules;
using System.Reflection;
using Testflow.Data.Description;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Data.Sequence;

namespace Testflow.ParameterCheckerTest
{
    [TestClass]
    public class ParameterCheckerTest
    {
        private ISequenceManager _sequenceManager;
        private IParameterChecker _parameterChecker;
        private IComInterfaceManager _interfaceManager;
        private const string path = @"C:\SeeSharp\JYTEK\Hardware\DAQ\JYUSB61902\Bin\";
        private const string testProjectPath = path + "ParameterCheckerTest.tfseq";

        public ParameterCheckerTest()
        {
            #region 创建并初始化假的TestFlowRunner
            Type runnerType = typeof(TestflowRunner);
            //默认options
            TestflowRunnerOptions option = new TestflowRunnerOptions();
            //创建假的TestFlowRunner
            FakeTestflowRunner fakeTestflowRunner = new FakeTestflowRunner(option);
            Type intType = typeof(int);
            //用反射将获取private fieldInfo，然后赋值fake
            FieldInfo fieldInfo = runnerType.GetField("_runnerInst", BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, fakeTestflowRunner);
            fakeTestflowRunner.Initialize();
            #endregion

            _parameterChecker = fakeTestflowRunner.ParameterChecker;
            _sequenceManager = fakeTestflowRunner.SequenceManager;
            _interfaceManager = fakeTestflowRunner.ComInterfaceManager;

            //IComInterfaceDescription description = _interfaceManager.GetComponentInterface(path + "JYUSB61902.dll");



        }

        [TestMethod]
        public void CheckSequence()
        {
            ISequenceGroup sequenceGroup = _sequenceManager.LoadSequenceGroup(Usr.SerializationTarget.File, testProjectPath);


        }
    }
}
