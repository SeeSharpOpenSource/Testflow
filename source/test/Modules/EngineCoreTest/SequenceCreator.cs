using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EngineCoreTestLib;
using Testflow.Data;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.SequenceElements;

namespace Testflow.EngineCoreTest
{
    public class SequenceCreator
    {
        private ISequenceManager _sequenceManager;
        private AssemblyInfo _intAssemblyInfo;
        private AssemblyInfo _testClassAssemblyInfo;
        private ITypeData _intTypeData;
        private ITypeData _testClassTypeData;
        private const string StaticMethodName = "StaticWait";
        private const string InstanceMethodName = "WaitTime";
        private const string ErrorMethodName = "RaiseError";
        private const string GetValueMethodName = "GetWaitTime";

        public SequenceCreator()
        {
            TestflowRunner testflowRunner = TestflowRunner.GetInstance();
            _sequenceManager = testflowRunner.SequenceManager;
            IComInterfaceManager interfaceManager = testflowRunner.ComInterfaceManager;

            Type testClassType = typeof(CoreTestClass);
            string location = testClassType.Assembly.Location;
            IComInterfaceDescription interfaceDescription = interfaceManager.GetComponentInterface(location);


            Type intType = typeof(int);
            _intTypeData = interfaceManager.GetTypeByName(intType.Name, intType.Namespace);
            _testClassTypeData = interfaceManager.GetTypeByName(testClassType.Name, testClassType.Namespace);
        }

        public ITestProject GetParallelTestProjectData()
        {
            throw new NotImplementedException();
        }

        public ITestProject GetSequencialTestProjectData()
        {
            ITestProject testProject = _sequenceManager.CreateTestProject();
            testProject.SetUp.Steps.Add(CreateStaticWaitStep());
            testProject.TearDown.Steps.Add(CreateStaticWaitStep());
            _sequenceManager.ValidateSequenceData(testProject);

            ISequenceGroup sequenceGroup = _sequenceManager.CreateSequenceGroup();
            sequenceGroup.ExecutionModel = ExecutionModel.SequentialExecution;
            sequenceGroup.Description = "SequenceGroup Description";

            ISequence sequence1 = _sequenceManager.CreateSequence();
            sequence1.Behavior = RunBehavior.Normal;
            sequence1.Description = "Sequence Description";

            sequence1.Steps.Add(CreateClassGenStep("instance", sequence1));
            sequence1.Steps.Add(CreateInstanceWaitStep("instance", "waitTime", sequence1));
            sequence1.Steps.Add(CreateGetFuncStep("instance", "totalTime", sequence1));

            sequenceGroup.Sequences.Add(sequence1);

            ISequence sequence2 = _sequenceManager.CreateSequence();
            sequence2.Behavior = RunBehavior.Normal;
            sequence2.Description = "Sequence Description";

            sequence2.Steps.Add(CreateClassGenStep("instance", sequence2));
            sequence2.Steps.Add(CreateInstanceWaitStep("instance", "waitTime", sequence2));
            sequence2.Steps.Add(CreateErrorStep("instance"));

            sequenceGroup.Sequences.Add(sequence2);
            _sequenceManager.ValidateSequenceData(sequenceGroup, testProject);

            testProject.SequenceGroups.Add(sequenceGroup);

            return testProject;
        }

        

        public ISequenceGroup GetParallelSequenceGroupData()
        {
            throw new NotImplementedException();
        }

        public ISequenceGroup GetSequencetialSequenceGroupData()
        {
            throw new NotImplementedException();
        }

        private ISequenceStep CreateStaticWaitStep()
        {
            // 添加静态Sleep的Step
            ISequenceStep step = _sequenceManager.CreateSequenceStep();
            step.RecordStatus = true;
            step.Function = new FunctionData()
            {
                ClassType = _testClassTypeData,
                ClassTypeIndex = 1,
                Description = null,
                Instance = string.Empty,
                MethodName = StaticMethodName,
                Return = string.Empty,
                Type = FunctionType.StaticFunction
            };
            step.Function.ParameterType = new ArgumentCollection();
            Argument intArgument = new Argument()
            {
                Modifier = ArgumentModifier.None,
                Name = "waitTime",
                Type = _intTypeData,
                TypeIndex = 0
            };
            step.Function.ParameterType.Add(intArgument);

            step.Function.Parameters = new ParameterDataCollection();
            ParameterData intParameterData = new ParameterData()
            {
                Index = 0,
                ParameterType = ParameterType.Value,
                Value = "200"
            };
            step.Function.Parameters.Add(intParameterData);
            return step;
        }

        private ISequenceStep CreateClassGenStep(string instanceName, ISequence parentSequence)
        {
            if (parentSequence.Variables.All(item => !item.Name.Equals(instanceName)))
            {
                // 添加实例变量
                Variable instanceVariable = new Variable()
                {
                    Description = $"Variable {instanceName}",
                    LogRecordLevel = RecordLevel.FinalResult,
                    Name = instanceName,
                    OIRecordLevel = RecordLevel.FinalResult,
                    Parent = parentSequence,
                    ReportRecordLevel = RecordLevel.FinalResult,
                    Type = _testClassTypeData,
                    TypeIndex = 1,
                    VariableType = VariableType.Class
                };
                parentSequence.Variables.Add(instanceVariable);
            }

            // 添加实例化类的step
            ISequenceStep step = _sequenceManager.CreateSequenceStep();
            step.RecordStatus = true;
            step.Function = new FunctionData()
            {
                ClassType = _testClassTypeData,
                ClassTypeIndex = 1,
                Description = null,
                Instance = instanceName,
                MethodName = _testClassTypeData.Name,
                Return = string.Empty,
                Type = FunctionType.Constructor
            };
            step.Function.ParameterType = new ArgumentCollection();
            Argument intArgument = new Argument()
            {
                Modifier = ArgumentModifier.None,
                Name = "waitTime",
                Type = _intTypeData,
                TypeIndex = 0
            };
            step.Function.ParameterType.Add(intArgument);

            step.Function.Parameters = new ParameterDataCollection();
            ParameterData intParameterData = new ParameterData()
            {
                Index = 0,
                ParameterType = ParameterType.Value,
                Value = "100"
            };
            step.Function.Parameters.Add(intParameterData);
            return step;
        }

        private ISequenceStep CreateInstanceWaitStep(string instanceName, string returnName, ISequence parentSequence)
        {
            if (parentSequence.Variables.All(item => !item.Name.Equals(returnName)))
            {
                // 添加实例变量
                Variable returnVar = new Variable()
                {
                    Description = $"Variable {returnName}",
                    LogRecordLevel = RecordLevel.FinalResult,
                    Name = returnName,
                    OIRecordLevel = RecordLevel.FinalResult,
                    Parent = parentSequence,
                    ReportRecordLevel = RecordLevel.FinalResult,
                    Type = _testClassTypeData,
                    TypeIndex = 1,
                    VariableType = VariableType.Class
                };
                parentSequence.Variables.Add(returnVar);
            }

            // 添加实例化类的step
            ISequenceStep step = _sequenceManager.CreateSequenceStep();
            step.RecordStatus = true;
            Argument intArgument = new Argument()
            {
                Modifier = ArgumentModifier.None,
                Name = "waitTime",
                Type = _intTypeData,
                TypeIndex = 0
            };
            step.Function = new FunctionData()
            {
                ClassType = _testClassTypeData,
                ClassTypeIndex = 1,
                Description = null,
                Instance = instanceName,
                MethodName = InstanceMethodName,
                Return = returnName,
                ReturnType = intArgument,
                Type = FunctionType.InstanceFunction
            };
            step.Function.ParameterType = new ArgumentCollection();
            step.Function.ParameterType.Add(intArgument);

            step.Function.Parameters = new ParameterDataCollection();
            ParameterData intParameterData = new ParameterData()
            {
                Index = 0,
                ParameterType = ParameterType.Value,
                Value = "100"
            };
            step.Function.Parameters.Add(intParameterData);
            return step;
        }

        private ISequenceStep CreateGetFuncStep(string instanceName, string returnName, ISequence parentSequence)
        {
            if (parentSequence.Variables.All(item => !item.Name.Equals(returnName)))
            {
                // 添加实例变量
                Variable returnVar = new Variable()
                {
                    Description = $"Variable {returnName}",
                    LogRecordLevel = RecordLevel.FinalResult,
                    Name = instanceName,
                    OIRecordLevel = RecordLevel.FinalResult,
                    Parent = parentSequence,
                    ReportRecordLevel = RecordLevel.FinalResult,
                    Type = _testClassTypeData,
                    TypeIndex = 1,
                    VariableType = VariableType.Class
                };
                parentSequence.Variables.Add(returnVar);
            }

            // 添加实例化类的step
            ISequenceStep step = _sequenceManager.CreateSequenceStep();
            step.RecordStatus = true;
            
            step.Function = new FunctionData()
            {
                ClassType = _testClassTypeData,
                ClassTypeIndex = 1,
                Description = null,
                Instance = instanceName,
                MethodName = GetValueMethodName,
                Return = string.Empty,
                ReturnType = null,
                Type = FunctionType.InstanceFunction
            };
            Argument intArgument = new Argument()
            {
                Modifier = ArgumentModifier.Out,
                Name = "waitTime",
                Type = _intTypeData,
                TypeIndex = 0
            };
            step.Function.ParameterType = new ArgumentCollection();
            step.Function.ParameterType.Add(intArgument);

            step.Function.Parameters = new ParameterDataCollection();
            ParameterData intParameterData = new ParameterData()
            {
                Index = 0,
                ParameterType = ParameterType.Value,
                Value = "100"
            };
            step.Function.Parameters.Add(intParameterData);
            return step;
        }

        private ISequenceStep CreateErrorStep(string instanceName)
        {
            ISequenceStep step = _sequenceManager.CreateSequenceStep();
            step.RecordStatus = true;

            step.Function = new FunctionData()
            {
                ClassType = _testClassTypeData,
                ClassTypeIndex = 1,
                Description = null,
                Instance = instanceName,
                MethodName = ErrorMethodName,
                Return = string.Empty,
                ReturnType = null,
                Type = FunctionType.InstanceFunction
            };
            return step;
        }
    }
}