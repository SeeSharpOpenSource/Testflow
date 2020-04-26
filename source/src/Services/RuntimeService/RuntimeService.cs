using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.Runtime.Data;
using Testflow.Usr;

namespace Testflow.RuntimeService
{
    public class RuntimeService : IRuntimeService
    {
        private Modules.ISequenceManager _sequenceManager;
        private Modules.IEngineController _engineController;
        private IList<IRuntimeSession> _sessions;
       
        public ITestProject TestProject { get; set; }

        //State紧跟着_engineController的state
        public RuntimeState State { get { return _engineController.State; } }

        public IList<IRuntimeSession> Sessions { get { return _sessions; } }

        //todo 目前做不了。。等加入debug的时候再去做
        public IRuntimeConfiguration Configuration { get; set; }

        public RuntimeService()
        {
            _sessions = new List<IRuntimeSession>(5);
            //其他的调用load
        }

        #region activate, deactivate先不实现
        public void Activate()
        {
            // ignore
        }
        public void Deactivate()
        {
            // ignore
        }
        #endregion

        #region Load TestProject
        public void Load(ITestProject testProject)
        {
            //todo 目前拿不到Configuration.Type, 因为_engineController._runtimeEngine为private.并且_engineController里没有接口
            //Configuration.Type

            //validate：1.assemblies & type 2. validateVariables 3.validate parent
            _sequenceManager.ValidateSequenceData(testProject);

            TestProject = testProject;
            //todo, constants里定义一个defaultListSize
            for (int n=0; n < testProject.SequenceGroups.Count; n++)
            {
                IRuntimeContext context = new RuntimeContext($"RuntimeContext {n}", n, testProject, testProject.SequenceGroups[n]);
                IRuntimeSession session = new RuntimeSession(n, context);
                session.Initialize();
                _sessions.Add(session);
            }
            _engineController.SetSequenceData(testProject);
        }

        public void Load(ISequenceGroup sequenceGroup)
        {
            //确保没有TestProject
            TestProject = null;

            //validate：1.assemblies & type 2. validateVariables 3.validate parent
            _sequenceManager.ValidateSequenceData(sequenceGroup);


            IRuntimeContext context = new RuntimeContext($"RuntimeContext 0", 0, null, sequenceGroup);
            IRuntimeSession session = new RuntimeSession(0, context);
            session.Initialize();
            _sessions.Add(session);


            _engineController.SetSequenceData(sequenceGroup);
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

        //向engineController.runtimeEngine注册用户添加好的事件
        private void RegisterEvents()
        {
            //注册事件,空事件在runtimeEngine方法里面判断
            _engineController.RegisterRuntimeEvent(TestGenStart, Constants.TestGenerationStart, CoreCommon.Common.CoreConstants.TestProjectSessionId);
            _engineController.RegisterRuntimeEvent(TestGenOver, Constants.TestGenerationEnd, CoreCommon.Common.CoreConstants.TestProjectSessionId);
            _engineController.RegisterRuntimeEvent(TestStart, Constants.TestInstanceStart, CoreCommon.Common.CoreConstants.TestProjectSessionId);
            _engineController.RegisterRuntimeEvent(TestOver, Constants.TestInstanceOver, CoreCommon.Common.CoreConstants.TestProjectSessionId);
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
            runner.ResultManager?.RuntimeInitialize();
            _sequenceManager = TestflowRunner.GetInstance().SequenceManager;
            _engineController = TestflowRunner.GetInstance().EngineController;
        }

        public void Dispose()
        {
            TestflowRunner runner = TestflowRunner.GetInstance();
            runner.LogService?.Dispose();
            runner.ComInterfaceManager?.Dispose();
            runner.SequenceManager?.Dispose();
            runner.DataMaintainer?.Dispose();
            runner.EngineController?.Dispose();
            runner.ParameterChecker?.Dispose();
            runner.ResultManager?.Dispose();
        }
        #endregion

        #region 开始、停止运行
        //todo I18n
        public void Run()
        {
            RegisterEvents();

            if (TestProject == null && Sessions.Count == 0)
            {
                throw new TestflowException(ModuleErrorCode.ServiceNotLoaded, "RuntimeService not loaded. Load TestProject or SequenceGroup into RuntimeService first");
            }

            //todo ModuleUtils.EngineStartThread会返回错误信息，看怎么处理
            //好像也不需要返回Exception，因为应该外部来catch？？？？
            //加载进TestProject
            if (TestProject != null)
            {
                foreach(IRuntimeSession session in Sessions)
                {
                    session.Start();
                }
            }
            //加载进SequenceGroup
            else
            {
                Sessions[0].Start();
            }
        }

        public void Stop()
        {
            _engineController.Stop();
        }
        #endregion
    }
}
