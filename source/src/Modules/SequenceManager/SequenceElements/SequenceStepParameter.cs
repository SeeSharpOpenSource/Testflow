using System;
using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.SequenceManager.Common;

namespace Testflow.SequenceManager.SequenceElements
{
    [Serializable]
    internal class SequenceStepParameter : ISequenceStepParameter
    {
        public SequenceStepParameter()
        {
            this.Index = Constants.UnverifiedTypeIndex;
            this.SubStepParameters = null;
            this.Parameters = null;
            this.Instance = string.Empty;
            this.Return = string.Empty;
        }

        public int Index { get; set; }
        public IList<ISequenceStepParameter> SubStepParameters { get; set; }
        public IParameterDataCollection Parameters { get; set; }
        public string Instance { get; set; }
        public string Return { get; set; }
        public ISequenceDataContainer Clone()
        {
            SequenceStepParameterCollection subStepParameters = null;
            if (null != this.SubStepParameters)
            {
                subStepParameters = new SequenceStepParameterCollection();
                Common.Utility.CloneDataCollection(this.SubStepParameters, subStepParameters);
            }

            ParameterDataCollection dataCollection = null;
            if (null != Parameters)
            {
                dataCollection = new ParameterDataCollection();
                Common.Utility.CloneCollection(Parameters, dataCollection);
            }

            SequenceStepParameter stepParameter = new SequenceStepParameter()
            {
                Index = Constants.UnverifiedIndex,
                SubStepParameters = subStepParameters,
                Parameters = dataCollection,
                Instance = string.Empty,
                Return = string.Empty
            };
            return stepParameter;
        }

        public void Initialize(ISequenceFlowContainer parent)
        {
            ISequenceStep sequenceStep = parent as ISequenceStep;
            if (null != sequenceStep.SubSteps && sequenceStep.SubSteps.Count > 0)
            {
                this.SubStepParameters = new SequenceStepParameterCollection();
                foreach (ISequenceStep subStep in sequenceStep.SubSteps)
                {
                    SequenceStepParameter stepParameter = new SequenceStepParameter();
                    stepParameter.Initialize(subStep);
                    this.SubStepParameters.Add(stepParameter);
                }
            }
            else
            {
                this.SubStepParameters = null;
            }

            IFunctionData functionData = sequenceStep.Function;
            if (null != functionData.Parameters && functionData.Parameters.Count > 0)
            {
                this.Parameters = functionData.Parameters;
            }
            else
            {
                this.Parameters = null;
            }

            this.Return = functionData.Return;
            this.Instance = functionData.Instance;
        }
    }
}