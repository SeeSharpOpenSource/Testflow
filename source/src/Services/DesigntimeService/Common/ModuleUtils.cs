using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Usr;

namespace Testflow.DesigntimeService.Common
{
    internal static class ModuleUtils
    {
        //public static int GetSessionId(ITestProject testProject, ISequenceGroup sequenceGroup)
        //{
        //    if (null == testProject)
        //    {
        //        throw new TestflowDataException(ModuleErrorCode.TestProjectDNE, "TestProject is null");
        //    }
        //    int index = testProject.SequenceGroups.IndexOf(sequenceGroup);
        //    if (index < 0)
        //    {
        //        throw new TestflowDataException(ModuleErrorCode.SequenceGroupDNE, "SequenceGroup does not exist");
        //    }
        //    return index;
        //}

        internal static ISequenceGroup GetSequenceGroup(int sessionId, ITestProject testProject)
        {
            return testProject.SequenceGroups[sessionId]; 
        }

        internal static IVariable FindVariableInSequenceGroup(string varName, ISequenceGroup sequenceGroup)
        {
            foreach (IVariable variable in sequenceGroup.Variables)
            {
                if (variable.Name.Equals(varName))
                {
                    return variable;
                }
            }
            foreach (IVariable variable in sequenceGroup.SetUp.Variables)
            {
                if (variable.Name.Equals(varName))
                {
                    return variable;
                }
            }
            foreach (IVariable variable in sequenceGroup.TearDown.Variables)
            {
                if (variable.Name.Equals(varName))
                {
                    return variable;
                }
            }
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                foreach (IVariable variable in sequence.Variables)
                {
                    if (variable.Name.Equals(varName))
                    {
                        return variable;
                    }
                }
            }
            return null;
        }

        //todo I18n
        internal static IParameterData FindParameterByName(string paramName, ISequenceStep sequenceStep)
        {
            IArgument argument = sequenceStep.Function.ParameterType.FirstOrDefault(item => item.Name.Equals(paramName));
            if(argument == null)
            {
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, $"Parameter with {paramName} not found");
            }

            int index = sequenceStep.Function.ParameterType.IndexOf(argument);
            return sequenceStep.Function.Parameters[index];
        }

        public static void Rename(ISequenceFlowContainer target, string newName)
        {
            
        }
    }
}
