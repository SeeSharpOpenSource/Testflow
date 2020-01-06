using System.Linq;
using Testflow.CoreCommon;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
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

//        protected override void CheckSequenceData()
//        {
//            if (null != StepData.Function)
//            {
//                Context.LogSession.Print(LogLevel.Error, Context.SessionId, 
//                    $"Conditionblock cannot configure function data. Step:{StepData.Name}");
//                I18N i18N = I18N.GetInstance(Constants.I18nName);
//                throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetStr("ConditionBlockHasFunc"));
//
//            }
//            if (null != StepData.SubSteps &&
//                StepData.SubSteps.Any(item => item.StepType != SequenceStepType.ConditionStatement))
//            {
//                Context.LogSession.Print(LogLevel.Error, Context.SessionId, 
//                    $"Some steps under condition block is not ConditionStatement type. Step:{GetStack()}");
//                I18N i18N = I18N.GetInstance(Constants.I18nName);
//                throw new TestflowDataException(ModuleErrorCode.SequenceDataError, i18N.GetStr("IllegalStepType"));
//            }
//        }

        protected override void InvokeStepSingleTime(bool forceInvoke)
        {
            // 重置计时时间
            Actuator.ResetTiming();

            // 调用前置监听
            OnPreListener();

            // 开始计时
            Actuator.StartTiming();
            // 停止计时
            Actuator.EndTiming();
            this.Result = StepResult.Pass;
            // 如果当前step被标记为记录状态，则返回状态信息
            if (null != StepData && StepData.RecordStatus)
            {
                RecordRuntimeStatus();
            }

            // 调用后置监听
            OnPostListener();

            if (null != StepData && StepData.HasSubSteps)
            {
                StepTaskEntityBase subStepEntity = SubStepRoot;
                do
                {
                    subStepEntity.Invoke(forceInvoke);
                    object returnValue = subStepEntity.Return;
                    // 如果ConditionStatement返回值为True则说明该分支已执行完成，则跳过后续的Step
                    if (returnValue is bool && (bool)returnValue)
                    {
                        break;
                    }
                } while (null != (subStepEntity = subStepEntity.NextStep));
            }
        }
    }
}