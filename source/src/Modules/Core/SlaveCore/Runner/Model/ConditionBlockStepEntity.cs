using System.Linq;
using Testflow.CoreCommon;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;
using Testflow.Usr;
using Testflow.Utility.I18nUtil;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class ConditionBlockStepEntity : StepTaskEntityBase
    {
        public ConditionBlockStepEntity(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
        }

        protected override void CheckSequenceData()
        {
            if (null != StepData.Function)
            {
                Context.LogSession.Print(LogLevel.Error, Context.SessionId, 
                    $"Conditionblock cannot configure function data. Step:{StepData.Name}");
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetStr("ConditionBlockHasFunc"));

            }
            if (null != StepData.SubSteps &&
                StepData.SubSteps.Any(item => item.StepType != SequenceStepType.ConditionStatement))
            {
                Context.LogSession.Print(LogLevel.Error, Context.SessionId, 
                    $"Some steps under condition block is not ConditionStatement type. Step:{GetStack()}");
                I18N i18N = I18N.GetInstance(Constants.I18nName);
                throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetStr("IllegalStepType"));
            }
        }

        protected override void InvokeStepSingleTime(bool forceInvoke)
        {
            throw new System.NotImplementedException();
        }
    }
}