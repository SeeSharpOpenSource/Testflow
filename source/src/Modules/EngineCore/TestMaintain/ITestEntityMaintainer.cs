using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.EngineCore.Data;
using Testflow.EngineCore.Message;
using Testflow.EngineCore.TestMaintain.Container;

namespace Testflow.EngineCore.TestMaintain
{
    /// <summary>
    /// 测试运行时维护模块。实现测试容器的创建、管理、销毁
    /// </summary>
    internal interface ITestEntityMaintainer : IMessageHandler
    {
        void CreateHosts(IList<HostInfo> runnerHosts);

        RuntimeContainer Generate(ITestProject testProject, params object[] param);

        RuntimeContainer Generate(ISequenceGroup sequenceGroup, params object[] param);

        void FreeHosts();

        void FreeHost(int id);
    }
}