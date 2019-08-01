using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.RuntimeService.Common;

namespace Testflow.RuntimeService
{
    public class RuntimeService : IRuntimeService
    {
        private Modules.ISequenceManager _sequenceManager;
        private Modules.IEngineController _engineController;
       
        public ITestProject TestProject { get; set; }

        //todo State跟随EngineController State: State = _engineController.GetRuntimeState()
        public RuntimeState State { get; }

        public IList<IRuntimeSession> Sessions { get; }

        public IRuntimeConfiguration Configuration { get; set; }

        #region activate, deactivate先不实现
        public void Activate()
        {
            throw new NotImplementedException();
        }
        public void Deactivate()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Load TestProject
        public void Load(ITestProject testProject)
        {
            TestProject = testProject;
        }

        public void Load(ISequenceGroup sequenceGroup)
        {
            TestProject = _sequenceManager.CreateTestProject();
            TestProject.SequenceGroups.Add(sequenceGroup);
        }
        #endregion

        #region GetSession返回IRuntimeSession
        public IRuntimeSession GetSession(string sequenceGroupName)
        {
            int sessionId = ModuleUtils.GetSessionId(TestProject, sequenceGroupName);
            return Sessions.FirstOrDefault(item => (item.ID == sessionId));
        }

        public IRuntimeSession GetSession(ISequenceGroup sequenceGroup)
        {
            int sessionId = ModuleUtils.GetSessionId(TestProject, sequenceGroup);
            return Sessions.FirstOrDefault(item => (item.ID == sessionId));
        }
        #endregion

        #region 事件相关
        public event RuntimeDelegate.TestGenerationAction TestGenStart;
        public event RuntimeDelegate.TestGenerationAction TestGenOver;
        public event RuntimeDelegate.TestInstanceStatusAction TestStart;
        public event RuntimeDelegate.TestInstanceStatusAction TestOver;

        public void RuntimeServiceRegister(Delegate callBack, string eventName)
        {
            _engineController.RegisterRuntimeEvent(callBack,eventName, CoreCommon.Common.CoreConstants.TestProjectIndex);
        }

        private void OnTestGenStart(ITestGenerationInfo generationinfo)
        {
            TestGenStart?.Invoke(generationinfo);
        }

        private void OnTestGenOver(ITestGenerationInfo generationinfo)
        {
            TestGenOver?.Invoke(generationinfo);
        }

        private void OnTestStart(IList<ITestResultCollection> statistics)
        {
            TestStart?.Invoke(statistics);
        }
        private void OnTestOver(IList<ITestResultCollection> statistics)
        {
            TestOver?.Invoke(statistics);
        }
        #endregion

        #region 初始化
        public void Initialize()
        {
            TestflowRunner runner = TestflowRunner.GetInstance();
            runner.LogService.RuntimeInitialize();
            //动态启用ComInterfaceManager再说吧
            //runner.ComInterfaceManager.RuntimeInitialize();
            runner.SequenceManager.RuntimeInitialize();
            runner.DataMaintainer.RuntimeInitialize();
            runner.EngineController.RuntimeInitialize();
            _sequenceManager = TestflowRunner.GetInstance().SequenceManager;
            _engineController = TestflowRunner.GetInstance().EngineController;
        }

        public void Dispose()
        {
            _sequenceManager?.Dispose();
            _engineController?.Dispose();
        }
        #endregion

        #region 开始、停止运行
        public void Run()
        {
            _engineController.Start();
        }

        public void Stop()
        {
            _engineController.Stop();
        }
        #endregion
    }
}
