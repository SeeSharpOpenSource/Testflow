using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Testflow.CoreCommon.Common;
using Testflow.Data.Sequence;

namespace Testflow.SlaveCore.Data
{
    internal class ExecutionStep
    {
        #region 序列功能标志

        public bool HasSubStep { get; }

        public bool HasLoopCount { get; }

        public bool HasRetryCount { get; }

        public bool RecordStatus { get; }

        public bool BreakIfFailed { get; }

        public RunBehavior Behavior { get; }

        #endregion


        #region 子序列属性

        public List<ExecutionStep> SubSteps { get; }

        #endregion


        #region 方法属性

        public Type ClassType { get; set; }

        public MethodInfo Method { get; set; }

        public ConstructorInfo Constructor { get; set; }

        public List<object> Params { get; }

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

        public ExecutionStep(int session, ISequenceStep step)
        {
            this.HasSubStep = step.HasSubSteps;
            this.HasLoopCount = false;
            this.HasRetryCount = false;
            this.RecordStatus = step.RecordStatus;
            this.Behavior = step.Behavior;
            this.BreakIfFailed = step.BreakIfFailed;

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

            if (HasSubStep)
            {
                this.SubSteps = new List<ExecutionStep>(step.SubSteps.Count);
                foreach (ISequenceStep subStep in step.SubSteps)
                {
                    SubSteps.Add(new ExecutionStep(session, subStep));
                }
            }
            else
            {
                this.Method = null;
                this.Params = new List<object>(step.Function.Parameters.Count);

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

        public void Invoke()
        {
            // TODO
        }

    }
}