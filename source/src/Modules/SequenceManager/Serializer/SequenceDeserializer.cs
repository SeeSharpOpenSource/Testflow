using System.IO;
using Newtonsoft.Json;
using Testflow.Usr;
using Testflow.Data.Sequence;
using Testflow.Logger;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.Serializer
{
    internal static class SequenceDeserializer
    {
        #region 反序列化

        public static TestProject LoadTestProject(string filePath, bool forceLoad, IModuleConfigData envInfo)
        {
            filePath = ModuleUtils.GetAbsolutePath(filePath, Directory.GetCurrentDirectory());

            if (!filePath.EndsWith($".{CommonConst.TestGroupFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(ModuleErrorCode.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            TestProject testProject = null;
            testProject = XmlReaderHelper.ReadTestProject(filePath);
            
            // 需要单独配置Setup和TearDown的索引号
            testProject.SetUp.Index = CommonConst.SetupIndex;
            testProject.TearDown.Index = CommonConst.TeardownIndex;

            CheckModelVersion(testProject.ModelVersion, envInfo);
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                CheckModelVersion(sequenceGroup.Info.Version, envInfo);
            }
            foreach (SequenceGroupLocationInfo sequenceGroupLocation in testProject.SequenceGroupLocations)
            {
                SequenceGroup sequenceGroup = null;

                string sequenceGroupFile = ModuleUtils.GetAbsolutePath(sequenceGroupLocation.SequenceFilePath, filePath);
                if (File.Exists(sequenceGroupFile))
                {
                    sequenceGroup = LoadSequenceGroup(sequenceGroupFile, forceLoad, envInfo);
                    sequenceGroup.Parent = testProject;
                }
                else
                {
                    ILogService logService = TestflowRunner.GetInstance().LogService;
                    logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0, "Sequence group file not exist.");
                    sequenceGroup = new SequenceGroup();
                    sequenceGroup.Initialize(testProject);
                    sequenceGroup.Info.SequenceGroupFile = sequenceGroupFile;
                    sequenceGroup.Available = false;
                }
                testProject.SequenceGroups.Add(sequenceGroup);
            }
            ValidateTypeDatas(testProject);

            return testProject;
        }

        public static SequenceGroup LoadSequenceGroup(string filePath, bool forceLoad, IModuleConfigData envInfo)
        {
            filePath = ModuleUtils.GetAbsolutePath(filePath, Directory.GetCurrentDirectory());

            if (!filePath.EndsWith($".{CommonConst.SequenceFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            SequenceGroup sequenceGroup = XmlReaderHelper.ReadSequenceGroup(filePath);

            // 需要单独配置Setup和TearDown的索引号
            sequenceGroup.SetUp.Index = CommonConst.SetupIndex;
            sequenceGroup.TearDown.Index = CommonConst.TeardownIndex;

            string sequenceParamFile = ModuleUtils.GetAbsolutePath(sequenceGroup.Info.SequenceParamFile, filePath);
            SequenceGroupParameter parameter =
                XmlReaderHelper.ReadSequenceGroupParameter(sequenceParamFile);
            if (!forceLoad && !sequenceGroup.Info.Hash.Equals(parameter.Info.Hash))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.UnmatchedFileHash, i18N.GetStr("UnmatchedHash"));
            }
            sequenceGroup.Parameters = parameter;
            SetParameterToSequenceData(sequenceGroup, parameter);
            ValidateTypeDatas(sequenceGroup);
            return sequenceGroup;
        }

        public static void LoadParameter(SequenceGroup sequenceGroup, string filePath, bool forceLoad)
        {
            filePath = ModuleUtils.GetAbsolutePath(filePath, Directory.GetCurrentDirectory());
            SequenceGroupParameter parameter = XmlReaderHelper.ReadSequenceGroupParameter(filePath);
            if (!forceLoad && !sequenceGroup.Info.Hash.Equals(parameter.Info.Hash))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.UnmatchedFileHash, i18N.GetStr("UnmatchedHash"));
            }
            sequenceGroup.Parameters = parameter;
            SetParameterToSequenceData(sequenceGroup, parameter);
            ValidateTypeDatas(sequenceGroup);
        }

        public static TestProject LoadTestProjectFromJson(string jsonStr)
        {
            SequenceJsonConvertor convertor = new SequenceJsonConvertor();
            TestProject testProject = JsonConvert.DeserializeObject<TestProject>(jsonStr, convertor);
            ValidateTypeDatas(testProject);
            ModuleUtils.ValidateParent(testProject);
            return testProject;
        }
        
        public static SequenceGroup LoadSequenceGroupFromJson(string jsonStr)
        {
            SequenceJsonConvertor convertor = new SequenceJsonConvertor();
            SequenceGroup sequenceGroup = JsonConvert.DeserializeObject<SequenceGroup>(jsonStr, convertor);
            ValidateTypeDatas(sequenceGroup);
            ModuleUtils.ValidateParent(sequenceGroup, null);
            return sequenceGroup;
        }

        #endregion

        private static void SetParameterToSequenceData(ISequenceGroup sequenecGroup, ISequenceGroupParameter parameter)
        {
            for (int i = 0; i < sequenecGroup.Variables.Count; i++)
            {
                IVariable variable = sequenecGroup.Variables[i];
                if (!variable.Name.Equals(parameter.VariableValues[i].Value))
                {
                    ILogService logService = TestflowRunner.GetInstance().LogService;
                    logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0, 
                        $"Variable{variable.Name} {sequenecGroup.Name} value in parameter data is invalid.");
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.UnmatchedParameter, i18N.GetStr("UnmatchedData"));
                }
                variable.Value = parameter.VariableValues[i].Value;
            }

            SetParameterToSequenceData(sequenecGroup.SetUp, parameter.SetUpParameters);
            for (int i = 0; i < sequenecGroup.Sequences.Count; i++)
            {
                SetParameterToSequenceData(sequenecGroup.Sequences[i], parameter.SequenceParameters[i]);
            }
            SetParameterToSequenceData(sequenecGroup.TearDown, parameter.TearDownParameters);
        }

        private static void SetParameterToSequenceData(ISequence sequenece, ISequenceParameter parameter)
        {
            for (int i = 0; i < sequenece.Variables.Count; i++)
            {
                IVariable variable = sequenece.Variables[i];
                if (!variable.Name.Equals(parameter.VariableValues[i].Name))
                {
                    ILogService logService = TestflowRunner.GetInstance().LogService;
                    logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0,
                        $"Variable{variable.Name} {sequenece.Name} value in parameter data is invalid.");
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(ModuleErrorCode.UnmatchedParameter, i18N.GetStr("UnmatchedData"));
                }
                variable.Value = parameter.VariableValues[i].Value;
            }

            for (int i = 0; i < sequenece.Steps.Count; i++)
            {
                SetParameterToSequenceData(sequenece.Steps[i], parameter.StepParameters[i]);
            }
        }

        private static void SetParameterToSequenceData(ISequenceStep sequenecStep, ISequenceStepParameter parameter)
        {
            if (sequenecStep.HasSubSteps)
            {
                for (int i = 0; i < sequenecStep.SubSteps.Count; i++)
                {
                    SetParameterToSequenceData(sequenecStep.SubSteps[i], parameter.SubStepParameters[i]);
                }
            }
            else if (null != sequenecStep.Function)
            {
                sequenecStep.Function.Parameters = parameter.Parameters;
                sequenecStep.Function.Instance = parameter.Instance;
                sequenecStep.Function.Return = parameter.Return;
            }
        }

        private static void ValidateTypeDatas(TestProject testProject)
        {
            ValidateTypeDatas(testProject.Variables, testProject.TypeDatas);
            ValidateTypeDatas(testProject.SetUp, testProject.TypeDatas);
            ValidateTypeDatas(testProject.TearDown, testProject.TypeDatas);
        }

        private static void ValidateTypeDatas(ISequenceGroup sequenceGroup)
        {
            ValidateTypeDatas(sequenceGroup.Arguments, sequenceGroup.TypeDatas);
            ValidateTypeDatas(sequenceGroup.Variables, sequenceGroup.TypeDatas);
            ValidateTypeDatas(sequenceGroup.SetUp, sequenceGroup.TypeDatas);
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                ValidateTypeDatas(sequence, sequenceGroup.TypeDatas);
            }
            ValidateTypeDatas(sequenceGroup.TearDown, sequenceGroup.TypeDatas);
        }

        private static void ValidateTypeDatas(ISequence sequence, ITypeDataCollection typeDatas)
        {
            ValidateTypeDatas(sequence.Variables, typeDatas);
            foreach (ISequenceStep sequenceStep in sequence.Steps)
            {
                ValidateTypeDatas(sequenceStep, typeDatas);
            }
        }

        private static void ValidateTypeDatas(ISequenceStep sequenceStep, ITypeDataCollection typeDatas)
        {
            if (sequenceStep.HasSubSteps)
            {
                foreach (ISequenceStep subStep in sequenceStep.SubSteps)
                {
                    ValidateTypeDatas(subStep, typeDatas);
                }
            }
            else
            {
                FunctionData functionData = sequenceStep.Function as FunctionData;
                if (null == functionData)
                {
                    return;
                }
                if (Constants.UnverifiedTypeIndex != functionData.ClassTypeIndex)
                {
                    functionData.ClassType = typeDatas[functionData.ClassTypeIndex];
                }
                Argument returnType = functionData.ReturnType as Argument;
                if (null != returnType && Constants.UnverifiedTypeIndex != returnType.TypeIndex)
                {
                    returnType.Type = typeDatas[returnType.TypeIndex];
                }
                foreach (IArgument rawArgument in functionData.ParameterType)
                {
                    Argument argument = rawArgument as Argument;
                    if (Constants.UnverifiedTypeIndex != argument.TypeIndex)
                    {
                        argument.Type = typeDatas[argument.TypeIndex];
                    }
                }
            }
        }

        private static void ValidateTypeDatas(IArgumentCollection variables, ITypeDataCollection typeDatas)
        {
            foreach (IArgument rawArguments in variables)
            {
                Argument argument = rawArguments as Argument;
                if (Constants.UnverifiedTypeIndex != argument.TypeIndex)
                {
                    argument.Type = typeDatas[argument.TypeIndex];
                }
            }
        }

        private static void ValidateTypeDatas(IVariableCollection variables, ITypeDataCollection typeDatas)
        {
            foreach (IVariable rawVariable in variables)
            {
                Variable variable = rawVariable as Variable;
                if (Constants.UnverifiedTypeIndex != variable.TypeIndex)
                {
                    variable.Type = typeDatas[variable.TypeIndex];
                }
            }
        }

        private static void CheckModelVersion(string version, IModuleConfigData envInfo)
        {
            I18N i18N = I18N.GetInstance(Constants.I18nName);
            const char versionDelim = '.';
            string[] versionElem = version.Split(versionDelim);
            string envVersion = envInfo.GetProperty<string>(Constants.VersionName);
            string[] envVersionElem = envVersion.Split(versionDelim);
            int versionElemSize = versionElem.Length <= envVersionElem.Length
                ? versionElem.Length
                : envVersionElem.Length;
            for (int i = 0; i < versionElemSize; i++)
            {
                int versionId = int.Parse(versionElem[i]);
                int envVersionId = int.Parse(envVersionElem[i]);
                if (versionId > envVersionId)
                {
                    throw new TestflowDataException(ModuleErrorCode.InvalidModelVersion, 
                        i18N.GetStr("InvalidModelVersion"));
                }
            }
            for (int i = envVersionElem.Length; i < versionElem.Length; i++)
            {
                if (int.Parse(versionElem[i]) > 0)
                {
                    throw new TestflowDataException(ModuleErrorCode.InvalidModelVersion,
                        i18N.GetStr("InvalidModelVersion"));
                }
            }
        }
    }
}