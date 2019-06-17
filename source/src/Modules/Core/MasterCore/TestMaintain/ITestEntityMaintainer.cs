using System.Collections.Generic;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.CoreCommon.Messages;
using Testflow.Data.Sequence;
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

        void StartHost(ISequenceFlowContainer sequenceData);

        void FreeHosts();

        void FreeHost(int id);

        RuntimeContainer Generate(ITestProject testProject, RuntimePlatform platform, params object[] param);

        RuntimeContainer Generate(ISequenceGroup sequenceGroup, RuntimePlatform platform, params object[] param);

        void SendRmtGenMessage(int session, string sequenceData);

        Dictionary<int, RuntimeContainer> TestContainers { get; }
    }
}