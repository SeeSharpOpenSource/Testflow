using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.DesignTime;
using Testflow.SequenceManager.SequenceElements;
using Testflow.DesigntimeService.Common;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.DesigntimeService
{
    public class DesignTimeService : IDesignTimeService
    {
        private Modules.ISequenceManager _sequenceManager;
        private Modules.IComInterfaceManager _interfaceManager;

        #region 属性
        /// <summary>
        /// 当前服务的测试工程
        /// </summary>
        public ITestProject TestProject { get; set; }

        /// <summary>
        /// SetUp模块的会话
        /// </summary>
        public IDesignTimeSession SetUpSession { get; set; }

        /// <summary>
        /// 保存所有设计时会话的集合
        /// 键值对为IDesignTimeSession.SessionId, IDesignTimeSession
        /// sessionId，现为序列组在testproject.SequenceGroups的index
        /// </summary>
        public IDictionary<int, IDesignTimeSession> SequenceSessions { get; set; }

        /// <summary>
        /// 设计时导入的所有组件和程序集的映射
        /// 键值对为Assembly.AssemblyName, IComInterfaceDescription
        /// </summary>
        public IDictionary<string, IComInterfaceDescription> Components { get; set; }

        /// <summary>
        /// TearDown模块的会话
        /// </summary>
        public IDesignTimeSession TearDownSession { get; set; }
        #endregion

        //todo SetUp/TearDown = null
        public DesignTimeService()
        {
            this.TestProject = null; // need to load

            SequenceSessions = new Dictionary<int, IDesignTimeSession>();
            Components = new Dictionary<string, IComInterfaceDescription>();

            I18NOption i18NOption = new I18NOption(this.GetType().Assembly, "i18n_designService_zh",
                "i18n_designService_en")
            {
                Name = Constants.I18nName
            };
            I18N.InitInstance(i18NOption);

            SetUpSession = null;
            TearDownSession = null;
        }


        #region 设计时控制

        #region activate, deactivate先不实现
        public void Activate()
        {
            //todo
            //反序列化已存在的TestProject
            //或
            //todo
            //用load去创建新TestProject

        }
        public void Deactivate()
        {
            // ignore
        }

        public void Rename(ISequenceFlowContainer target, string newName)
        {
            ModuleUtils.Rename(target, newName);
        }

        #endregion

        #region load; unload
        public ITestProject Load(string name, string description)
        {
            //这里不去检查TestProject旧值, 如果值混乱，怪用户没有unload
            TestProject = _sequenceManager.CreateTestProject();
            TestProject.Name = name;
            TestProject.Description = description;
            return TestProject;
        }

        public ITestProject Load(ITestProject testProject)
        {
            TestProject = testProject;
            foreach (ISequenceGroup sequenceGroup in testProject.SequenceGroups)
            {
                AddSequenceGroup(sequenceGroup);
            }
            return TestProject;
        }

        public ITestProject Load(string name, string description, ISequenceGroup sequenceGroup)
        {
            TestProject = _sequenceManager.CreateTestProject();
            TestProject.Name = name;
            TestProject.Description = description;
            sequenceGroup.Parent = TestProject;
            AddSequenceGroup(sequenceGroup);
            return TestProject;
        }

        //todo SetUp/TearDown = null
        public void Unload()
        {
            this.TestProject = null; // need to load
            foreach (IDesignTimeSession designTimeSession in SequenceSessions.Values)
            {
                designTimeSession.Dispose();
            }

            SequenceSessions.Clear();
            Components?.Clear();

            SetUpSession = null;
            TearDownSession = null;
        }
        #endregion

        #endregion

        #region 设计时支持 先不实现
        public IList<string> GetFittedProperties(string variable, string propertyPrefix)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 加减SequenceGroup
        public IDesignTimeSession AddSequenceGroup(string name, string description)
        {
            //添加到TestProject
            ISequenceGroup sequenceGroup = _sequenceManager.CreateSequenceGroup();
            sequenceGroup.Name = name;
            sequenceGroup.Description = description;
            return AddSequenceGroup(sequenceGroup);
        }

        public IDesignTimeSession AddSequenceGroup(ISequenceGroup sequenceGroup)
        {
            int index = TestProject.SequenceGroups.Count;
            //添加到TestProject
            TestProject.SequenceGroups.Add(sequenceGroup);
            foreach(IComInterfaceDescription comDescription in _interfaceManager.GetComponentInterfaces(sequenceGroup.Assemblies))
            {
                AddComponent(comDescription);
            }
            //添加到SequenceSessions
            IDesignTimeSession designtimeSession = new DesignTimeSession(index, sequenceGroup);
            SequenceSessions.Add(index, designtimeSession);
            return designtimeSession;
        }

        public IDesignTimeSession RemoveSequenceGroup(string name, string description)
        {
            ISequenceGroup sequenceGroup = TestProject.SequenceGroups.FirstOrDefault(item => item.Name.Equals(name) && item.Description.Equals(description));
            //可能传入null，没关系，同样报错
            return RemoveSequenceGroup(sequenceGroup);
        }
        
        //todo I18n
        public IDesignTimeSession RemoveSequenceGroup(ISequenceGroup sequenceGroup)
        {
            //在TestProject里找寻sequenceGroup的sessionId
            int sessionId = TestProject.SequenceGroups.IndexOf(sequenceGroup);
            if (sessionId == -1)
            {
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, "SequenceGroup does not exist in current service");
            }
            IDesignTimeSession designTimeSession = SequenceSessions[sessionId];
            //从SequenceSessions去除
            SequenceSessions.Remove(sessionId);

            //从TestProject去除
            TestProject.SequenceGroups.RemoveAt(sessionId);
            FixSessionID(sessionId);
            return designTimeSession;
        }

        public IDesignTimeSession RemoveSequenceGroup(IDesignTimeSession designTimeSession)
        {
            return RemoveSequenceGroup(designTimeSession.Context.SequenceGroup);
        }
        #endregion

        #region 加减程序集
        //to ask
        public IComInterfaceDescription AddComponent(IComInterfaceDescription comInterface)
        {
            //to ask: 是否要加到TestProject.Assemblies的assemblyinfo？; 估计不要
            //TestProject.Assemblies.Add(comInterface.Assembly);
            if (!Components.ContainsKey(comInterface.Assembly.AssemblyName))
            {   
                Components.Add(comInterface.Assembly.AssemblyName, comInterface);
            }
            return comInterface;
        }

        //to do
        public IComInterfaceDescription RemoveComponent(IComInterfaceDescription comInterface)
        {
            if (Components.ContainsKey(comInterface.Assembly.AssemblyName))
            {
                Components.Remove(comInterface.Assembly.AssemblyName);
            }
            else
            {
                //I18N
                throw new TestflowRuntimeException(ModuleErrorCode.TargetNotExist, "没有该组件");
            }
            return comInterface;
        }

        //to do
        public IComInterfaceDescription RemoveComponent(string componentName)
        {
            IComInterfaceDescription comInterfaceDescription = null;
            if (Components.TryGetValue(componentName, out comInterfaceDescription))
            {
                Components.Remove(componentName);
            }
            else
            {
                //I18N
                throw new TestflowDataException(ModuleErrorCode.TargetNotExist, "没有该组件");
            }
            return comInterfaceDescription;
        }
        #endregion

        /// <summary>
        /// 修正SequenceSessions里后续IDesigntimeSession里的sessionId
        /// 更新SequenceSessions里的键值对
        /// todo 需测试正确性
        /// </summary>
        /// <param name="index"></param>
        private void FixSessionID(int index)
        {
            for(;index < TestProject.SequenceGroups.Count; index++)
            {
                IDesignTimeSession session = SequenceSessions[index + 1];
                session.SessionId = index;
                SequenceSessions.Remove(index + 1);
                SequenceSessions.Add(index, session);
            }
        }


        public void Initialize()
        {
            TestflowRunner runner = TestflowRunner.GetInstance();
            runner.LogService.DesigntimeInitialize();
            runner.ComInterfaceManager.DesigntimeInitialize();
            runner.SequenceManager.DesigntimeInitialize();
            runner.DataMaintainer.DesigntimeInitialize();
            runner.EngineController.DesigntimeInitialize();
            runner.ResultManager?.DesigntimeInitialize();
            runner.ParameterChecker?.DesigntimeInitialize();
            _sequenceManager = TestflowRunner.GetInstance().SequenceManager;
            _interfaceManager = TestflowRunner.GetInstance().ComInterfaceManager;

        }

        private int _diposedFlag = 0;
        public void Dispose()
        {
            if (_diposedFlag != 0)
            {
                return;
            }
            Thread.VolatileWrite(ref _diposedFlag, 1);
            Thread.MemoryBarrier();
            Unload();
            I18N.RemoveInstance(Constants.I18nName);
        }
    }
}
