using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.EngineCore.Container;
using Testflow.EngineCore.Data;

namespace Testflow.EngineCore.TestMaintain
{
    /// <summary>
    /// 远程测试实体维护模块
    /// </summary>
    internal class RemoteTestEntityMaintainer : ITestEntityMaintainer
    {
        public void CreateHosts(IList<HostInfo> runnerHosts)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeContainer Generate(ITestProject testProject, params object[] param)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeContainer Generate(ISequenceGroup sequenceGroup, params object[] param)
        {
            throw new System.NotImplementedException();
        }

        public void FreeHosts()
        {
            throw new System.NotImplementedException();
        }

        public void FreeHost(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}