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

namespace Testflow.CoreCommon.Common
{
    public static class CoreUtils
    {
        const string VarNameDelim = ".";

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
                .Append(stack.SequenceIndex).Append(VarNameDelim).Append(string.Join(VarNameDelim, stack.StepStack))
                .Append(VarNameDelim).Append(variableName).ToString();
        }

        public static IVariable GetVariable(ISequenceFlowContainer sequenceData, string runtimeVariable)
        {
            return sequenceData is ISequenceGroup
                ? GetVariable((ISequenceGroup) sequenceData, runtimeVariable)
                : GetVariable((ITestProject) sequenceData, runtimeVariable);
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
    }
}