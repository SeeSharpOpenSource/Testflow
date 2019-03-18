using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SequenceManager;
using Testflow.SequenceManager.SequenceElements;
using Testflow.SequenceManager.Serializer;

namespace Testflow.SequenceManagerTest
{
    /// <summary>
    /// Summary description for SequenceManagerTest
    /// </summary>
    [TestClass]
    public class SequenceDeserializeTest
    {
        private const string TestProjectPath = @"Test\test.tfproj";
        private const string SequenceGroupPath = @"Test\SequenceGroup1\SequenceGroup1.tfseq";
        private const string ParameterPath = @"Test\SequenceGroup1\SequenceGroup1.tfparam";
        private const string TestSequenceGroupPath = @"Test\SequenceGroup2\SequenceGroup2.tfseq";
        private const string TestParameterPath = @"Test\SequenceGroup3\SequenceGroup3.tfparam";

        public SequenceDeserializeTest()
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
        
        [TestInitialize]
        public void Initialize()
        {
            ITestProject testProject = _sequenceManager.CreateTestProject();

            testProject.Initialize(null);

            Assert.AreEqual(testProject.ModelVersion, _configData.GetProperty<string>("ModelVersion"));
            ISequenceGroup sequenceGroup1 = _sequenceManager.CreateSequenceGroup();
            ISequenceGroup sequenceGroup2 = _sequenceManager.CreateSequenceGroup();
            ISequenceGroup sequenceGroup3 = _sequenceManager.CreateSequenceGroup();
            sequenceGroup1.Initialize(testProject);
            testProject.SequenceGroups.Add(sequenceGroup1);


            sequenceGroup2.Initialize(testProject);
            testProject.SequenceGroups.Add(sequenceGroup2);

            sequenceGroup3.Initialize(testProject);
            testProject.SequenceGroups.Add(sequenceGroup3);

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

            _sequenceManager.Serialize(testProject, SerializationTarget.File, TestProjectPath);
        }


        [TestMethod]
        public void TestXmlReadTestProject()
        {
            TestProject testProject = XmlReaderHelper.ReadTestProject(TestProjectPath);
            Assert.AreEqual(testProject.Name, "TestProject1");
            Assert.AreEqual(testProject.Description, "");
            Assert.AreEqual(testProject.ModelVersion, "1.0.0");
            Assert.AreEqual(testProject.SetUp.Name, "");
            Assert.AreEqual(testProject.TearDown.Name, "");
            Assert.AreEqual(testProject.SequenceGroupLocations.Count, 3);
        }

        [TestMethod]
        public void TestXmlReadSequenceGroup()
        {
            SequenceGroup sequenceGroup = XmlReaderHelper.ReadSequenceGroup(SequenceGroupPath);
            Assert.AreEqual(sequenceGroup.Name, "SequenceGroup1");
            Assert.AreEqual(sequenceGroup.Info.Version, "1.0.0");
//            Assert.AreEqual(sequenceGroup.Info.SequenceGroupFile, SequenceGroupPath);
//            Assert.AreEqual(sequenceGroup.Info.SequenceParamFile, ParameterPath);
            Assert.AreEqual(sequenceGroup.TypeDatas.Count, 3);
            Assert.AreEqual(sequenceGroup.TypeDatas[0].AssemblyName, "TestAssemblyName");
            Assert.AreEqual(sequenceGroup.TypeDatas[1].Name, "Int32");
            Assert.AreEqual(sequenceGroup.Sequences.Count, 3);
            Assert.AreEqual(sequenceGroup.Sequences[0].Name, "Sequence1");
            Assert.AreEqual(sequenceGroup.Sequences[0].Variables.Count, 4);
            Assert.AreEqual(sequenceGroup.Sequences[0].Variables[0].VariableType, VariableType.Value);
            Assert.AreEqual(sequenceGroup.Sequences[0].Variables[1].Name, "Variable2");
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps.Count, 3);
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Name, "SequenceStep1");
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].HasSubSteps, false);
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.Type, FunctionType.InstanceFunction);
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.Instance, "Variable1");
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.Return, "Variable3");
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.MethodName, "Add");
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.ParameterType.Count, 2);
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.ParameterType[0].Name, "addSource");
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[2].LoopCounter.Name, "LoopCounterDemo");
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].RetryCounter.Name, "RetryCounterDemo");
        }

        [TestMethod]
        public void TestXmlReadSequenceGroupParameter()
        {
            SequenceGroup sequenceGroup = XmlReaderHelper.ReadSequenceGroup(SequenceGroupPath);
            SequenceGroupParameter parameter = XmlReaderHelper.ReadSequenceGroupParameter(ParameterPath);
            Assert.AreEqual(parameter.Info.Version, "1.0.0");
            Assert.AreEqual(parameter.Info.Hash, sequenceGroup.Info.Hash);
            Assert.AreEqual(parameter.SequenceParameters.Count, 3);
            Assert.AreEqual(parameter.SequenceParameters[0].StepParameters.Count, 3);
            Assert.AreEqual(parameter.SequenceParameters[0].StepParameters[0].Instance, "Variable1");
            Assert.AreEqual(parameter.SequenceParameters[0].StepParameters[0].Return, "Variable3");
        }

        [TestMethod]
        public void TestProjectDeserialize()
        {
            ITestProject testProject = _sequenceManager.LoadTestProject(SerializationTarget.File, TestProjectPath);

            Assert.AreEqual(testProject.Name, "TestProject1");
        }

        [TestMethod]
        public void SequenceGroupDeserialize()
        {
            ISequenceGroup sequenceGroup = _sequenceManager.LoadSequenceGroup(SerializationTarget.File, SequenceGroupPath);

            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.ClassType, sequenceGroup.TypeDatas[0]);
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.ReturnType.Type, sequenceGroup.TypeDatas[2]);
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.ParameterType[0].Type, sequenceGroup.TypeDatas[2]);
            Assert.AreEqual(sequenceGroup.Sequences[0].Steps[0].Function.ParameterType[1].Type, sequenceGroup.TypeDatas[2]);
        }

        [TestMethod]
        public void ParameterDeserializer()
        {
            ISequenceGroup sequenceGroup = _sequenceManager.LoadSequenceGroup(SerializationTarget.File, SequenceGroupPath);
            try
            {
                _sequenceManager.LoadParameter(sequenceGroup, false, TestParameterPath);
                Assert.Fail();
            }
            catch (TestflowDataException ex)
            {
                Assert.AreEqual(ex.ErrorCode, ModuleErrorCode.UnmatchedFileHash);
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            _sequenceManager.Dispose();
        }
    }
}
