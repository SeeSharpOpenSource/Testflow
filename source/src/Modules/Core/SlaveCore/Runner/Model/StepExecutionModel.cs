using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Messages;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class StepExecutionModel : StepModelBase
    {
        #region 序列功能标志

        public bool HasLoopCount { get; }

        public bool HasRetryCount { get; }

        public FunctionType FunctionType { get; }

        #endregion

        #region 子序列属性

        public List<StepExecutionModel> SubSteps { get; }

        #endregion


        #region 方法属性

        public Type ClassType { get; set; }

        public MethodInfo Method { get; set; }

        public ConstructorInfo Constructor { get; set; }

        public object[] Params { get; }

        public string InstanceVar { get; set; }

        public string ReturnVar { get; set; }

        #endregion


        #region 计数相关

        public int LoopCount { get; set; }

        public int MaxLoopCount { get; }

        public string LoopVar { get; }

        public int RetryCount { get; set; }

        public int MaxRetryCount { get; }

        public string RetryVar { get; }

        #endregion

        public StepExecutionModel(ISequenceStep step, SlaveContext context) : base(step, context)
        {
            this.HasLoopCount = false;
            this.HasRetryCount = false;
//            this.RecordStatus = step.RecordStatus;
//            this.Behavior = step.Behavior;
//            this.BreakIfFailed = step.BreakIfFailed;
            this.FunctionType = step.Function.Type;

            int session = context.SessionId;
            this.LoopCount = 0;
            this.RetryCount = 0;

            if (CoreUtils.IsValidVaraible(step.Function.Instance))
            {
                this.InstanceVar = GetVariableFullName(step.Function.Instance, step, session);
            }
            if (CoreUtils.IsValidVaraible(step.Function.Return))
            {
                this.RetryVar = GetVariableFullName(step.Function.Return, step, session);
            }
            this.ReturnVar = GetVariableFullName(InstanceVar, step, session);

            if (null != step.LoopCounter && step.LoopCounter.MaxValue > 1 && step.LoopCounter.CounterEnabled)
            {
                this.HasLoopCount = true;
                this.MaxLoopCount = step.LoopCounter.MaxValue;
                this.LoopVar = GetVariableFullName(step.LoopCounter.CounterVariable, step, session);
            }

            if (null != step.RetryCounter && step.RetryCounter.MaxRetryTimes > 1 && step.RetryCounter.RetryEnabled)
            {
                this.HasRetryCount = true;
                this.MaxRetryCount = step.RetryCounter.MaxRetryTimes;
                this.RetryVar = GetVariableFullName(LoopVar, step, session);
            }

            if (StepData.HasSubSteps)
            {
                this.SubSteps = new List<StepExecutionModel>(step.SubSteps.Count);
                foreach (ISequenceStep subStep in step.SubSteps)
                {
                    SubSteps.Add(new StepExecutionModel(subStep, context));
                }
            }
            else
            {
                this.Method = null;
                this.Params = new object[step.Function.Parameters.Count];
                this.Constructor = null;
            }
        }

        private string GetVariableFullName(string variableName, ISequenceStep step, int session)
        {
            while (step.Parent is ISequenceStep)
            {
                step = (ISequenceStep)step.Parent;
            }
            ISequence sequence = (ISequence) step.Parent;
            IVariable variable = sequence.Variables.FirstOrDefault(item => item.Name.Equals(variableName));

            if (null != variable)
            {
                return CoreUtils.GetRuntimeVariableName(session, variable);
            }
            variable = sequence.Variables.First(item => item.Name.Equals(variableName));
            return CoreUtils.GetRuntimeVariableName(session, variable);
        }

        /// <summary>
        /// 当指定时间内该序列没有额外信息到达时传递运行时状态的信息
        /// </summary>
        public override void FillStatusInfo(StatusMessage statusMessage)
        {
            statusMessage.Stacks.Add(GetStack());
            statusMessage.Results.Add(StepResult.NotAvailable);
        }

        public override void Invoke()
        {
            if (!HasLoopCount)
            {
                ExecuteStepSingleTime();
            }
            else
            {
                LoopCount = 0;
                do
                {
                    if (CoreUtils.IsValidVaraible(LoopVar))
                    {
                        Context.VariableMapper.SetVariableValue(LoopVar, LoopCount);
                    }

                } while (++LoopCount < MaxLoopCount);
            }
        }

        private void ExecuteStepSingleTime()
        {
            if (StepData.HasSubSteps)
            {
                foreach (StepExecutionModel subStep in SubSteps)
                {
                    subStep.Invoke();
                }
            }
            object instance;
            object returnValue;
            InitializeParamValue();
            switch (FunctionType)
            {
                case FunctionType.Constructor:
                    instance = Constructor.Invoke(Params);
                    if (CoreUtils.IsValidVaraible(InstanceVar))
                    {
                        Context.VariableMapper.SetVariableValue(InstanceVar, instance);
                    }
                    break;
                case FunctionType.InstanceFunction:
                    instance = Context.VariableMapper.GetVariableValue(InstanceVar);
                    returnValue = Method.Invoke(instance, Params);
                    if (CoreUtils.IsValidVaraible(ReturnVar))
                    {
                        Context.VariableMapper.SetVariableValue(ReturnVar, returnValue);
                    }
                    break;
                case FunctionType.StaticFunction:
                    returnValue = Method.Invoke(null, Params);
                    if (CoreUtils.IsValidVaraible(ReturnVar))
                    {
                        Context.VariableMapper.SetVariableValue(ReturnVar, returnValue);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (StepData.RecordStatus)
            {
                SequenceStatusInfo statusInfo = new SequenceStatusInfo(StepData.Index, this.GetStack(), StatusReportType.Record);
                Context.StatusQueue.Enqueue(statusInfo);
            }
        }

        private void InitializeParamValue()
        {
            throw new NotImplementedException();
        }
    }
}