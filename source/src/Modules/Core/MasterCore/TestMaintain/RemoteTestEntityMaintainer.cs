using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Data;
using Testflow.MasterCore.TestMaintain.Container;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.TestMaintain
{
    /// <summary>
    /// 远程测试实体维护模块
    /// </summary>
    internal class RemoteTestEntityMaintainer : ITestEntityMaintainer
    {
        public void ConnectHosts(IList<HostInfo> runnerHosts)
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

        public void ConnectHost(HostInfo runnerHosts)
        {
            throw new System.NotImplementedException();
        }

        public void DisconnectHost(int sessionId)
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

        public void HandleMessage(IMessage message)
        {
            throw new System.NotImplementedException();
        }

        public void AddToQueue(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}