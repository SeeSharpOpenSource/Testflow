using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testflow.Data.Description;
using Testflow.Data.Sequence;
using Testflow.DesignTime;
using Testflow.SequenceManager.SequenceElements;
using Testflow.DesigntimeService.Common;
using Testflow.Usr;

namespace Testflow.DesigntimeService
{
    public class DesignTimeService : IDesignTimeService
    {
        private Modules.ISequenceManager _sequenceManager;

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

        public DesignTimeService()
        {
            this.TestProject = null; // need to load
            SetUpSession = new DesignTimeSession()
            {
                SessionId = CommonConst.SetupIndex
            };
            SequenceSessions = new Dictionary<int, IDesignTimeSession>();
            Components = new Dictionary<string, IComInterfaceDescription>();
            TearDownSession = new DesignTimeSession()
            {
                SessionId = CommonConst.TeardownIndex
            };
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
            throw new NotImplementedException();

        }
        public void Deactivate()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region load; unload
        public ITestProject Load(string name, string description)
        {
            TestProject = _sequenceManager.CreateTestProject();
            TestProject.Name = name;
            TestProject.Description = description;
            return TestProject;
        }

        public ITestProject Load(ITestProject testProject)
        {
            TestProject = testProject;
            return TestProject;
        }

        public ITestProject Load(string name, string description, ISequenceGroup sequenceGroup)
        {
            TestProject = _sequenceManager.CreateTestProject();
            TestProject.Name = name;
            TestProject.Description = description;
            TestProject.SequenceGroups.Add(sequenceGroup);
            return TestProject;
        }

        public void Unload()
        {
            TestProject = null;
            SetUpSession = null;
            SequenceSessions = null;
            Components = null;
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

        #region Sequence Edit
        //to ask
        public IComInterfaceDescription AddComponent(IComInterfaceDescription comInterface)
        {
            //to ask: 是否要加到TestProject.Assemblies的assemblyinfo？; 估计不要
            //TestProject.Assemblies.Add(comInterface.Assembly);
            Components.Add(comInterface.Assembly.AssemblyName, comInterface);
            return comInterface;
        }

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
            //添加到TestProject
            TestProject.SequenceGroups.Add(sequenceGroup);

            //添加到SequenceSessions
            IDesignTimeSession designtimeSession = new DesignTimeSession();
            int index = ModuleUtils.GetSessionId(TestProject, sequenceGroup);
            designtimeSession.SessionId = index;
            SequenceSessions.Add(index, designtimeSession);
            return designtimeSession;
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
                throw new TestflowRuntimeException(ModuleErrorCode.ComponentDNE, "没有该组件");
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
                throw new TestflowDataException(ModuleErrorCode.ComponentDNE, "没有该组件");
            }
            return comInterfaceDescription;
        }

        public IDesignTimeSession RemoveSequenceGroup(string name, string description)
        {
            //判断是否存在
            ISequenceGroup sequenceGroup = TestProject.SequenceGroups.FirstOrDefault(item => item.Name.Equals(name) && item.Description.Equals(description));
            if(sequenceGroup == null)
            {
                //I18N
                throw new TestflowDataException(ModuleErrorCode.SequenceGroupDNE, "没有该序列组");
            }

            return InternalRemoveSequenceGroup(sequenceGroup);
        }

        public IDesignTimeSession RemoveSequenceGroup(ISequenceGroup sequenceGroup)
        {
            //判断是否存在
            if (!TestProject.SequenceGroups.Contains(sequenceGroup))
            {
                //I18N
                throw new TestflowDataException(ModuleErrorCode.SequenceGroupDNE, "没有该序列组");
            }
            TestProject.SequenceGroups.Remove(sequenceGroup);
            return InternalRemoveSequenceGroup(sequenceGroup);
        }

        private IDesignTimeSession InternalRemoveSequenceGroup(ISequenceGroup sequenceGroup)
        {
            //从SequenceSessions去除
            IDesignTimeSession designTimeSession = null;
            int sessionId = ModuleUtils.GetSessionId(TestProject, sequenceGroup);
            SequenceSessions.TryGetValue(sessionId, out designTimeSession);
            SequenceSessions.Remove(sessionId);


            //从TestProject去除
            TestProject.SequenceGroups.Remove(sequenceGroup);

            FixSessionID(sessionId);

            return designTimeSession;
        }

        public IDesignTimeSession RemoveSequenceGroup(IDesignTimeSession designTimeSession)
        {
            //判断是否存在
            KeyValuePair<int, IDesignTimeSession> kv = SequenceSessions.FirstOrDefault(item => item.Value.Equals(designTimeSession));
            if (kv.Equals(default(KeyValuePair<int, IDesignTimeSession>)))
            {
                throw new TestflowDataException(ModuleErrorCode.SequenceGroupDNE, "没有该序列组");
            }

            //从TestProject去除
            TestProject.SequenceGroups.Remove(ModuleUtils.GetSequenceGroup(kv.Key, TestProject));

            //从SequenceSession去除
            SequenceSessions.Remove(kv.Key);

            FixSessionID(kv.Key);

            return kv.Value;
        }
        
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

        #endregion

        public void Initialize()
        {
            _sequenceManager = TestflowRunner.GetInstance().SequenceManager;
        }

        public void Dispose()
        {
            Unload();
            _sequenceManager?.Dispose();
        }
    }
}
