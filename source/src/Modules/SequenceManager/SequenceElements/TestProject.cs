using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Testflow.Usr;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.Serializer;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    [SerializationOrderEnable]
    [JsonConverter(typeof(SequenceJsonConvertor))]
    public class TestProject : ITestProject
    {
        public TestProject()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.TypeDatas = new TypeDataCollection();
            this.Assemblies = new AssemblyInfoCollection();
            this.ExecutionModel = ExecutionModel.SequentialExecution;
            this.Variables = new VariableCollection();
            this.VariableValues = new VariableInitValueCollection();
            this.SetUp = new Sequence()
            {
                Name = "SetUp",
                Index = CommonConst.SetupIndex
            };
            this.SetUpParameters = new SequenceParameter();
            this.SetUpParameters.Initialize(this.SetUp);

            this.SequenceGroups = new SequenceGroupCollection();
            this.SequenceGroupParameters = new ParameterDataCollections();
            this.SequenceGroupLocations = null;
            this.TearDown = new Sequence()
            {
                Name = "TearDown",
                Index = CommonConst.TeardownIndex
            };
            this.TearDownParameters = new SequenceParameter();
            this.TearDownParameters.Initialize(this.TearDown);
            this.ModelVersion = string.Empty;
            this.Platform = RunnerPlatform.Default;
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
        public RunnerPlatform Platform { get; set; }

        [SerializationOrder(0)]
        [RuntimeType(typeof(AssemblyInfoCollection))]
        public IAssemblyInfoCollection Assemblies { get; set; }

        [SerializationOrder(1)]
        [RuntimeType(typeof(TypeDataCollection))]
        public ITypeDataCollection TypeDatas { get; set; }

        [RuntimeSerializeIgnore]
        public string ModelVersion { get; set; }

        public ExecutionModel ExecutionModel { get; set; }

        [SerializationOrder(2)]
        [RuntimeType(typeof(VariableCollection))]
        public IVariableCollection Variables { get; set; }

        [SerializationOrder(3)]
        [RuntimeType(typeof(VariableInitValueCollection))]
        public IList<IVariableInitValue> VariableValues { get; set; }

        [SerializationOrder(4)]
        [RuntimeType(typeof(Sequence))]
        public ISequence SetUp { get; set; }

        [SerializationOrder(5)]
        [RuntimeType(typeof(SequenceParameter))]
        public ISequenceParameter SetUpParameters { get; set; }

        [SerializationOrder(6)]
        [XmlIgnore]
        [SerializationIgnore]
        [RuntimeSerializeIgnore]
        public ISequenceGroupCollection SequenceGroups { get; set; }

        [SerializationOrder(7)]
        [RuntimeSerializeIgnore]
        public SequenceGroupLocationInfoCollection SequenceGroupLocations { get; set; }

        [SerializationOrder(8)]
        [RuntimeType(typeof(ParameterDataCollections))]
        public IList<IParameterDataCollection> SequenceGroupParameters { get; set; }

        [SerializationOrder(9)]
        [RuntimeType(typeof(Sequence))]
        public ISequence TearDown { get; set; }

        [SerializationOrder(10)]
        [RuntimeType(typeof(SequenceParameter))]
        public ISequenceParameter TearDownParameters { get; set; }

        public void Initialize(ISequenceFlowContainer parent)
        {
            if (!ModuleUtils.IsValidName(this.Name))
            {
                this.Name = string.Format(Constants.TestProjectNameFormat, 1);
            }
            this.SequenceGroupLocations = new SequenceGroupLocationInfoCollection();
        }

        public ISequenceFlowContainer Clone()
        {
            throw new InvalidOperationException();
        }

        #region 序列化声明及反序列化构造

        public TestProject(SerializationInfo info, StreamingContext context)
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