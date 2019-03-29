using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Common
{
    public static class CoreUtils
    {
        const string VarNameDelim = ".";

        public static string GetRuntimeVariableName(ITestProject testProject, ISequenceGroup sequenceGroup,
            ISequence sequence, IVariable variable)
        {
            StringBuilder runtimeVarName = new StringBuilder(50);
            runtimeVarName.Append(CoreConstants.TestProjectIndex).Append(VarNameDelim);
            if (null != sequenceGroup)
            {
                if (null != testProject)
                {
                    runtimeVarName.Append(testProject.SequenceGroups.IndexOf(sequenceGroup)).Append(VarNameDelim);
                }
                else
                {
                    runtimeVarName.Append(0).Append(VarNameDelim);
                }
                if (null != sequence)
                {
                    runtimeVarName.Append(sequenceGroup.Sequences.IndexOf(sequence)).Append(VarNameDelim);
                }
            }
            return runtimeVarName.Append(variable.Name).ToString();
        }

        public static string GetRuntimeVariableName(IVariable variable)
        {
            StringBuilder runtimeVariableName = new StringBuilder(50);
            Stack<ISequenceFlowContainer> stacks = new Stack<ISequenceFlowContainer>(8);
            ISequenceFlowContainer parent = variable;
            while (null != parent.Parent)
            {
                parent = parent.Parent;
                stacks.Push(parent);
            }
            parent = null;
            while (0 != stacks.Count)
            {
                ISequenceFlowContainer child = stacks.Pop();
                if (child is ITestProject)
                {
                    runtimeVariableName.Append(CoreConstants.TestProjectSessionId).Append(VarNameDelim);
                }
                else if (child is ISequenceGroup)
                {
                    if (null == parent)
                    {
                        runtimeVariableName.Append(CoreConstants.TestProjectSessionId).Append(VarNameDelim)
                            .Append(0).Append(VarNameDelim);
                    }
                    else
                    {
                        runtimeVariableName.Append(((ITestProject)parent).SequenceGroups.IndexOf((ISequenceGroup)child)).
                            Append(VarNameDelim);
                    }
                }
                else if (child is ISequence)
                {
                    runtimeVariableName.Append(((ISequenceGroup)parent).Sequences.IndexOf((ISequence)child)).
                            Append(VarNameDelim);
                }
                else if (parent is ISequence)
                {
                    runtimeVariableName.Append(((ISequence)parent).Steps.IndexOf((ISequenceStep)child)).
                            Append(VarNameDelim);
                }
                else
                {
                    runtimeVariableName.Append(((ISequenceStep)parent).SubSteps.IndexOf((ISequenceStep)child)).
                            Append(VarNameDelim);
                }
                parent = child;
            }
            return runtimeVariableName.Append(variable.Name).ToString();
        }

        public static string GetRuntimeVariableName(string variableName, ICallStack stack)
        {
            StringBuilder runtimeVarName = new StringBuilder(50);
            return runtimeVarName.Append(CoreConstants.TestProjectIndex).Append(VarNameDelim).Append(stack.SequenceGroupIndex)
                .Append(stack.SequenceIndex).Append(VarNameDelim).Append(string.Join(VarNameDelim, stack.StepStack))
                .Append(VarNameDelim).Append(variableName).ToString();

        }

        public static IVariable GetVariable(ITestProject testProject, string runtimeVariable)
        {
            string[] variableElement = runtimeVariable.Split(VarNameDelim.ToCharArray());
            if (2 == variableElement.Length)
            {
                return testProject.Variables.FirstOrDefault(item => item.Name.Equals(variableElement[1]));
            }
            else if (3 == variableElement.Length)
            {
                return testProject.SequenceGroups[int.Parse(variableElement[1])].Variables.FirstOrDefault(
                    item => item.Name.Equals(variableElement[2]));
            }
            else
            {
                return testProject.SequenceGroups[int.Parse(variableElement[1])].Sequences[int.Parse(variableElement[2])]
                        .Variables.FirstOrDefault(item => item.Name.Equals(variableElement[3]));
            }
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
    }
}