using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Testflow.Usr;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Data;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class StepExecutionEntity : StepTaskEntityBase
    {
        #region 序列功能标志

        public bool HasLoopCount { get; }

        public bool HasRetryCount { get; }

        public FunctionType FunctionType { get; }

        #endregion

        #region 子序列属性

        public StepTaskEntityBase SubStepRoot { get; }

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

        public StepExecutionEntity(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
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
//            this.ReturnVar = GetVariableFullName(InstanceVar, step, session);

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
                this.SubStepRoot = ModuleUtils.CreateSubStepModelChain(StepData.SubSteps, Context, sequenceIndex);
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
            ISequenceGroup sequenceGroup = (ISequenceGroup)sequence.Parent;
            variable = sequenceGroup.Variables.First(item => item.Name.Equals(variableName));
            return CoreUtils.GetRuntimeVariableName(session, variable);
        }

        public override void GenerateInvokeInfo()
        {
            if (StepData.HasSubSteps)
            {
                this.SubStepRoot.NextStep.GenerateInvokeInfo();
            }
            else
            {
                switch (FunctionType)
                {
                    case FunctionType.StaticFunction:
                    case FunctionType.InstanceFunction:
                        this.Method = Context.TypeInvoker.GetMethod(StepData.Function);
                        break;
                    case FunctionType.Constructor:
                        this.Constructor = Context.TypeInvoker.GetConstructor(StepData.Function);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            NextStep?.GenerateInvokeInfo();
        }

        public override void InitializeParamsValues()
        {
            if (StepData.HasSubSteps)
            {
                SubStepRoot.InitializeParamsValues();
            }
            else
            {
                IArgumentCollection argumentInfos = StepData.Function.ParameterType;
                IParameterDataCollection parameters = StepData.Function.Parameters;
                for (int i = 0; i < argumentInfos.Count; i++)
                {
                    string paramValue = parameters[i].Value;
                    if (parameters[i].ParameterType == ParameterType.Value)
                    {
                        Params[i] = Context.TypeInvoker.CastValue(argumentInfos[i].Type, paramValue);
                    }
                    else
                    {
                        // 如果是变量，则先获取对应的Varaible变量，真正的值在运行时才更新获取
                        string variableName = ModuleUtils.GetVariableNameFromParamValue(paramValue);
                        IVariable variable = ModuleUtils.GetVaraibleByRawVarName(variableName, StepData);
                        if (null == variable)
                        {
                            Context.LogSession.Print(LogLevel.Error, SequenceIndex,
                                $"Unexist variable '{variableName}' in sequence data.");
                            throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                                Context.I18N.GetFStr("UnexistVariable", variableName));

                        }
                        Params[i] = variable;
                    }
                }
                if (null != StepData.Function.ReturnType && CoreUtils.IsValidVaraible(StepData.Function.Return))
                {
                    // 如果是变量，则先获取对应的Varaible变量，真正的值在运行时才更新获取
                    string variableName = ModuleUtils.GetVariableNameFromParamValue(StepData.Function.Return);
                    IVariable variable = ModuleUtils.GetVaraibleByRawVarName(variableName, StepData);
                    if (null == variable)
                    {
                        Context.LogSession.Print(LogLevel.Error, SequenceIndex,
                            $"Unexist variable '{variableName}' in sequence data.");
                        throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                            Context.I18N.GetFStr("UnexistVariable", variableName));
                    }
                    ReturnVar = CoreUtils.GetRuntimeVariableName(Context.SessionId, variable);
                }
            }
            NextStep?.InitializeParamsValues();
        }

        protected override void InvokeStep()
        {
            this.Result = StepResult.Error;
            try
            {
                switch (StepData.Behavior)
                {
                    case RunBehavior.Normal:
                        ExecuteSequenceStep();
                        this.Result = StepResult.Pass;
                        break;
                    case RunBehavior.Skip:
                        this.Result = StepResult.Skip;
                        break;
                    case RunBehavior.ForceSuccess:
                        try
                        {
                            ExecuteSequenceStep();
                            this.Result = StepResult.Pass;
                        }
                        catch (TaskFailedException ex)
                        {
                            this.Result = StepResult.Failed;
                            Context.LogSession.Print(LogLevel.Warn, SequenceIndex, ex,
                                "Execute failed but force success.");
                        }
                        break;
                    case RunBehavior.ForceFailed:
                        ExecuteSequenceStep();
                        this.Result = StepResult.Failed;
                        TestflowAssertException exception = new TestflowAssertException(Context.I18N.GetStr("StepForceFailed"));
                        SequenceStatusInfo info = new SequenceStatusInfo(SequenceIndex, this.GetStack(),
                            StatusReportType.Failed, this.Result, exception);
                        Context.StatusQueue.Enqueue(info);
                        throw new TaskFailedException(SequenceIndex);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            finally
            {
                // 如果当前step被标记为记录状态，并且不是强制失败，则返回状态信息
                if (StepData.RecordStatus && RunBehavior.ForceFailed != StepData.Behavior)
                {
                    SequenceStatusInfo statusInfo = new SequenceStatusInfo(StepData.Index, this.GetStack(), 
                        StatusReportType.Record, Result);
                    // 更新watch变量值
                    statusInfo.WatchDatas = Context.VariableMapper.GetWatchDataValues(StepData);
                    Context.StatusQueue.Enqueue(statusInfo);
                }
            }

            NextStep?.Invoke();
        }

        private void ExecuteSequenceStep()
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
                    ExecuteStepSingleTime();
                    if (CoreUtils.IsValidVaraible(LoopVar))
                    {
                        Context.VariableMapper.SetParamValue(LoopVar, StepData.LoopCounter.CounterVariable, LoopCount,
                            StepData.RecordStatus);
                    }
                } while (++LoopCount < MaxLoopCount);
            }
        }

        private void ExecuteStepSingleTime()
        {
            if (StepData.HasSubSteps)
            {
                SubStepRoot.Invoke();
            }
            else
            {
                object instance;
                object returnValue;
                SetVariableParamValue();
                switch (FunctionType)
                {
                    case FunctionType.Constructor:
                        instance = Constructor.Invoke(Params);
                        if (CoreUtils.IsValidVaraible(InstanceVar))
                        {
                            Context.VariableMapper.SetParamValue(InstanceVar, StepData.Function.Instance, instance,
                                StepData.RecordStatus);
                            LogTraceVariable(StepData.Function.Instance, InstanceVar);
                        }
                        break;
                    case FunctionType.InstanceFunction:
                        instance = Context.VariableMapper.GetParamValue(InstanceVar, StepData.Function.Instance);
                        returnValue = Method.Invoke(instance, Params);
                        if (CoreUtils.IsValidVaraible(ReturnVar))
                        {
                            Context.VariableMapper.SetParamValue(ReturnVar, StepData.Function.Return, returnValue,
                                StepData.RecordStatus);
                            LogTraceVariable(StepData.Function.Return, returnValue);
                        }
                        break;
                    case FunctionType.StaticFunction:
                        returnValue = Method.Invoke(null, Params);
                        if (CoreUtils.IsValidVaraible(ReturnVar))
                        {
                            Context.VariableMapper.SetParamValue(ReturnVar, StepData.Function.Return, returnValue,
                                StepData.RecordStatus);
                            LogTraceVariable(StepData.Function.Return, returnValue);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                // 更新所有被ref修饰的变量类型的值
                UpdateParamVariableValue();
            }
        }

        // 因为Variable的值在整个过程中会变化，所以需要在运行前实时获取
        private void SetVariableParamValue()
        {
            IParameterDataCollection parameters = StepData.Function.Parameters;
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].ParameterType == ParameterType.Variable)
                {
                    // 获取变量值的名称
                    string variableName = CoreUtils.GetRuntimeVariableName(Context.SessionId, (IVariable) Params[i]);
                    // 使用变量名称获取变量当前对象的值
                    object variableValue = Context.VariableMapper.GetParamValue(variableName, parameters[i].Value);
                    // 根据ParamString和变量对应的值配置参数。
                    Params[i] = ModuleUtils.GetParamValue(parameters[i].Value, variableValue);
                }
            }
        }

        // 更新所有被ref或out修饰的参数值。如果变量的LogRecordLevel为Trace，则将更新的值写入日志。
        private void UpdateParamVariableValue()
        {
            for (int i = 0; i < Params.Length; i++)
            {
                IArgument argument = StepData.Function.ParameterType[i];
                IParameterData parameter = StepData.Function.Parameters[i];
                // 如果参数值是直接传递值，或者参数没有使用ref或out修饰，则返回
                if (parameter.ParameterType == ParameterType.Value || argument.Modifier == ArgumentModifier.None)
                {
                    return;
                }
                object value = Params[i];
                string variableName = ModuleUtils.GetVariableNameFromParamValue(parameter.Value);
                IVariable variable = ModuleUtils.GetVaraibleByRawVarName(variableName, StepData);
                string runtimeVariableName = CoreUtils.GetRuntimeVariableName(Context.SessionId, variable);
                Context.VariableMapper.SetParamValue(runtimeVariableName, parameter.Value, value, 
                    StepData.RecordStatus);
                if (variable.LogRecordLevel == RecordLevel.Trace)
                {
                    LogTraceVariable(variable, value);
                }
            }
        }

        private void LogTraceVariable(IVariable variable, object value)
        {
            const string variableLogFormat = "[Variable Trace] Name:{0}, Stack:{1}, Value: {2}.";
            string stackStr = GetStack().ToString();
            string varValueStr;
            if (null != value)
            {
                varValueStr = variable.VariableType == VariableType.Class
                    ? JsonConvert.SerializeObject(value)
                    : value.ToString();
            }
            else
            {
                varValueStr = CoreConstants.NullValue;
            }
            string printStr = string.Format(variableLogFormat, variable.Name, stackStr, varValueStr);
            Context.LogSession.Print(LogLevel.Info, Context.SessionId, printStr);
        }

        private void LogTraceVariable(string varString, object value)
        {
            string variableName = ModuleUtils.GetVariableNameFromParamValue(varString);
            IVariable variable = ModuleUtils.GetVaraibleByRawVarName(variableName, StepData);
            if (variable.LogRecordLevel == RecordLevel.Trace)
            {
                LogTraceVariable(variable, value);
            }
        }
    }
}