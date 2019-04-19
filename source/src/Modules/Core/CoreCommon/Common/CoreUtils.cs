using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Testflow.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Utility.I18nUtil;

namespace Testflow.CoreCommon.Common
{
    public static class CoreUtils
    {
        const string VarNameDelim = "$";
        const string PropertyDelim = ".";

        public static string GetRuntimeVariableName(int session, IVariable variable)
        {
            StringBuilder runtimeVariableName = new StringBuilder(50);
            runtimeVariableName.Append(session).Append(VarNameDelim);
            ISequence parent = variable.Parent as ISequence;
            if (parent != null)
            {
                runtimeVariableName.Append(parent.Index).Append(VarNameDelim);
            }
            return runtimeVariableName.Append(variable.Name).ToString();
        }

        public static string GetRuntimeVariableName(string variableName, ICallStack stack)
        {
            StringBuilder runtimeVarName = new StringBuilder(50);
            return runtimeVarName.Append(stack.SessionIndex)
                .Append(stack.Session).Append(VarNameDelim).Append(string.Join(VarNameDelim, stack.StepStack))
                .Append(VarNameDelim).Append(variableName).ToString();
        }

        public static IVariable GetVariable(ISequenceFlowContainer sequenceData, string runtimeVariable)
        {
            IVariable variable;
            if (sequenceData is ITestProject)
            {
                variable = GetVariable((ITestProject)sequenceData, runtimeVariable);
            }
            else if (sequenceData is ISequenceGroup)
            {
                variable = GetVariable((ISequenceGroup) sequenceData, runtimeVariable);
            }
            else
            {
                variable = GetVariable((ISequence) sequenceData, runtimeVariable);
            }
            return variable;
        }

        public static IVariable GetVariable(ITestProject testProject, string runtimeVariable)
        {
            string[] variableElement = runtimeVariable.Split(VarNameDelim.ToCharArray());
            IVariableCollection varCollection = null;
            if (2 == variableElement.Length)
            {
                varCollection = testProject.Variables;
            }
            else if (3 == variableElement.Length)
            {
                varCollection = variableElement[1].Equals(CommonConst.SetupIndex.ToString())? 
                    testProject.SetUp.Variables : testProject.TearDown.Variables;
            }
            return varCollection.FirstOrDefault(item => item.Name.Equals(variableElement[variableElement.Length - 1]));
        }

        public static string GetVariableNameRegex(ISequenceFlowContainer sequenceData, int session)
        {
            string regex;
            if (sequenceData is ITestProject || sequenceData is ISequenceGroup)
            {
                const string rootRegexFormat = @"^{0}\{1}[^\{1}]+";
                regex = string.Format(rootRegexFormat, session, VarNameDelim);
            }
            else
            {
                ISequence sequence = (ISequence)sequenceData;
                const string sequenceVarRegexFormat = @"^{0}\{2}{1}\{2}[^\{2}]+";
                regex = string.Format(sequenceVarRegexFormat, session, sequence.Index, VarNameDelim);
            }
            return regex;
        }

        public static IVariable GetVariable(ISequenceGroup sequenceGroup, string runtimeVariable)
        {
            string[] variableElement = runtimeVariable.Split(VarNameDelim.ToCharArray());
            IVariableCollection varCollection = null;
            if (2 == variableElement.Length)
            {
                varCollection = sequenceGroup.Variables;
            }
            else if (3 == variableElement.Length)
            {
                int sequenceIndex = int.Parse(variableElement[1]);
                if (sequenceIndex == CommonConst.SetupIndex)
                {
                    varCollection = sequenceGroup.SetUp.Variables;
                }
                else if (sequenceIndex == CommonConst.TeardownIndex)
                {
                    varCollection = sequenceGroup.TearDown.Variables;
                }
                else
                {
                    varCollection = sequenceGroup.Sequences[sequenceIndex].Variables;
                }
            }
            return varCollection.FirstOrDefault(item => item.Name.Equals(variableElement[variableElement.Length - 1]));
        }

        public static IVariable GetVariable(ISequence sequence, string runtimeVariable)
        {
            string[] variableElement = runtimeVariable.Split(VarNameDelim.ToCharArray());
            IVariableCollection varCollection = sequence.Variables;
            return varCollection.FirstOrDefault(item => item.Name.Equals(variableElement[variableElement.Length - 1]));
        }

        public static void SetMessageValue(SerializationInfo info, object target, Type type)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PropertyInfo propertyInfo = type.GetProperty(enumerator.Name,
                    BindingFlags.Instance | BindingFlags.Public);
                propertyInfo.SetValue(target, info.GetValue(enumerator.Name, propertyInfo.PropertyType));
            }
        }

        public static IDictionary<IVariable, string> GetVariableValues(DebugWatchData debugWatchData, ISequenceFlowContainer sequenceData)
        {
            Dictionary<IVariable, string> variableValues = new Dictionary<IVariable, string>(debugWatchData.Count);
            for (int i = 0; i < debugWatchData.Count; i++)
            {
                IVariable variable = GetVariable(sequenceData, debugWatchData.Names[i]);
                variableValues.Add(variable, debugWatchData.Values[i]);
            }
            return variableValues;
        }

        public static bool IsValidVaraible(string variable)
        {
            return !string.IsNullOrWhiteSpace(variable);
        }
    }
}