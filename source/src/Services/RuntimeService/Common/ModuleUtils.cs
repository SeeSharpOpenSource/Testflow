using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Usr;

namespace Testflow.RuntimeService.Common
{
    internal static class ModuleUtils
    {
        public static int GetSessionId(ITestProject testProject, ISequenceGroup sequenceGroup)
        {
            if (null == testProject)
            {
                throw new TestflowDataException(ModuleErrorCode.TestProjectDNE, "TestProject is null");
            }
            int index = testProject.SequenceGroups.IndexOf(sequenceGroup);
            if (index < 0)
            {
                throw new TestflowDataException(ModuleErrorCode.SequenceGroupDNE, "SequenceGroup does not exist");
            }
            return index;
        }

        public static int GetSessionId(ITestProject testProject, string sequenceGroupName)
        {
            if (null == testProject)
            {
                return 0;
            }
            ISequenceGroup sequenceGroup =  testProject.SequenceGroups.FirstOrDefault(item => item.Name.Equals(sequenceGroupName));
            if (sequenceGroup == null)
            {
                throw new TestflowDataException(ModuleErrorCode.SequenceGroupDNE, "SequenceGroup does not exist");
            }
            return testProject.SequenceGroups.IndexOf(sequenceGroup);
            
        }

        //public static ISequenceGroup GetSequenceGroup(int sessionID, ITestProject testProject)
        //{
        //    if (null == testProject)
        //    {
        //        return null;
        //    }
        //    return testProject.SequenceGroups.ElementAt(sessionID);
        //}
    }
}
