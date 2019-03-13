using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    // SequenceGroupParameter只在序列化时使用
    public class SequenceGroupParameter : ISequenceGroupParameter
    {
        public SequenceGroupParameter()
        {
            this.Info = new SequenceParameterInfo();
            this.VariableValues = null;
            this.SetUpParameters = null;
            this.SequenceParameters = null;
            this.TearDownParameters = null;
            this.Name = string.Empty;
            this.Description = string.Empty;
        }
        [RuntimeSerializeIgnore]
        public string Name { get; set; }
        [RuntimeSerializeIgnore]
        public string Description { get; set; }
        [RuntimeSerializeIgnore]
        public ISequenceParameterInfo Info { get; set; }
        [RuntimeType(typeof(VariableInitValueCollection))]
        public IList<IVariableInitValue> VariableValues { get; set; }
        [RuntimeType(typeof(SequenceParameter))]
        public ISequenceParameter SetUpParameters { get; set; }
        [RuntimeType(typeof(SequenceParameterCollection))]
        public IList<ISequenceParameter> SequenceParameters { get; set; }
        [RuntimeType(typeof(SequenceParameter))]
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

            this.Info.Hash = sequenceGroup.Info.Hash;
            this.Info.Modified = true;
            this.Info.ModifiedTime = DateTime.Now;
            this.Info.Path = sequenceGroup.Info.SequenceParamFile;
            this.Info.Version = sequenceGroup.Info.Version;
            this.VariableValues = new VariableInitValueCollection();

            foreach (IVariable variable in sequenceGroup.Variables)
            {
                VariableInitValue variableInitValue = new VariableInitValue()
                {
                    Name = variable.Name
                };
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Common.Utility.FillSerializationInfo(info, this);
        }
    }
}