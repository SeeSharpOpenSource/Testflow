using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Logger;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.Serializer
{
    internal static class SequenceSerializer
    {
        #region 序列化

        public static void Serialize(string filePath, TestProject testProject)
        {
            VerifySequenceData(testProject);
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
            List<string> serialziedFileList = new List<string>(20);
            try
            {
                Type[] extraTypes = typeSet.ToArray();
                XmlSerializer testProjectSerializer = new XmlSerializer(testProjectType, extraTypes);
                InitSequenceGroupLocations(testProject, filePath);
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    testProjectSerializer.Serialize(fileStream, testProject);
                }
                serialziedFileList.Add(filePath);

                FillParameterDataToSequenceData(testProject);
                for (int i = 0; i < testProject.SequenceGroups.Count; i++)
                {
                    SequenceGroup sequenceGroup = testProject.SequenceGroups[i] as SequenceGroup;
                    if (!Common.Utility.IsValidPath(sequenceGroup.Info.SequenceGroupFile))
                    {
                        sequenceGroup.Info.SequenceGroupFile = Common.Utility.GetSequenceGroupPath(filePath, i);
                        sequenceGroup.Info.SequenceParamFile =
                            Common.Utility.GetParameterFilePath(sequenceGroup.Info.SequenceGroupFile);
                    }
                    else if (!Common.Utility.IsValidPath(sequenceGroup.Info.SequenceParamFile))
                    {
                        sequenceGroup.Info.SequenceParamFile =
                            Common.Utility.GetParameterFilePath(sequenceGroup.Info.SequenceGroupFile);
                    }
                    SequenceGroupParameter parameter = new SequenceGroupParameter();
                    parameter.Initialize(sequenceGroup);
                    FillParameterDataToSequenceData(sequenceGroup, parameter);
                    
                    XmlSerializer sequenceGroupSerializer = new XmlSerializer(typeof(SequenceGroup), extraTypes);
                    XmlSerializer sequenceParamSerializer = new XmlSerializer(typeof(SequenceGroupParameter), extraTypes);
                    using (FileStream fileStream = new FileStream(sequenceGroup.Info.SequenceGroupFile, FileMode.OpenOrCreate))
                    {
                        serialziedFileList.Add(sequenceGroup.Info.SequenceGroupFile);
                        sequenceGroupSerializer.Serialize(fileStream, sequenceGroup);
                    }
                    using (FileStream fileStream = new FileStream(sequenceGroup.Info.SequenceParamFile, FileMode.OpenOrCreate))
                    {
                        serialziedFileList.Add(sequenceGroup.Info.SequenceParamFile);
                        sequenceParamSerializer.Serialize(fileStream, parameter);
                    }
                }
            }
            catch (IOException ex)
            {
                RollBackFilesIfFailed(serialziedFileList);
                throw new TestflowRuntimeException(Constants.SerializeFailed, ex.Message, ex);
            }
            catch (ApplicationException)
            {
                RollBackFilesIfFailed(serialziedFileList);
                throw;
            }
        }

        public static void Serialize(string filePath, SequenceGroup sequenceGroup)
        {
            VerifySequenceData(sequenceGroup);
            HashSet<Type> typeSet = new HashSet<Type>();
            const string sequenceElementNameSpace = "Testflow.SequenceManager.SequenceElements";
            Type sequenceGroupType = typeof (SequenceGroup);
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
            sequenceGroup.Info.SequenceGroupFile = filePath;
            sequenceGroup.Info.SequenceParamFile = Common.Utility.GetParameterFilePath(filePath);

            SequenceGroupParameter parameter = new SequenceGroupParameter();
            parameter.Initialize(sequenceGroup);
            FillParameterDataToSequenceData(sequenceGroup, parameter);

            List<string> serialziedFileList = new List<string>(20);
            try
            {
                Type[] extraTypes = typeSet.ToArray();
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    serialziedFileList.Add(filePath);
                    XmlSerializer serializer = new XmlSerializer(sequenceGroupType, extraTypes);
                    serializer.Serialize(fileStream, sequenceGroup);
                }
                using (FileStream fileStream = new FileStream(sequenceGroup.Info.SequenceParamFile, FileMode.OpenOrCreate))
                {
                    serialziedFileList.Add(sequenceGroup.Info.SequenceParamFile);
                    XmlSerializer serializer = new XmlSerializer(typeof(SequenceGroupParameter), extraTypes);
                    serializer.Serialize(fileStream, sequenceGroup);
                }
            }
            catch (IOException ex)
            {
                RollBackFilesIfFailed(serialziedFileList);
                throw new TestflowRuntimeException(Constants.SerializeFailed, ex.Message, ex);
            }
            catch (ApplicationException)
            {
                RollBackFilesIfFailed(serialziedFileList);
                throw;
            }
        }

        private static void InitSequenceGroupLocations(TestProject testProject, string testProjectPath)
        {
            testProject.SequenceGroupLocations.Clear();
            ISequenceGroupCollection sequenceGroups = testProject.SequenceGroups;
            for (int i = 0; i < sequenceGroups.Count; i++)
            {
                ISequenceGroup sequenceGroup = sequenceGroups[i];
                if (!Common.Utility.IsValidPath(sequenceGroup.Info.SequenceGroupFile))
                {
                    sequenceGroup.Info.SequenceGroupFile = Common.Utility.GetSequenceGroupPath(testProjectPath, i);
                    sequenceGroup.Info.SequenceParamFile =
                        Common.Utility.GetParameterFilePath(sequenceGroup.Info.SequenceGroupFile);
                }
                else if (!Common.Utility.IsValidPath(sequenceGroup.Info.SequenceParamFile))
                {
                    sequenceGroup.Info.SequenceParamFile =
                        Common.Utility.GetParameterFilePath(sequenceGroup.Info.SequenceGroupFile);
                }
                SequenceGroupLocationInfo locationInfo = new SequenceGroupLocationInfo()
                {
                    Name = sequenceGroup.Name,
                    SequenceFilePath = sequenceGroup.Info.SequenceGroupFile,
                    ParameterFilePath = sequenceGroup.Info.SequenceParamFile
                };
                testProject.SequenceGroupLocations.Add(locationInfo);
            }
        }

        private static void FillParameterDataToSequenceData(ITestProject testProject)
        {
            FillParameterDataToSequenceData(testProject.SetUp, testProject.SetUpParameters);
            for (int i = 0; i < testProject.Variables.Count; i++)
            {
                testProject.VariableValues[i].Value = testProject.Variables[i].Value;
            }
            FillParameterDataToSequenceData(testProject.TearDown, testProject.TearDownParameters);
        }

        private static void FillParameterDataToSequenceData(ISequenceGroup sequenceGroup, ISequenceGroupParameter parameter)
        {
            for (int i = 0; i < sequenceGroup.Variables.Count; i++)
            {
                parameter.VariableValues[i].Value = sequenceGroup.Variables[i].Value;
            }
            FillParameterDataToSequenceData(sequenceGroup.SetUp, parameter.SetUpParameters);
            for (int i = 0; i < sequenceGroup.Sequences.Count; i++)
            {
                FillParameterDataToSequenceData(sequenceGroup.Sequences[i], parameter.SequenceParameters[i]);
            }
            FillParameterDataToSequenceData(sequenceGroup.TearDown, parameter.TearDownParameters);
        }

        private static void FillParameterDataToSequenceData(ISequence sequence, ISequenceParameter parameter)
        {
            for (int i = 0; i < sequence.Variables.Count; i++)
            {
                parameter.VariableValues[i].Value = sequence.Variables[i].Value;
            }

            for (int i = 0; i < sequence.Steps.Count; i++)
            {
                FillParameterDataToSequenceData(sequence.Steps[i], parameter.StepParameters[i]);
            }
        }

        private static void FillParameterDataToSequenceData(ISequenceStep sequenceStep, ISequenceStepParameter parameter)
        {
            if (sequenceStep.HasSubSteps)
            {
                for (int i = 0; i < sequenceStep.SubSteps.Count; i++)
                {
                    FillParameterDataToSequenceData(sequenceStep.SubSteps[i], parameter.SubStepParameters[i]);
                }
            }
            else
            {
                parameter.Instance = sequenceStep.Function.Instance;
                parameter.Return = sequenceStep.Function.Return;
                IParameterDataCollection parameterValues = sequenceStep.Function.Parameters;
                for (int i = 0; i < parameterValues.Count; i++)
                {
                    parameter.Parameters[i].ParameterType = parameterValues[i].ParameterType;
                    parameter.Parameters[i].Value = parameterValues[i].Value;
                }
            }
        }

        /// <summary>
        /// 序列化前预处理，包括更新TypeData和TypeIndex属性
        /// </summary>
        private static void VerifySequenceData(TestProject testProject)
        {
            testProject.TypeDatas.Clear();
            foreach (IVariable variable in testProject.Variables)
            {
                VerifyType(testProject.TypeDatas, variable as Variable);
            }
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                foreach (IArgument argument in sequenceGroup.Arguments)
                {
                    VerifyType(testProject.TypeDatas, argument as Argument);
                }
            }

            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                VerifySequenceData(sequenceGroup as SequenceGroup);
            }
        }

        private static void VerifySequenceData(SequenceGroup sequenceGroup)
        {
            foreach (IVariable variable in sequenceGroup.Variables)
            {
                VerifyType(sequenceGroup.TypeDatas, variable as Variable);
            }
            foreach (IArgument argument in sequenceGroup.Arguments)
            {
                VerifyType(sequenceGroup.TypeDatas, argument as Argument);
            }
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                foreach (IVariable variable in sequence.Variables)
                {
                    VerifyType(sequenceGroup.TypeDatas, variable as Variable);
                }
                foreach (ISequenceStep sequenceStep in sequence.Steps)
                {
                    VerifyType(sequenceGroup.TypeDatas, sequenceStep);
                }
            }
        }

        private static void RollBackFilesIfFailed(IList<string> fileList)
        {
            foreach (string file in fileList)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException ex)
                    {
                        LogService logService = LogService.GetLogService();
                        logService.Print(LogLevel.Warn, CommonConst.PlatformLogSession, 0, ex, "Roll back file failed.");
                    }
                }
            }
        }

        private static void VerifyType(ITypeDataCollection typeDatas, ISequenceStep step)
        {
            if (step.HasSubSteps)
            {
                foreach (ISequenceStep subStep in step.SubSteps)
                {
                    VerifyType(typeDatas, subStep);
                }
            }
            else
            {
                VerifyType(typeDatas, step.Function as FunctionData);
            }
        }

        private static void VerifyType(ITypeDataCollection typeDatas, Variable variable)
        {
            if (null == variable.Type || VariableType.Undefined == variable.VariableType)
            {
                variable.TypeIndex = Constants.UnverifiedTypeIndex;
                return;
            }
            int index = typeDatas.IndexOf(variable.Type);
            if (-1 == index)
            {
                index = typeDatas.Count;
                typeDatas.Add(variable.Type);
            }
            variable.TypeIndex = index;
        }

        private static void VerifyType(ITypeDataCollection typeDatas, Argument argument)
        {
            if (null == argument.Type || VariableType.Undefined == argument.VariableType)
            {
                argument.TypeIndex = Constants.UnverifiedTypeIndex;
                return;
            }
            int index = typeDatas.IndexOf(argument.Type);
            if (-1 == index)
            {
                index = typeDatas.Count;
                typeDatas.Add(argument.Type);
            }
            argument.TypeIndex = index;
        }

        private static void VerifyType(ITypeDataCollection typeDatas, FunctionData functionData)
        {
            if (null == functionData.ClassType)
            {
                functionData.ClassTypeIndex = Constants.UnverifiedTypeIndex;
                return;
            }
            int index = typeDatas.IndexOf(functionData.ClassType);
            if (-1 == index)
            {
                index = typeDatas.Count;
                typeDatas.Add(functionData.ClassType);
            }

            functionData.ClassTypeIndex = index;
            foreach (IArgument argument in functionData.ParameterType)
            {
                VerifyType(typeDatas, argument as Argument);
            }

            VerifyType(typeDatas, functionData.ReturnType as Argument);
        }

        #endregion
    }
}