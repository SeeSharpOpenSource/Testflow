using System.Collections.Generic;
using System.Linq;
using Testflow.Common;
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
        private IComInterfaceManager _comInterfaceManager;

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
            
        }
        
        /// <summary>
        /// 初始化变量和参数的类型数据
        /// </summary>
        public void VerifyVariableTypes(ISequenceGroup sequenceGroup)
        {

        }

        /// <summary>
        /// 初始化变量和参数的类型数据
        /// </summary>
        public void VerifyVariableTypes(ISequenceFlowContainer flowContainer, IVariable variable)
        {

        }
        
        /// <summary>
        /// 初始化变量和参数的类型数据
        /// </summary>
        public void VerifyVariableTypes(ISequenceFlowContainer flowContainer, IArgument argument)
        {

        }

        public void RefreshUsedAssemblyAndType(ITestProject testProject)
        {
            HashSet<ITypeData> typeDatas = new HashSet<ITypeData>();
            AddVariableTypeDatas(typeDatas, testProject.Variables);
            AddSequenceTypeDatas(typeDatas, testProject.SetUp);
            AddSequenceTypeDatas(typeDatas, testProject.TearDown);

            RefreshUsedTypeDatas(testProject.TypeDatas, typeDatas);

            HashSet<string> assemblyNames = new HashSet<string>();
            RefreshUsedAssemblies(testProject.Assemblies, assemblyNames);
            foreach (ITypeData typeData in typeDatas)
            {
                assemblyNames.Add(typeData.AssemblyName);
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
            RefreshUsedAssemblies(sequenceGroup.Assemblies, assemblyNames);
            foreach (ITypeData typeData in typeDatas)
            {
                assemblyNames.Add(typeData.AssemblyName);
            }
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
            if (step.HasSubSteps)
            {
                foreach (ISequenceStep subStep in step.SubSteps)
                {
                    AddSequenceStepTypeDatas(typeDatas, subStep);
                }
            }
            else if (null != step.Function)
            {
                AddFunctionTypeDatas(typeDatas, step.Function);
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
            for (int i = typeDatas.Count - 1; i <= 0; i++)
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
            for (int i = assemblies.Count - 1; i <= 0; i++)
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
                    return;
                }
                assemblyInfo = _comInterfaceManager.GetComInterfaceByName(assemblyName).Assembly;
                if (null == assemblyInfo)
                {
                    LogService logService = LogService.GetLogService();
                    logService.Print(LogLevel.Error, CommonConst.PlatformLogSession, 0, 
                        $"Unloaded assembly '{assemblyName}' used.");
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(SequenceManagerErrorCode.TypeDataError, 
                        i18N.GetFStr("InvalidAssemblyUsed", assemblyName));
                }
                assemblies.Add(assemblyInfo);
            }
            
        }
    }
}