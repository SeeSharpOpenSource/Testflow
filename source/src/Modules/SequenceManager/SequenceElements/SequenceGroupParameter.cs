using System;
using System.Collections.Generic;
using Testflow.Data.Sequence;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    // SequenceGroupParameter只在序列化时使用
    internal class SequenceGroupParameter : ISequenceGroupParameter
    {
        public SequenceGroupParameter()
        {
            this.Info = null;
            this.VariableValues = null;
            this.SetUpParameters = null;
            this.SequenceParameters = null;
            this.TearDownParameters = null;
        }

        public ISequenceParameterInfo Info { get; set; }
        public IList<IVariableInitValue> VariableValues { get; set; }
        public ISequenceParameter SetUpParameters { get; set; }
        public IList<ISequenceParameter> SequenceParameters { get; set; }
        public ISequenceParameter TearDownParameters { get; set; }

        public void RefreshSignature(SequenceGroup parent)
        {
            this.Info.Hash = parent.Info.Hash;
            this.Info.Version = parent.Info.Version;
            if (Info.Modified)
            {
                this.Info.Modified = false;
                this.Info.ModifiedTime = DateTime.Now;
            }
        }

        public ISequenceDataContainer Clone()
        {
            SequenceParameterInfo parameterInfo = new SequenceParameterInfo()
            {
                CreateTime = DateTime.Now,
                Hash = string.Empty,
                Modified = false,
                ModifiedTime = DateTime.Now,
                Path = string.Empty,
                Version = this.Info.Version
            };

            VariableInitValueCollection initValueCollection = new VariableInitValueCollection();
            Common.Utility.CloneDataCollection(this.VariableValues, initValueCollection);

            SequenceParameterCollection sequenceParameterCollection = new SequenceParameterCollection();
            Common.Utility.CloneDataCollection(this.SequenceParameters, sequenceParameterCollection);

            SequenceGroupParameter parameter = new SequenceGroupParameter()
            {
                Info = parameterInfo,
                VariableValues = initValueCollection,
                SetUpParameters = this.SetUpParameters.Clone() as ISequenceParameter,
                SequenceParameters = sequenceParameterCollection,
                TearDownParameters = this.TearDownParameters.Clone() as ISequenceParameter,
            };
            return parameter;
        }

        public void Initialize(ISequenceFlowContainer flowContainer)
        {
            ISequenceGroup sequenceGroup = flowContainer as ISequenceGroup;
            this.Info = new SequenceParameterInfo()
            {
                Hash = sequenceGroup.Info.Hash,
                Modified = false,
                CreateTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                Path = sequenceGroup.Info.SequenceParamFile,
                Version = sequenceGroup.Info.Version
            };
            this.VariableValues = new VariableInitValueCollection();
            foreach (IVariable variable in sequenceGroup.Variables)
            {
                VariableInitValue variableInitValue = new VariableInitValue();
                variableInitValue.Initialize(variable);
                this.VariableValues.Add(variableInitValue);
            }

            this.SetUpParameters = new SequenceParameter();
            this.SetUpParameters.Initialize(sequenceGroup.SetUp);

            this.SequenceParameters = new SequenceParameterCollection();
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                SequenceParameter parameter = new SequenceParameter();
                parameter.Initialize(sequence);
                SequenceParameters.Add(parameter);
            }

            this.TearDownParameters = new SequenceParameter();
            this.TearDownParameters.Initialize(sequenceGroup.TearDown);
        }
    }
}