using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.EngineCore.Common;
using Testflow.EngineCore.Container;
using Testflow.EngineCore.Data;
using Testflow.EngineCore.Message.Messages;

namespace Testflow.EngineCore.TestMaintain
{
    /// <summary>
    /// 本地测试实体维护模块
    /// </summary>
    internal class LocalTestEntityMaintainer : ITestEntityMaintainer
    {
        private ModuleGlobalInfo _globalInfo;

        public LocalTestEntityMaintainer(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
        }

        public void CreateHosts(IList<HostInfo> runnerHosts)
        {
            throw new System.NotImplementedException();
        }

        public RuntimeContainer Generate(ITestProject testProject, params object[] param)
        {
            RmtGenMessage rmtGenMessage = new RmtGenMessage();
            string sequenceStr = _globalInfo.TestflowRunner.SequenceManager.RuntimeSerialize(testProject);
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