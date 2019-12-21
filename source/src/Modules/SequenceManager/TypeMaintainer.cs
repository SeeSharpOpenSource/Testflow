using System;
using System.Collections.Generic;
using System.Linq;
using Testflow.Usr;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Logger;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager
{
    internal class TypeMaintainer
    {
        private readonly IComInterfaceManager _comInterfaceManager;

        public TypeMaintainer()
        {
            TestflowRunner testflowRunner = TestflowRunner.GetInstance();
            _comInterfaceManager = testflowRunner.ComInterfaceManager;
        }

        /// <summary>
        /// 初始化变量和参数的类型数据
        /// </summary>
        public void VerifyVariableTypes(ITestProject testProject)
        {
            VariableTreeTable variableTree = new VariableTreeTable(null);
            variableTree.Push(testProject.Variables);
            VerifyVariableTypes(testProject.SetUp, variableTree);
            VerifyVariableTypes(testProject.TearDown, variableTree);
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                if (null != sequenceGroup && sequenceGroup.Available)
                {
                    VerifyVariableTypes(sequenceGroup);
                }
            }
        }
        
        /// <summary>
        /// 初始化变量和参数的类型数据
        /// </summary>
        public void VerifyVariableTypes(ISequenceGroup sequenceGroup)
        {
            VariableTreeTable variableTree = new VariableTreeTable(sequenceGroup.Arguments);
            variableTree.Push(sequenceGroup.Variables);
            VerifyVariableTypes(sequenceGroup.SetUp, variableTree);
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                VerifyVariableTypes(sequence, variableTree);
            }
            VerifyVariableTypes(sequenceGroup.TearDown, variableTree);
            variableTree.Pop();
        }

        private void VerifyVariableTypes(ISequence sequence, VariableTreeTable variableTree)
        {
            variableTree.Push(sequence.Variables);
            foreach (ISequenceStep sequenceStep in sequence.Steps)
            {
                VerifyVariableTypes(sequenceStep, variableTree);
            }
            variableTree.Pop();
        }

        private void VerifyVariableTypes(ISequenceStep sequenceStep, VariableTreeTable variableTree)
        {
            if (!string.IsNullOrWhiteSpace(sequenceStep.LoopCounter?.CounterVariable))
            {
                string variableName = ModuleUtils.GetVarNameByParamValue(sequenceStep.LoopCounter.CounterVariable);
                IVariable variable = variableTree.GetVariable(variableName);
                Type varType = typeof(int);
                // Argument不能作为遍历变量
                if (null == variable)
                {
                    ThrowIfVariableNotFound(variableName, sequenceStep);
                }
                else if (!ModuleUtils.IsPropertyParam(sequenceStep.LoopCounter.CounterVariable) && 
                    (variable.Type == null || !variable.Type.Name.Equals(varType.Name)))
                {
                    
                    variable.Type = _comInterfaceManager.GetTypeByName(varType.Name, varType.Namespace);
                }
            }

            if (!string.IsNullOrWhiteSpace(sequenceStep.RetryCounter?.CounterVariable))
            {
                string variableName = ModuleUtils.GetVarNameByParamValue(sequenceStep.RetryCounter.CounterVariable);
                IVariable variable = variableTree.GetVariable(variableName);
                // Argument不能作为遍历变量
                if (null == variable)
                {
                    ThrowIfVariableNotFound(variableName, sequenceStep);
                }
                else if (!ModuleUtils.IsPropertyParam(sequenceStep.RetryCounter.CounterVariable))
                {
                    Type varType = typeof(int);
                    ITypeData typeData = _comInterfaceManager.GetTypeByName(varType.Name, varType.Namespace);
                    variable.Type = typeData;
                }
            }
            if (null != sequenceStep.Function)
            {
                IFunctionData functionData = sequenceStep.Function;
                if (!string.IsNullOrWhiteSpace(functionData.Instance))
                {
                    SetVariableAndArgumentType(functionData.Instance, functionData.ClassType, variableTree, sequenceStep);
                }
                if (!string.IsNullOrWhiteSpace(functionData.Return))
                {
                    SetVariableAndArgumentType(functionData.Return, functionData.ReturnType.Type, variableTree, sequenceStep);
                }
                for (int i = 0; i < functionData.ParameterType.Count; i++)
                {
                    IParameterData parameterValue = functionData.Parameters[i];
                    if (parameterValue.ParameterType == ParameterType.Variable &&
                        !string.IsNullOrWhiteSpace(parameterValue.Value))
                    {
                        SetVariableAndArgumentType(parameterValue.Value, functionData.ParameterType[i].Type, variableTree,
                            sequenceStep);
                    }
                }
            }
            if (sequenceStep.HasSubSteps)
            {
                foreach (ISequenceStep subStep in sequenceStep.SubSteps)
                {
                    VerifyVariableTypes(subStep, variableTree);
                }
            }
            
        }

        private void SetVariableAndArgumentType(string paramValue, ITypeData type, VariableTreeTable variableTree, 
            ISequenceFlowContainer parent)
        {
            IVariable variable;
            IArgument argument;
            string variableName = ModuleUtils.GetVarNameByParamValue(paramValue);
            if (null != (variable = variableTree.GetVariable(variableName)))
            {
                if (!ModuleUtils.IsPropertyParam(paramValue))
                {
                    variable.Type = type;
                }
            }
            else if (null != (argument = variableTree.GetArgument(variableName)))
            {
                if (!ModuleUtils.IsPropertyParam(paramValue))
                {
                    argument.Type = type;
                }
            }
            else
            {
                ThrowIfVariableNotFound(variableName, parent);
            }
        }

        private void ThrowIfVariableNotFound(string variableName, ISequenceFlowContainer parent)
        {
            ILogService logService = TestflowRunner.GetInstance().LogService;
            logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0,
                $"Undeclared variable '{0}' in sequence '{parent.Name}'");
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            throw new TestflowDataException(ModuleErrorCode.VariableError,
                i18N.GetFStr("UndeclaredVariable", variableName, parent.Name));
        }

        /// <summary>
        /// 初始化变量和参数的类型数据
        /// </summary>
        public bool VerifyVariableType(ISequenceFlowContainer flowContainer, IVariable variable, TypeDataCollection typeDatas)
        {
            if (flowContainer is ITestProject)
            {
                ITestProject testProject = flowContainer as ITestProject;
                return VerifyVariableInSequence(testProject.SetUp, variable, typeDatas) ||
                       VerifyVariableInSequence(testProject.TearDown, variable, typeDatas);

            }
            else if (flowContainer is ISequenceGroup)
            {
                ISequenceGroup sequenceGroup = flowContainer as ISequenceGroup;
                if (VerifyVariableInSequence(sequenceGroup.SetUp, variable, typeDatas) ||
                    VerifyVariableInSequence(sequenceGroup.TearDown, variable, typeDatas))
                {
                    return true;
                }
                return sequenceGroup.Sequences.Any(sequence => VerifyVariableInSequence(sequence, variable, typeDatas));
            }
            else if (flowContainer is ISequence)
            {
                return VerifyVariableInSequence(flowContainer as ISequence, variable, typeDatas);
            }
            else if (flowContainer is ISequenceStep)
            {
                return VerifyVariableInSequenceStep(flowContainer as ISequenceStep, variable, typeDatas);
            }
            return false;
        }
        
        private bool VerifyVariableInSequence(ISequence sequence, IVariable variable, TypeDataCollection typeDatas)
        {
            if (sequence.Variables.Any(var => var.Name.Equals(variable.Name)))
            {
                return false;
            }
            foreach (ISequenceStep sequenceStep in sequence.Steps)
            {
                bool verified = VerifyVariableInSequenceStep(sequenceStep, variable, typeDatas);
                if (verified)
                {
                    return true;
                }
            }
            return false;
        }

        private bool VerifyVariableInSequenceStep(ISequenceStep sequenceStep, IVariable variable, TypeDataCollection typeDatas)
        {
            if ((null != sequenceStep.LoopCounter && variable.Name.Equals(sequenceStep.LoopCounter.CounterVariable)) ||
                    (null != sequenceStep.RetryCounter) && variable.Name.Equals(sequenceStep.RetryCounter.CounterVariable))
            {
                Type varType = typeof(int);
                ITypeData typeData = typeDatas.GetTypeData(varType.Name);
                if (null == typeData)
                {
                    variable.Type = _comInterfaceManager.GetTypeByName(varType.Name, varType.Namespace);
                }
                return true;
            }
            if (null != sequenceStep.Function)
            {
                IFunctionData functionData = sequenceStep.Function;
                if (variable.Name.Equals(functionData.Instance))
                {
                    variable.Type = functionData.ClassType;
                    return true;
                }
                if (variable.Name.Equals(functionData.Return))
                {
                    variable.Type = functionData.ReturnType.Type;
                    return true;
                }
                IParameterData parameter = functionData.Parameters.FirstOrDefault
                    (item => item.ParameterType == ParameterType.Variable && variable.Name.Equals(item.Value));
                if (null != parameter)
                {
                    variable.Type = functionData.ParameterType[parameter.Index].Type;
                    return true;
                }
            }
            if (sequenceStep.HasSubSteps)
            {
                foreach (ISequenceStep subStep in sequenceStep.SubSteps)
                {
                    bool verified = VerifyVariableInSequenceStep(subStep, variable, typeDatas);
                    if (verified)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region 更新TypeDatas和Assemblies，删除无用的、增加被依赖的

        public void RefreshUsedAssemblyAndType(ITestProject testProject)
        {
            HashSet<ITypeData> typeDatas = new HashSet<ITypeData>();
            AddVariableTypeDatas(typeDatas, testProject.Variables);
            AddSequenceTypeDatas(typeDatas, testProject.SetUp);
            AddSequenceTypeDatas(typeDatas, testProject.TearDown);

            RefreshUsedTypeDatas(testProject.TypeDatas, typeDatas);

            HashSet<string> assemblyNames = new HashSet<string>();
            foreach (ITypeData typeData in typeDatas)
            {
                assemblyNames.Add(typeData.AssemblyName);
            }
            RefreshUsedAssemblies(testProject.Assemblies, assemblyNames);

            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                if (null != sequenceGroup && sequenceGroup.Available)
                {
                    RefreshUsedAssemblyAndType(sequenceGroup);
                }
            }
        }

        public void RefreshUsedAssemblyAndType(ISequenceGroup sequenceGroup)
        {
            HashSet<ITypeData> typeDatas = new HashSet<ITypeData>();
            AddArgumentTypeDatas(typeDatas, sequenceGroup.Arguments);
            AddVariableTypeDatas(typeDatas, sequenceGroup.Variables);
            AddSequenceTypeDatas(typeDatas, sequenceGroup.SetUp);
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                AddSequenceTypeDatas(typeDatas, sequence);
            }
            AddSequenceTypeDatas(typeDatas, sequenceGroup.TearDown);

            RefreshUsedTypeDatas(sequenceGroup.TypeDatas, typeDatas);

            HashSet<string> assemblyNames = new HashSet<string>();
            foreach (ITypeData typeData in typeDatas)
            {
                assemblyNames.Add(typeData.AssemblyName);
            }
            RefreshUsedAssemblies(sequenceGroup.Assemblies, assemblyNames);
        }

        private void AddVariableTypeDatas(HashSet<ITypeData> typeDatas, IVariableCollection variables)
        {
            foreach (IVariable variable in variables.Where(variable => null != variable.Type))
            {
                typeDatas.Add(variable.Type);
            }
        }

        private void AddArgumentTypeDatas(HashSet<ITypeData> typeDatas, IArgumentCollection arguments)
        {
            foreach (IArgument variable in arguments.Where(argument => null != argument.Type))
            {
                typeDatas.Add(variable.Type);
            }
        }

        private void AddSequenceTypeDatas(HashSet<ITypeData> typeDatas, ISequence sequence)
        {
            AddVariableTypeDatas(typeDatas, sequence.Variables);
            foreach (ISequenceStep sequenceStep in sequence.Steps)
            {
                AddSequenceStepTypeDatas(typeDatas, sequenceStep);
            }
        }

        private void AddSequenceStepTypeDatas(HashSet<ITypeData> typeDatas, ISequenceStep step)
        {
            if (null != step.Function)
            {
                AddFunctionTypeDatas(typeDatas, step.Function);
            }
            if (step.HasSubSteps)
            {
                foreach (ISequenceStep subStep in step.SubSteps)
                {
                    AddSequenceStepTypeDatas(typeDatas, subStep);
                }
            }
        }

        private void AddFunctionTypeDatas(HashSet<ITypeData> typeDatas, IFunctionData functionData)
        {
            if (null != functionData.ClassType)
            {
                typeDatas.Add(functionData.ClassType);
            }
            if (null != functionData.ReturnType && null != functionData.ReturnType.Type)
            {
                typeDatas.Add(functionData.ReturnType.Type);
            }
            AddArgumentTypeDatas(typeDatas, functionData.ParameterType);
        }

        private void RefreshUsedTypeDatas(ITypeDataCollection typeDatas, HashSet<ITypeData> usedTypeDatas)
        {
            for (int i = typeDatas.Count - 1; i >= 0; i--)
            {
                if (usedTypeDatas.Contains(typeDatas[i]))
                {
                    continue;
                }
                typeDatas.RemoveAt(i);
            }
            foreach (ITypeData usedTypeData in usedTypeDatas)
            {
                // 如果已存在add方法内部会进行判断
                typeDatas.Add(usedTypeData);
            }
        }

        private void RefreshUsedAssemblies(IAssemblyInfoCollection assemblies, HashSet<string> usedAssembly)
        {
            for (int i = assemblies.Count - 1; i >= 0; i--)
            {
                if (usedAssembly.Contains(assemblies[i].AssemblyName))
                {
                    continue;
                }
                assemblies.RemoveAt(i);
            }
            foreach (string assemblyName in usedAssembly)
            {
                IAssemblyInfo assemblyInfo = assemblies.FirstOrDefault(item => item.AssemblyName.Equals(assemblyName));
                if (null != assemblyInfo)
                {
                    continue;
                }
                assemblyInfo = _comInterfaceManager.GetAssemblyInfo(assemblyName);
                if (null == assemblyInfo)
                {
                    ILogService logService = TestflowRunner.GetInstance().LogService;
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0,
                        $"Unloaded assembly '{assemblyName}' used.");
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.TypeDataError,
                        i18N.GetFStr("InvalidAssemblyUsed", assemblyName));
                }
                assemblies.Add(assemblyInfo);
            }

        }

        #endregion

    }
}