using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;

namespace Testflow.CoreCommon.Common
{
    public static class ModuleUtils
    {
        const string VarNameDelim = ".";
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
                    runtimeVariableName.Append(Constants.TestProjectSessionId).Append(VarNameDelim);
                }
                else if (child is ISequenceGroup)
                {
                    if (null == parent)
                    {
                        runtimeVariableName.Append(Constants.TestProjectSessionId).Append(VarNameDelim)
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
            return runtimeVarName.Append(Constants.TestProjectIndex).Append(VarNameDelim).Append(stack.SequenceGroupIndex)
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
    }
}