using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Usr;

namespace Testflow.RuntimeService
{
    internal static class ModuleUtils
    {
        private static TestflowException Exception = null;

        internal static int GetSessionId(ITestProject testProject, ISequenceGroup sequenceGroup)
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

        internal static int GetSessionId(ITestProject testProject, string sequenceGroupName)
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

        //在新的thread里运行engine
        //返回抓到的错误信息
        //如果没有错误，返回null
        //这个可能不需要返回，因为外部用try, catch就能抓到这个错误？
        internal static Exception EngineStartThread(ISequenceFlowContainer sequence)
        {
            Thread engineThread = new Thread(() => ModuleUtils.EngineStart(sequence))
            {
                IsBackground = true
            };
            engineThread.Start();
            return Exception;
        }

        internal static void EngineStart(ISequenceFlowContainer sequence)
        {
            try
            {
                TestflowRunner.GetInstance().EngineController.SetSequenceData(sequence);
                TestflowRunner.GetInstance().EngineController.Start();
            }
            catch (TestflowException ex)
            {
                Exception = ex;
            }
        }
    }
}
