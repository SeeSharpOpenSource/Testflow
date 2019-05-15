using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testflow.Usr;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.Collections;
using Moq;

namespace Testflow.SequenceManagerTest
{
    /// <summary>
    /// Summary description for SequenceManagerTest
    /// </summary>
    [TestClass]
    public class SequenceSerializeTest
    {
        private const string TestProjectPath = @"Test\test.tfproj";
        private const string SequenceGroupPath = @"Test\SequenceGroup1\SequenceGroup1.tfseq";
        private const string ParameterPath = @"Test\SequenceGroup1\SequenceGroup1.tfparam";

        public SequenceSerializeTest()
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

        [TestMethod]
        public void TestProjectSerialize()
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

            IFunctionData functionData = _sequenceManager.CreateFunctionData(new TestFuncDescription());
            sequenceStep1.Function = functionData;
            sequenceStep3.Function = functionData;
            functionData.Instance = "Variable1";
            functionData.Return = "Variable3";

            sequenceStep1.RetryCounter = new RetryCounter()
            {
//                CounterVariable = "Varialbe1",
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
        public void SequenceGroupSerialize()
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

        [TestCleanup]
        public void TearDown()
        {
            _sequenceManager.Dispose();
        }
    }

    internal class TestConfigData : IModuleConfigData
    {
        public TestConfigData()
        {
            Properties = new SerializableMap<string, object>(10);
            this.Version = "1.1.0";
            this.Name = "";
        }

        public void InitExtendProperties()
        {
            Properties.Add("ModelVersion", "1.0.0");
        }

        public ISerializableMap<string, object> Properties { get; }

        public void SetProperty(string propertyName, object value)
        {
            this.Properties[propertyName] = value;
        }

        public object GetProperty(string propertyName)
        {
            return Properties[propertyName];
        }

        public TDataType GetProperty<TDataType>(string propertyName)
        {
            return (TDataType) Properties[propertyName];
        }

        public Type GetPropertyType(string propertyName)
        {
            if (null == Properties[propertyName])
            {
                return null;
            }
            return Properties[propertyName].GetType();
        }

        public bool ContainsProperty(string propertyName)
        {
            return Properties.ContainsKey(propertyName);
        }

        public IList<string> GetPropertyNames()
        {
            return Properties.Keys.ToArray();
        }

        public string Version { get; set; }
        public string Name { get; set; }
    }

    public class TestFuncDescription : IFuncInterfaceDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public FunctionType FuncType { get; set; }
        public int ComponentIndex { get; set; }
        public ITypeData ClassType { get; set; }
        public bool IsGeneric { get; set; }
        public IArgumentDescription Return { get; set; }
        public IList<IArgumentDescription> Arguments { get; set; }
        public string Signature { get; set; }

        public TestFuncDescription()
        {
            Name = "Add";
            Description = "Add two algorithms";
            FuncType = FunctionType.InstanceFunction;
            ComponentIndex = 0;
            ClassType = new TypeData()
            {
                AssemblyName = "TestAssemblyName",
                Name = "Algorithm",
                Namespace = "Testflow.Test"
            };
            IsGeneric = false;
            Return = new TestArgDescription()
            {
                Name = "ReturnValue",
                Description = "ArgumentDescription",
                ArgumentType = VariableType.Value,
                Type = new TypeData()
                {
                    AssemblyName = "TestAssemblyName",
                    Name = "Double",
                    Namespace = "System"
                },
                Modifier = ArgumentModifier.None,
            };
            Signature = "TestAddFunction";
            Arguments = new List<IArgumentDescription>(2);
            Arguments.Add(new TestArgDescription()
            {
                Name = "addSource",
                Description = "AddInput1",
                ArgumentType = VariableType.Value,
                Type = new TypeData()
                {
                    AssemblyName = "TestAssemblyName",
                    Name = "Double",
                    Namespace = "System"
                },
                Modifier = ArgumentModifier.None,
            });

            Arguments.Add(new TestArgDescription()
            {
                Name = "addSource",
                Description = "AddInput2",
                ArgumentType = VariableType.Value,
                Type = new TypeData()
                {
                    AssemblyName = "TestAssemblyName",
                    Name = "Double",
                    Namespace = "System"
                },
                Modifier = ArgumentModifier.None,
            });
        }

        public class TestArgDescription : IArgumentDescription
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public VariableType ArgumentType { get; set; }
            public ITypeData Type { get; set; }
            public ArgumentModifier Modifier { get; set; }
            public string DefaultValue { get; set; }
        }
    }
}
