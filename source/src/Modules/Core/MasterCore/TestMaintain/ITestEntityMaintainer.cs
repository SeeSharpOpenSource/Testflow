using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Data;
using Testflow.MasterCore.Message;
using Testflow.MasterCore.TestMaintain.Container;

namespace Testflow.MasterCore.TestMaintain
{
    /// <summary>
    /// 测试运行时维护模块。实现测试容器的创建、管理、销毁
    /// </summary>
    internal interface ITestEntityMaintainer : IMessageHandler
    {
        void ConnectHost(HostInfo runnerHosts);

        void DisconnectHost(int sessionId);

        void FreeHosts();

        RuntimeContainer Generate(ITestProject testProject, params object[] param);

        RuntimeContainer Generate(ISequenceGroup sequenceGroup, params object[] param);
    }
}