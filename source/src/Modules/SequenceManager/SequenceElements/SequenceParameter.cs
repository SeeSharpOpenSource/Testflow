using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    public class SequenceParameter : ISequenceParameter
    {
        public SequenceParameter()
        {
            this.Index = Constants.UnverifiedIndex;
            this.StepParameters = null;
            this.VariableValues = null;
        }

        public int Index { get; set; }
        [RuntimeType(typeof(SequenceStepParameterCollection))]
        public IList<ISequenceStepParameter> StepParameters { get; set; }
        [RuntimeType(typeof(VariableInitValueCollection))]
        public IList<IVariableInitValue> VariableValues { get; set; }

        public void Initialize(ISequenceFlowContainer flowContainer)
        {
            ISequence sequence = flowContainer as ISequence;
            this.Index = sequence.Index;

            this.StepParameters = new SequenceStepParameterCollection();
            foreach (ISequenceStep sequenceStep in sequence.Steps)
            {
                SequenceStepParameter stepParameter = new SequenceStepParameter();
                stepParameter.Initialize(sequenceStep);
                this.StepParameters.Add(stepParameter);
            }

            this.VariableValues = new VariableInitValueCollection();
            foreach (IVariable variable in sequence.Variables)
            {
                VariableInitValue initValue = new VariableInitValue()
                {
                    Name = variable.Name
                };
                initValue.Initialize(variable);
                this.VariableValues.Add(initValue);
            }
        }

        public ISequenceDataContainer Clone()
        {
            throw new System.NotImplementedException();
        }

        #region 序列化声明及反序列化构造

        public SequenceParameter(SerializationInfo info, StreamingContext context)
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