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

        public static ISequenceGroup GetSequenceGroup(int sessionID, ITestProject testProject)
        {
            if (null == testProject)
            {
                throw new TestflowDataException(ModuleErrorCode.TestProjectDNE, "TestProject is null");
            }
            return testProject.SequenceGroups[sessionID]; 
        }
    }
}
