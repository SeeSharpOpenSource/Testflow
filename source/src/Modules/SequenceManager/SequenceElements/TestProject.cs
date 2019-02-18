using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.Common;
using Testflow.Data;
using Testflow.Data.Sequence;

namespace Testflow.SequenceManager.SequenceElements
{
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
            this.SetUp = new Sequence();
            this.SetUpParameters = new SequenceParameter();
            this.SequenceGroups = new SequenceGroupCollection();
            this.SequenceGroupParameters = new List<IParameterDataCollection>();
            this.TearDown = new Sequence();
            this.TearDownParameters = new SequenceParameter();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public ISequenceFlowContainer Parent
        {
            get { return null; }
            set { throw new InvalidOperationException(); }
        }

        public void Initialize(ISequenceFlowContainer parent)
        {
            return;
        }

        public ITypeDataCollection TypeDatas { get; set; }
        public IAssemblyInfoCollection Assemblies { get; set; }
        public ExecutionModel ExecutionModel { get; set; }
        public IVariableCollection Variables { get; set; }
        public IList<IVariableInitValue> VariableValues { get; set; }
        public ISequence SetUp { get; set; }
        public ISequenceParameter SetUpParameters { get; set; }
        public ISequenceGroupCollection SequenceGroups { get; set; }
        public IList<IParameterDataCollection> SequenceGroupParameters { get; set; }
        public ISequence TearDown { get; set; }
        public ISequenceParameter TearDownParameters { get; set; }

        public ISequenceFlowContainer Clone()
        {
            throw new InvalidOperationException();
        }
    }
}