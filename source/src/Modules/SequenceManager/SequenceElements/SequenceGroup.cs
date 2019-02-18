using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;
using Testflow.Utility;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceGroup : ISequenceGroup
    {
        public SequenceGroup()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Parent = null;
            this.Info = null;
            this.Assemblies = null;
            this.TypeDatas = null;
            this.Arguments = null;
            this.Variables = null;
            this.Parameters = null;
            this.ExecutionModel = ExecutionModel.SequentialExecution;
            this.SetUp = null;
            this.Sequences = null;
            this.TearDown = null;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        public ISequenceFlowContainer Parent { get; set; }

        public ISequenceGroupInfo Info { get; set; }

        public IAssemblyInfoCollection Assemblies { get; set; }

        public ITypeDataCollection TypeDatas { get; set; }

        public IArgumentCollection Arguments { get; set; }

        public IVariableCollection Variables { get; set; }

        public ISequenceGroupParameter Parameters { get; set; }

        public ExecutionModel ExecutionModel { get; set; }

        public ISequence SetUp { get; set; }

        public ISequenceCollection Sequences { get; set; }

        public ISequence TearDown { get; set; }

        public void Initialize(ISequenceFlowContainer parent)
        {
            ITestProject testProject = parent as ITestProject;
            string[] existNames = new string[0];
            if (null != testProject)
            {
                this.Parent = testProject;
                existNames = (from sequenceGroup in testProject.SequenceGroups
                                       where !ReferenceEquals(sequenceGroup, this)
                                       select sequenceGroup.Name).ToArray();
            }
            if (!Common.Utility.IsValidName(this.Name, existNames))
            {
                this.Name = Common.Utility.GetDefaultName(existNames, Constants.SequenceGroupNameFormat, 0);
            }

            this.Info = new SequenceGroupInfo();
            this.Assemblies = new AssemblyInfoCollection();
            this.TypeDatas = new TypeDataCollection();
            this.Arguments = new ArgumentCollection();
            this.Variables = new VariableCollection();
            // 该参数只在序列化时生成
            this.Parameters = null;
            this.ExecutionModel = ExecutionModel.SequentialExecution;
            this.SetUp = new Sequence();
            this.Sequences = new SequenceCollection();
            this.TearDown = new Sequence();
        }

        public void RefreshSignature()
        {
            if (!Info.Modified)
            {
                return;
            }
            this.Info.ModifiedTime = DateTime.Now;
            this.Info.Modified = false;
            this.Info.Hash = this.GetSequenceGroupSignature();
            if (null != Parameters)
            {
                SequenceGroupParameter sequenceGroupParameter = Parameters as SequenceGroupParameter;
                sequenceGroupParameter.RefreshSignature(this);
                Parameters.Info.Hash = this.Info.Hash;
            }
        }

        private string GetSequenceGroupSignature()
        {
            const string datetimeFormat = "yyyy-mm-dd-hh-MM-ss-fff";
            const string delim = "_";
            string hostInfo = Common.Utility.GetHostInfo();
            StringBuilder featureInfo = new StringBuilder(4000);
            featureInfo.Append(Name).Append(delim).Append(Info.CreationTime.ToString("datetimeFormat")).Append(delim).
                Append(Info.ModifiedTime.ToString(datetimeFormat)).Append(delim).Append(hostInfo).Append(delim)
                .Append(GetSequenceGroupFlowInfo());
            return featureInfo.ToString();
        }

        /// <summary>
        /// 获取序列组配置的特征字符串
        /// </summary>
        private string GetSequenceGroupFlowInfo()
        {
            const string delim = "_";
            StringBuilder flowInfo = new StringBuilder(2000);
            flowInfo.Append(GetSequenceFlowInfo(SetUp)).Append(delim).Append(GetSequenceFlowInfo(TearDown));
            foreach (ISequence sequence in Sequences)
            {
                flowInfo.Append(delim).Append(GetSequenceFlowInfo(sequence));
            }
            return flowInfo.ToString();
        }

        /// <summary>
        /// 获取序列配置的特征字符串
        /// </summary>
        private string GetSequenceFlowInfo(ISequence sequence)
        {
            // TODO 暂时简单处理，后续有必要再修改
            return $"{sequence.Name}_{sequence.Variables.Count}_{sequence.Steps.Count}";
        }

        ISequenceFlowContainer ICloneableClass<ISequenceFlowContainer>.Clone()
        {
            AssemblyInfoCollection assemblies = new AssemblyInfoCollection();
            foreach (IAssemblyInfo assemblyInfo in this.Assemblies)
            {
                assemblies.Add(assemblyInfo);
            }

            TypeDataCollection typeDatas = new TypeDataCollection();
            foreach (ITypeData typeData in TypeDatas)
            {
                typeDatas.Add(typeData);
            }

            ArgumentCollection arguments = new ArgumentCollection();
            Common.Utility.CloneCollection(this.Arguments, arguments);

            VariableCollection variables = new VariableCollection();
            Common.Utility.CloneFlowCollection(this.Variables, variables);

            // SequenceGroupParameter只在序列化时使用
            // Parameters只有在序列化时才会生成，在加载完成后会被删除
            ISequenceGroupParameter parameters = (null == Parameters) ? 
                null : this.Parameters.Clone() as ISequenceGroupParameter;

            SequenceCollection sequenceCollection = new SequenceCollection();
            Common.Utility.CloneFlowCollection(this.Sequences, sequenceCollection);

            SequenceGroup sequenceGroup = new SequenceGroup()
            {
                Name = this.Name + Constants.CopyPostfix,
                Description = this.Description,
                Parent = this.Parent,
                Info = this.Info.Clone(),
                Assemblies = assemblies,
                TypeDatas = typeDatas,
                Arguments = arguments,
                Variables = variables,
                Parameters = parameters,
                ExecutionModel = this.ExecutionModel,
                SetUp = this.SetUp.Clone() as ISequence,
                Sequences = sequenceCollection,
                TearDown = this.TearDown.Clone() as ISequence
            };
            sequenceGroup.RefreshSignature();
            return sequenceGroup;
        }
    }
}