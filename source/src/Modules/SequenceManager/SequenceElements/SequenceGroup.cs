using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Testflow.Usr;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.Serializer;
using Testflow.Utility;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    [JsonConverter(typeof(SequenceJsonConvertor))]
    public class SequenceGroup : ISequenceGroup
    {
        public SequenceGroup()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Parent = null;
            this.Info = new SequenceGroupInfo();

            this.Assemblies = new AssemblyInfoCollection();
            this.Available = true;
            this.TypeDatas = new TypeDataCollection();
            this.Arguments = new ArgumentCollection();
            this.Variables = new VariableCollection();
            // 该参数只在序列化时生成
            this.Parameters = null;
            this.ExecutionModel = ExecutionModel.SequentialExecution;
            this.SetUp = new Sequence()
            {
                Name = "SetUp",
                Index = CommonConst.SetupIndex
            };
            this.Sequences = new SequenceCollection();
            this.TearDown = new Sequence()
            {
                Name = "TearDown",
                Index = CommonConst.TeardownIndex
            };
        }

        [RuntimeSerializeIgnore]
        public string Name { get; set; }
        [RuntimeSerializeIgnore]
        public string Description { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public ISequenceFlowContainer Parent { get; set; }

        [RuntimeSerializeIgnore]
        public ISequenceGroupInfo Info { get; set; }

        [RuntimeType(typeof(AssemblyInfoCollection))]
        public IAssemblyInfoCollection Assemblies { get; set; }

        [RuntimeSerializeIgnore]
        public bool Available { get; set; }

        [RuntimeType(typeof(TypeDataCollection))]
        public ITypeDataCollection TypeDatas { get; set; }

        [RuntimeType(typeof(ArgumentCollection))]
        public IArgumentCollection Arguments { get; set; }

        [RuntimeType(typeof(VariableCollection))]
        public IVariableCollection Variables { get; set; }

        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public ISequenceGroupParameter Parameters { get; set; }

        public ExecutionModel ExecutionModel { get; set; }

        [RuntimeType(typeof(Sequence))]
        public ISequence SetUp { get; set; }

        [RuntimeType(typeof(SequenceCollection))]
        public ISequenceCollection Sequences { get; set; }

        [RuntimeType(typeof(Sequence))]
        public ISequence TearDown { get; set; }

        public void Initialize(ISequenceFlowContainer parent)
        {
            ITestProject testProject = parent as ITestProject;
            this.Parent = testProject;
            ModuleUtils.SetElementName(this, testProject?.SequenceGroups);
            
            Info.Modified = true;
            if (null != testProject)
            {
                Info.Version = testProject.ModelVersion;
            }
            this.Available = true;
            // 该参数只在序列化时生成
            this.Parameters = null;
            this.ExecutionModel = ExecutionModel.SequentialExecution;
            this.SetUp = new Sequence();
            this.TearDown = new Sequence();
            RefreshSignature();
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
            string hostInfo = ModuleUtils.GetHostInfo();
            StringBuilder featureInfo = new StringBuilder(4000);
            featureInfo.Append(Name).Append(delim).Append(Info.CreationTime.ToString("datetimeFormat")).Append(delim).
                Append(Info.ModifiedTime.ToString(datetimeFormat)).Append(delim).Append(hostInfo).Append(delim)
                .Append(GetSequenceGroupFlowInfo());
            return ModuleUtils.GetHashValue(featureInfo.ToString(), Encoding.UTF8);
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
            ModuleUtils.CloneCollection(this.Arguments, arguments);

            VariableCollection variables = new VariableCollection();
            ModuleUtils.CloneFlowCollection(this.Variables, variables);

            // SequenceGroupParameter只在序列化时使用
            // Parameters只有在序列化时才会生成，在加载完成后会被删除
            ISequenceGroupParameter parameters = (null == Parameters) ? 
                null : this.Parameters.Clone() as ISequenceGroupParameter;

            SequenceCollection sequenceCollection = new SequenceCollection();
            ModuleUtils.CloneFlowCollection(this.Sequences, sequenceCollection);

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

        #region 序列化声明及反序列化构造

        public SequenceGroup(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillDeserializationInfo(info, this, this.GetType());
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ModuleUtils.FillSerializationInfo(info, this);
        }

        #endregion
    }
}