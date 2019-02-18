using Testflow.Common;
using Testflow.Data.Sequence;
using Testflow.Modules;
using Testflow.SequenceManager.Common;
using Testflow.SequenceManager.SequenceElements;
using Testflow.Utility.I18nUtil;

namespace Testflow.SequenceManager.ParameterManager
{
    public abstract class ParameterManagerBase
    {
        protected readonly IModuleConfigData ConfigData;
        protected ParameterManagerBase(IModuleConfigData configData)
        {
            this.ConfigData = configData;
        }

        #region 填充参数配置信息到序列

        protected void SetParameterToSequenceGroup(SequenceGroup sequenceGroup, ISequenceGroupParameter parameter,
            bool forceLoad)
        {
            sequenceGroup.RefreshSignature();
            if (!sequenceGroup.Info.Hash.Equals(parameter.Info.Hash) && !forceLoad)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedParameter,
                    i18N.GetStr("UnmatchedHash"));
            }
            SetParameterToSequence(sequenceGroup.SetUp, parameter.SetUpParameters);
            SetParameterToSequence(sequenceGroup.TearDown, parameter.TearDownParameters);
            foreach (ISequence sequence in sequenceGroup.Sequences)
            {
                SetParameterToSequence(sequence, parameter.SequenceParameters[sequence.Index]);
            }
        }

        private void SetParameterToSequence(ISequence sequence, ISequenceParameter parameter)
        {
            if (sequence.Steps.Count != parameter.StepParameters.Count ||
                sequence.Variables.Count != parameter.VariableValues.Count)
            {
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedParameter,
                    i18N.GetStr("UnmatchedData"));
            }
            SetVariableValues(sequence, parameter);
            foreach (ISequenceStep sequenceStep in sequence.Steps)
            {
                SetParameterToSequenceStep(sequenceStep, parameter.StepParameters[sequenceStep.Index]);
            }
        }

        private static void SetVariableValues(ISequence sequence, ISequenceParameter parameter)
        {
            for (int i = 0; i < sequence.Variables.Count; i++)
            {
                IVariable variable = sequence.Variables[i];
                if (!variable.Name.Equals(parameter.VariableValues[i].Value))
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedParameter,
                        i18N.GetStr("UnmatchedData"));
                }
                variable.Value = parameter.VariableValues[i].Value;
            }
        }

        private void SetParameterToSequenceStep(ISequenceStep sequenceStep, ISequenceStepParameter parameter)
        {
            if (sequenceStep.HasSubSteps)
            {
                if (sequenceStep.SubSteps.Count != parameter.SubStepParameters.Count)
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedParameter,
                        i18N.GetStr("UnmatchedData"));
                }
                foreach (ISequenceStep subStep in sequenceStep.SubSteps)
                {
                    SetParameterToSequenceStep(subStep, parameter.SubStepParameters[subStep.Index]);
                }
            }
            else
            {
                if (sequenceStep.Function.ParameterType.Count != parameter.Parameters.Count ||
                    (null != parameter.SubStepParameters
                     && 0 != parameter.SubStepParameters.Count))
                {
                    I18N i18N = I18N.GetInstance(Constants.I18nName);
                    throw new TestflowDataException(SequenceManagerErrorCode.UnmatchedParameter,
                        i18N.GetStr("UnmatchedData"));
                }
                sequenceStep.Function.Parameters = parameter.Parameters;
                sequenceStep.Function.Return = parameter.Return;
                sequenceStep.Function.Instance = parameter.Instance;
            }
        }

        #endregion

        #region 从序列中生成独立的参数配置信息

        protected void GenerateParameterData(SequenceGroup sequenceGroup)
        {
            SequenceGroupParameter parameter = sequenceGroup.Parameters as SequenceGroupParameter;
            if (null == parameter)
            {
                parameter = new SequenceGroupParameter();
                parameter.Initialize(sequenceGroup);
                
            }
            


        }

        #endregion
    }



}