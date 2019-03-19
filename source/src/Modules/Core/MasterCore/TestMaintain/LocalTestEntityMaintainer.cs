using System.Collections.Generic;
using Testflow.CoreCommon.Data;
using Testflow.Data.Sequence;
using Testflow.MasterCore.Common;
using Testflow.MasterCore.TestMaintain.Container;
using Testflow.Utility.MessageUtil;

namespace Testflow.MasterCore.TestMaintain
{
    /// <summary>
    /// 本地测试实体维护模块
    /// </summary>
    internal class LocalTestEntityMaintainer : ITestEntityMaintainer
    {
        private readonly ModuleGlobalInfo _globalInfo;
        private Dictionary<int, RuntimeContainer> _runtimeContainers;

        public LocalTestEntityMaintainer(ModuleGlobalInfo globalInfo)
        {
            _globalInfo = globalInfo;
            this._runtimeContainers = new Dictionary<int, RuntimeContainer>(Constants.DefaultRuntimeSize);
        }

        /// <summary>
        /// 连接运行主机端
        /// </summary>
        public void ConnectHost(HostInfo runnerHosts)
        {
            // ignore
        }

        /// <summary>
        /// 释放远程主机端的连接
        /// </summary>
        public void DisconnectHost(int sessionId)
        {
            // ignore
        }

        public void FreeHost(int id)
        {
            // ignore
        }

        public RuntimeContainer Generate(ITestProject testProject, params object[] param)
        {
            string sequenceStr = _globalInfo.TestflowRunner.SequenceManager.RuntimeSerialize(testProject);
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