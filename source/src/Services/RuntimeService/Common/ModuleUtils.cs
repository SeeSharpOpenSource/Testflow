using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Usr;

namespace Testflow.RuntimeService
{
    internal static class ModuleUtils
    {
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

        
        internal static void EngineStartThread(ISequenceFlowContainer sequence)
        {
            Thread engineThread = new Thread(() => ModuleUtils.EngineStart(sequence))
            {
                IsBackground = true
            };
            engineThread.Start();
        }

        //在这里为什么不做个变量去保存引擎的运行情况？
        //因为事件触发:如果引擎失败，用户会根据相应的情况去添加事件
        //在此就无需自己在这里返回exception
        //而且还有一点。。无法返回exception给主线程。。除非加个锁？
        internal static void EngineStart(ISequenceFlowContainer sequence)
        {
                TestflowRunner.GetInstance().EngineController.Start();
        }
    }
}
