using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Testflow.Common;
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
            HashSet<Type> typeSet = new HashSet<Type>();
            const string sequenceElementNameSpace = "Testflow.SequenceManager.SequenceElements";
            Type testProjectType = typeof (TestProject);
            Type[] assemblyTypes = Assembly.GetAssembly(testProjectType).GetTypes();
            foreach (Type typeObj in assemblyTypes)
            {
                if (sequenceElementNameSpace.Equals(typeObj.Namespace))
                {
                    typeSet.Add(typeObj);
                }
            }
            typeSet.Remove(testProjectType);
            if (!filePath.EndsWith($".{CommonConst.TestGroupFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowRuntimeException(SequenceManagerErrorCode.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            try
            {
                TestProject testProject = null;
                Type[] extraTypes = typeSet.ToArray();
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(testProjectType, extraTypes);
                    testProject = serializer.Deserialize(fileStream) as TestProject;
                    CheckModelVersion(testProject.ModelVersion, envInfo);
                    foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
                    {
                        CheckModelVersion(sequenceGroup.Info.Version, envInfo);
                    }
                }

                foreach (SequenceGroupLocationInfo sequenceGroupLocation in testProject.SequenceGroupLocations)
                {
                    SequenceGroup sequenceGroup = null;
                    
                    if (File.Exists(sequenceGroupLocation.SequenceFilePath))
                    {
                        using (FileStream stream = new FileStream(sequenceGroupLocation.SequenceFilePath, FileMode.Open))
                        {
                            XmlSerializer sequenceGroupSerializer = new XmlSerializer(typeof(SequenceGroup), extraTypes);
                            sequenceGroup = sequenceGroupSerializer.Deserialize(stream) as SequenceGroup;
                            sequenceGroup.Parent = testProject;
                        }
                        SequenceGroupParameter parameter = null;
                        using (FileStream fileStream = new FileStream(sequenceGroup.Info.SequenceParamFile, FileMode.OpenOrCreate))
                        {
                            XmlSerializer sequenceParamSerializer = new XmlSerializer(typeof(SequenceGroupParameter), extraTypes);
                            parameter = sequenceParamSerializer.Deserialize(fileStream) as SequenceGroupParameter;
                        }
                        if (!forceLoad && !sequenceGroup.Info.Hash.Equals(parameter.Info.Hash))
                        {
                            I18N i18N = I18N.GetInstance(Constants.I18nName);
                            throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedFileHash, i18N.GetStr("UnmatchedHash"));
                        }
                        sequenceGroup.Parameters = parameter;
                        SetParameterToSequenceData(sequenceGroup, parameter);
                    }
                    else
                    {
                        LogService logService = LogService.GetLogService();
                        logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0, "Sequence group file not exist.");
                        sequenceGroup = new SequenceGroup();
                        sequenceGroup.Initialize(testProject);
                        sequenceGroup.Available = false;
                    }
                    testProject.SequenceGroups.Add(sequenceGroup);
                }
                
                return testProject;
            }
            catch (IOException ex)
            {
                LogService logService = LogService.GetLogService();
                logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0, ex, "Load test project failed.");
                throw new TestflowRuntimeException(SequenceManagerErrorCode.SerializeFailed, ex.Message, ex);
            }
        }

        public static SequenceGroup LoadSequenceGroup(string filePath, bool forceLoad, IModuleConfigData envInfo)
        {
            HashSet<Type> typeSet = new HashSet<Type>();
            const string sequenceElementNameSpace = "Testflow.SequenceManager.SequenceElements";
            Type sequenceGroupType = typeof (TestProject);
            Type[] assemblyTypes = Assembly.GetAssembly(sequenceGroupType).GetTypes();
            foreach (Type typeObj in assemblyTypes)
            {
                if (sequenceElementNameSpace.Equals(typeObj.Namespace))
                {
                    typeSet.Add(typeObj);
                }
            }
            typeSet.Remove(typeof (TestProject));
            typeSet.Remove(sequenceGroupType);
            if (!filePath.EndsWith($".{CommonConst.SequenceFileExtension}"))
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(SequenceManagerErrorCode.InvalidFileType, i18N.GetStr("InvalidFileType"));
            }
            try
            {
                Type[] extraTypes = typeSet.ToArray();
                SequenceGroup sequenceGroup = null;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(sequenceGroupType, extraTypes);
                    sequenceGroup = serializer.Deserialize(fileStream) as SequenceGroup;
                }
                SequenceGroupParameter parameter = null;
                using (FileStream fileStream = new FileStream(sequenceGroup.Info.SequenceParamFile, FileMode.OpenOrCreate))
                {
                    XmlSerializer sequenceParamSerializer = new XmlSerializer(typeof(SequenceGroupParameter), extraTypes);
                    parameter = sequenceParamSerializer.Deserialize(fileStream) as SequenceGroupParameter;
                }
                if (!forceLoad && !sequenceGroup.Info.Hash.Equals(parameter.Info.Hash))
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedFileHash, i18N.GetStr("UnmatchedHash"));
                }
                sequenceGroup.Parameters = parameter;
                SetParameterToSequenceData(sequenceGroup, parameter);
                return sequenceGroup;
            }
            catch (IOException ex)
            {
                throw new TestflowRuntimeException(SequenceManagerErrorCode.SerializeFailed, ex.Message, ex);
            }
        }

        #endregion

        private static void SetParameterToSequenceData(ISequenceGroup sequenecGroup, ISequenceGroupParameter parameter)
        {
            for (int i = 0; i < sequenecGroup.Variables.Count; i++)
            {
                IVariable variable = sequenecGroup.Variables[i];
                if (!variable.Name.Equals(parameter.VariableValues[i].Value))
                {
                    LogService logService = LogService.GetLogService();
                    logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0, 
                        $"Variable{variable.Name} {sequenecGroup.Name} value in parameter data is invalid.");
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedParameter, i18N.GetStr("UnmatchedData"));
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
                if (!variable.Name.Equals(parameter.VariableValues[i].Value))
                {
                    LogService logService = LogService.GetLogService();
                    logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0,
                        $"Variable{variable.Name} {sequenece.Name} value in parameter data is invalid.");
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedParameter, i18N.GetStr("UnmatchedData"));
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
            else
            {
                sequenecStep.Function.Parameters = parameter.Parameters;
                sequenecStep.Function.Instance = parameter.Instance;
                sequenecStep.Function.Return = parameter.Return;
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
                    throw new TestflowDataException(SequenceManagerErrorCode.InvalidModelVersion, 
                        i18N.GetStr("InvalidModelVersion"));
                }
            }
            for (int i = envVersionElem.Length; i < versionElem.Length; i++)
            {
                if (int.Parse(versionElem[i]) > 0)
                {
                    throw new TestflowDataException(SequenceManagerErrorCode.InvalidModelVersion,
                        i18N.GetStr("InvalidModelVersion"));
                }
            }
        }
    }
}