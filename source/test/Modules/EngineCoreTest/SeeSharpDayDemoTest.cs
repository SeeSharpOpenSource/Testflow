﻿using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.Usr;

namespace Testflow.EngineCoreTest
{
    [TestClass]
    public class SeeSharpDayDemoTest
    {
        private SequenceCreator _sequenceCreator;

        public SeeSharpDayDemoTest()
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
        public void SequenceTest()
        {
            TestflowRunner runnerInstance = TestflowRunner.GetInstance();
            ISequenceManager sequenceManager = runnerInstance.SequenceManager;
            ISequenceGroup sequenceGroup = sequenceManager.LoadSequenceGroup(SerializationTarget.File,
                @"C:\Users\jingtao\Desktop\sequence.tfseq");
            runnerInstance.EngineController.SetSequenceData(sequenceGroup);
            runnerInstance.EngineController.Start();
        }
    }
}