using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.SequenceElements;
using Testflow.SequenceManager.Serializer;
using Testflow.Utility.Collections;

namespace Testflow.SequenceManagerTest
{
    /// <summary>
    /// Summary description for SequenceManagerTest
    /// </summary>
    [TestClass]
    public class SequenceDeSerializeTest
    {
        public SequenceDeSerializeTest()
        {
            _sequenceManager = SequenceManager.SequenceManager.GetInstance();
            _configData = new TestConfigData();
            _configData.InitExtendProperties();
            _sequenceManager.ApplyConfig(_configData);
        }

        private TestContext testContextInstance;
        private SequenceManager.SequenceManager _sequenceManager;
        private TestConfigData _configData;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void TestXmlReader()
        {
            TestProject testProject = XmlReaderHelper.ReadTestProject(@"D:\testflow\test.tfproj");
        }

        [TestMethod]
        public void TestProjectDeSerialize()
        {
            ISequenceGroup sequenceGroup1 = _sequenceManager.CreateSequenceGroup();
            sequenceGroup1.Initialize(null);

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

            IFunctionData functionData = _sequenceManager.CreateFunctionData(new TestFuncDescription());
            sequenceStep1.Function = functionData;
            sequenceStep3.Function = functionData;
            functionData.Instance = "Variable1";
            functionData.Return = "Variable3";

            sequenceStep1.RetryCounter = new RetryCounter()
            {
                CounterVariable = "varialbe1",
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

            sequenceGroup1.Arguments.Add(new Argument()
            {
                Modifier = ArgumentModifier.None,
                Name = "TestArgument",
                Type = new TypeData()
                {
                    AssemblyName = "Assembly3",
                    Name = "ArgumentDemo",
                    Namespace = "Testflow.Test",
                },
                VariableType = VariableType.Value
            });

            _sequenceManager.Serialize(sequenceGroup1, SerializationTarget.File, @"D:\testflow\SequenceGroupTest\SequenceGroup.tfseq");
        }
    }
}
