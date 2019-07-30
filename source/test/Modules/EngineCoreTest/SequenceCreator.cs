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
        private ITypeData _testCallBackTypeData;
        private const string StaticMethodName = "StaticWait";
        private const string InstanceMethodName = "WaitTime";
        private const string ErrorMethodName = "RaiseError";
        private const string GetValueMethodName = "GetWaitTime";
        private const string CallBackMethodName = "StartForm";

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

            Type testCallBackType = typeof(CallBackClass);
            //同一个程序集，不需要再一次加载component
            //location = testClassType.Assembly.Location;
            //interfaceDescription = interfaceManager.GetComponentInterface(location);

            _testClassTypeData = interfaceManager.GetTypeByName(testClassType.Name, testClassType.Namespace);
            _testCallBackTypeData = interfaceManager.GetTypeByName(testCallBackType.Name, testCallBackType.Namespace);
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
            sequence1.Steps.Add(CreateInstanceWaitStep("instance", "200", sequence1, "totalWaitTime"));
            sequence1.Steps.Add(CreateGetFuncStep("instance", "totalTime", sequence1));

            sequenceGroup.Sequences.Add(sequence1);

            ISequence sequence2 = _sequenceManager.CreateSequence();
            sequence2.Behavior = RunBehavior.Normal;
            sequence2.Description = "Sequence Description";

            sequence2.Steps.Add(CreateClassGenStep("instance", sequence2));
            sequence2.Steps.Add(CreateInstanceVarParamWaitStep("instance", "waitTime", "300", sequence2, "totalWaitTime"));
            sequence2.Steps.Add(CreateErrorStep("instance"));

            sequenceGroup.Sequences.Add(sequence2);

            ISequence sequence3 = _sequenceManager.CreateSequence();
            sequence3.Behavior = RunBehavior.Normal;
            sequence3.Description = "Sequence Description";

            sequence3.Steps.Add(CreateClassGenStep("instance", sequence3));
            sequence3.Steps.Add(CreateInstanceWaitStep("instance", "200", sequence3, "totalWaitTime"));
            sequence3.Steps.Add(CreateErrorStep("instance"));

            sequenceGroup.Sequences.Add(sequence3);
            _sequenceManager.ValidateSequenceData(sequenceGroup, testProject);

            testProject.SequenceGroups.Add(sequenceGroup);

            return testProject;
        }

        #region CallBackTestProjects
        public ITestProject GetCallBackTestProjectData1()
        {
            ITestProject testProject = _sequenceManager.CreateTestProject();
            //testProject.SetUp.Steps.Add(CreateStaticWaitStep());
            //testProject.TearDown.Steps.Add(CreateStaticWaitStep());
            _sequenceManager.ValidateSequenceData(testProject);

            ISequenceGroup sequenceGroup = _sequenceManager.CreateSequenceGroup();
            sequenceGroup.ExecutionModel = ExecutionModel.SequentialExecution;
            sequenceGroup.Description = "SequenceGroup Description";

            ISequence sequence1 = _sequenceManager.CreateSequence();
            sequence1.Behavior = RunBehavior.Normal;
            sequence1.Description = "Sequence Description";

            sequence1.Steps.Add(CreateCallBackStep());
            sequenceGroup.Sequences.Add(sequence1);
            _sequenceManager.ValidateSequenceData(sequenceGroup, testProject);

            testProject.SequenceGroups.Add(sequenceGroup);
            return testProject;
        }
        
        public ITestProject GetCallBackTestProjectData2()
        {
            ITestProject testProject = _sequenceManager.CreateTestProject();
            //testProject.SetUp.Steps.Add(CreateStaticWaitStep());
            //testProject.TearDown.Steps.Add(CreateStaticWaitStep());
            _sequenceManager.ValidateSequenceData(testProject);

            ISequenceGroup sequenceGroup = _sequenceManager.CreateSequenceGroup();
            sequenceGroup.ExecutionModel = ExecutionModel.SequentialExecution;
            sequenceGroup.Description = "SequenceGroup Description";

            ISequence sequence1 = _sequenceManager.CreateSequence();
            sequence1.Behavior = RunBehavior.Normal;
            sequence1.Description = "Sequence Description";

            sequence1.Steps.Add(CreateCallBackStep(100, 200));
            sequenceGroup.Sequences.Add(sequence1);
            _sequenceManager.ValidateSequenceData(sequenceGroup, testProject);

            testProject.SequenceGroups.Add(sequenceGroup);
            return testProject;
        }
        #endregion

        public ISequenceGroup GetParallelSequenceGroupData()
        {
            throw new NotImplementedException();
        }

        public ISequenceGroup GetSequencetialSequenceGroupData()
        {
            throw new NotImplementedException();
        }

        private ISequenceStep CreateCallBackStep()
        {
            ISequenceStep step = _sequenceManager.CreateSequenceStep();
            step.RecordStatus = true;
            step.Function = new FunctionData()
            {
                ClassType = _testCallBackTypeData,
                ClassTypeIndex = 6,
                Description = null,
                Instance = string.Empty,
                MethodName = CallBackMethodName,
                Return = string.Empty,
                Type = FunctionType.CallBack
            };
            //无参
            //step.Function.ParameterType = null;
            //step.Function.Parameters = null;
            return step;
        }
        
        private ISequenceStep CreateCallBackStep(int x, int y)
        {
            ISequenceStep step = _sequenceManager.CreateSequenceStep();
            step.RecordStatus = true;
            step.Function = new FunctionData()
            {
                ClassType = _testCallBackTypeData,
                ClassTypeIndex = 6,
                Description = null,
                Instance = string.Empty,
                MethodName = CallBackMethodName,
                Return = string.Empty,
                Type = FunctionType.CallBack
            };

            #region 添加参数类型
            step.Function.ParameterType = new ArgumentCollection();
            Argument intArgument = new Argument()
            {
                Modifier = ArgumentModifier.None,
                Name = "x",
                Type = _intTypeData,
                TypeIndex = 0
            };
            step.Function.ParameterType.Add(intArgument);
            intArgument = new Argument()
            {
                Modifier = ArgumentModifier.None,
                Name = "y",
                Type = _intTypeData,
                TypeIndex = 0
            };
            step.Function.ParameterType.Add(intArgument);
            #endregion

            #region 添加参数值
            step.Function.Parameters = new ParameterDataCollection();
            ParameterData intParameterData = new ParameterData()
            {
                Index = 0,
                ParameterType = ParameterType.Value,
                Value = x.ToString()
            };
            step.Function.Parameters.Add(intParameterData);

            intParameterData = new ParameterData()
            {
                Index = 0,
                ParameterType = ParameterType.Value,
                Value = y.ToString()
            };
            step.Function.Parameters.Add(intParameterData);
            #endregion

            return step;
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
                TypeIndex = 0,
                VariableType = VariableType.Value
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

        private ISequenceStep CreateInstanceWaitStep(string instanceName, string paramValue, ISequence parentSequence, 
            string returnName)
        {
            if (parentSequence.Variables.All(item => !item.Name.Equals(returnName)))
            {
                // 添加实例变量
                Variable returnVar = new Variable()
                {
                    Description = returnName,
                    LogRecordLevel = RecordLevel.Trace,
                    Name = returnName,
                    OIRecordLevel = RecordLevel.Trace,
                    Parent = parentSequence,
                    ReportRecordLevel = RecordLevel.Trace,
                    Type = _intTypeData,
                    TypeIndex = 0,
                    VariableType = VariableType.Value
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
                TypeIndex = 0,
                VariableType = VariableType.Value
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
                Value = paramValue
            };
            step.Function.Parameters.Add(intParameterData);
            return step;
        }

        private ISequenceStep CreateInstanceVarParamWaitStep(string instanceName, string varName, string varValue, ISequence parentSequence, 
            string returnName)
        {
            if (parentSequence.Variables.All(item => !item.Name.Equals(varName)))
            {
                // 添加实例变量
                Variable paramVar = new Variable()
                {
                    Description = $"Variable {varName}",
                    LogRecordLevel = RecordLevel.Trace,
                    Name = varName,
                    OIRecordLevel = RecordLevel.Trace,
                    Parent = parentSequence,
                    ReportRecordLevel = RecordLevel.Trace,
                    Type = _intTypeData,
                    TypeIndex = 0,
                    VariableType = VariableType.Class,
                    Value = varValue
                };
                parentSequence.Variables.Add(paramVar);
            }

            if (parentSequence.Variables.All(item => !item.Name.Equals(returnName)))
            {
                // 添加实例变量
                Variable returnVar = new Variable()
                {
                    Description = $"Variable {returnName}",
                    LogRecordLevel = RecordLevel.Trace,
                    Name = returnName,
                    OIRecordLevel = RecordLevel.Trace,
                    Parent = parentSequence,
                    ReportRecordLevel = RecordLevel.Trace,
                    Type = _intTypeData,
                    TypeIndex = 0,
                    VariableType = VariableType.Value
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
                TypeIndex = 0,
                VariableType = VariableType.Value
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
                ParameterType = ParameterType.Variable,
                Value = varName
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
                    Name = returnName,
                    OIRecordLevel = RecordLevel.FinalResult,
                    Parent = parentSequence,
                    ReportRecordLevel = RecordLevel.FinalResult,
                    Type = _testClassTypeData,
                    TypeIndex = 1,
                    VariableType = VariableType.Class,
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
                TypeIndex = 0,
                VariableType = VariableType.Value
            };
            step.Function.ParameterType = new ArgumentCollection();
            step.Function.ParameterType.Add(intArgument);

            step.Function.Parameters = new ParameterDataCollection();
            ParameterData intParameterData = new ParameterData()
            {
                Index = 0,
                ParameterType = ParameterType.Value,
                Value = "100",
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