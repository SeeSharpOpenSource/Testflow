using System;
using System.Reflection;
using Newtonsoft.Json;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.CoreCommon.Data;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Model;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Actuators
{
    internal class FunctionActuator : ActuatorBase
    {
        #region 序列功能标志

        public FunctionType FunctionType { get; }

        #endregion

        #region 方法属性

        public MethodInfo Method { get; set; }

        public ConstructorInfo Constructor { get; set; }

        public Assembly StructAssembly { get; set; }

        public object[] Params { get; }

        public string InstanceVar { get; set; }

        public string ReturnVar { get; set; }

        #endregion

        public FunctionActuator(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
            int session = context.SessionId;

            this.Method = null;
            this.Params = new object[step.Function.Parameters?.Count ?? 0];
            this.Constructor = null;
            this.FunctionType = step.Function.Type;

            if (CoreUtils.IsValidVaraible(step.Function.Instance))
            {
                string variableName = ModuleUtils.GetVariableNameFromParamValue(step.Function.Instance);
                this.InstanceVar = ModuleUtils.GetVariableFullName(variableName, step, session);
            }
            if (CoreUtils.IsValidVaraible(step.Function.Return))
            {
                string variableName = ModuleUtils.GetVariableNameFromParamValue(step.Function.Return);
                this.ReturnVar = ModuleUtils.GetVariableFullName(variableName, step, session);
            }
        }

        protected override void GenerateInvokeInfo()
        {
            switch (FunctionType)
            {
                case FunctionType.StaticFunction:
                case FunctionType.InstanceFunction:
                    this.Method = Context.TypeInvoker.GetMethod(Function);
                    if (null == Method)
                    {
                        throw new TestflowRuntimeException(ModuleErrorCode.RuntimeError,
                            Context.I18N.GetFStr("LoadFunctionFailed", Function.MethodName));
                    }
                    break;
                case FunctionType.Constructor:
                    this.Constructor = Context.TypeInvoker.GetConstructor(Function);
                    if (null == Constructor)
                    {
                        throw new TestflowRuntimeException(ModuleErrorCode.RuntimeError,
                            Context.I18N.GetFStr("LoadFunctionFailed", Function.MethodName));
                    }
                    break;
                case FunctionType.StructConstructor:
                    Type classType = Context.TypeInvoker.GetType(Function.ClassType);
                    StructAssembly = classType.Assembly;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override void InitializeParamsValues()
        {
            IArgumentCollection argumentInfos = Function.ParameterType;
            IParameterDataCollection parameters = Function.Parameters;
            for (int i = 0; i < argumentInfos.Count; i++)
            {
                string paramValue = parameters[i].Value;
                if (parameters[i].ParameterType == ParameterType.Value)
                {
                    Params[i] = Context.TypeInvoker.CastConstantValue(argumentInfos[i].Type, paramValue);
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
                    // 将变量的值保存到Parameter中
                    string varFullName = CoreUtils.GetRuntimeVariableName(Context.SessionId, variable);
                    parameters[i].Value = ModuleUtils.GetFullParameterVariableName(varFullName, parameters[i].Value);
                    Params[i] = null;
                }
            }
            if (null != Function.ReturnType && CoreUtils.IsValidVaraible(Function.Return))
            {
                // 如果是变量，则先获取对应的Varaible变量，真正的值在运行时才更新获取
                string variableName = ModuleUtils.GetVariableNameFromParamValue(Function.Return);
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

        public override StepResult InvokeStep(bool forceInvoke)
        {
            object instance;
            object returnValue = null;
            SetVariableParamValue();
            switch (FunctionType)
            {
                case FunctionType.Constructor:
                    // 开始计时
                    StartTiming();
                    instance = Constructor.Invoke(Params);
                    // 停止计时
                    EndTiming();
                    if (CoreUtils.IsValidVaraible(InstanceVar))
                    {
                        Context.VariableMapper.SetParamValue(InstanceVar, Function.Instance, instance);
                        LogTraceVariable(Function.Instance, InstanceVar);
                    }
                    break;
                case FunctionType.StructConstructor:
                    // 开始计时
                    StartTiming();
                    instance = StructAssembly.CreateInstance(ModuleUtils.GetTypeFullName(Function.ClassType));
                    // 停止计时
                    EndTiming();
                    if (CoreUtils.IsValidVaraible(InstanceVar))
                    {
                        Context.VariableMapper.SetParamValue(InstanceVar, Function.Instance, instance);
                        LogTraceVariable(Function.Instance, InstanceVar);
                    }
                    break;
                case FunctionType.InstanceFunction:
                    instance = Context.VariableMapper.GetParamValue(InstanceVar, Function.Instance,
                        Function.ClassType);
                    // 开始计时
                    StartTiming();
                    returnValue = Method.Invoke(instance, Params);
                    // 停止计时
                    EndTiming();
                    if (CoreUtils.IsValidVaraible(ReturnVar))
                    {
                        Context.VariableMapper.SetParamValue(ReturnVar, Function.Return, returnValue);
                        LogTraceVariable(Function.Return, returnValue);
                    }
                    break;
                case FunctionType.StaticFunction:
                    // 开始计时
                    StartTiming();
                    returnValue = Method.Invoke(null, Params);
                    // 停止计时
                    EndTiming();
                    if (CoreUtils.IsValidVaraible(ReturnVar))
                    {
                        Context.VariableMapper.SetParamValue(ReturnVar, Function.Return, returnValue);
                        LogTraceVariable(Function.Return, returnValue);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            this.Return = returnValue;
            // 更新所有被ref修饰的变量类型的值
            UpdateParamVariableValue();
            return StepResult.Pass;
        }

        // 因为Variable的值在整个过程中会变化，所以需要在运行前实时获取
        private void SetVariableParamValue()
        {
            IArgumentCollection arguments = Function.ParameterType;
            IParameterDataCollection parameters = Function.Parameters;
            if (null == parameters)
            {
                return;
            }
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].ParameterType == ParameterType.Variable)
                {
                    // 获取变量值的名称，该名称为变量的运行时名称，其值在InitializeParamValue方法里配置
                    string variableName = ModuleUtils.GetVariableNameFromParamValue(parameters[i].Value);
                    // 根据ParamString和变量对应的值配置参数。
                    Params[i] = Context.VariableMapper.GetParamValue(variableName, parameters[i].Value,
                        arguments[i].Type);
                }
            }
        }

        // 更新所有被ref或out修饰的参数值。如果变量的LogRecordLevel为Trace，则将更新的值写入日志。
        private void UpdateParamVariableValue()
        {
            for (int i = 0; i < Params.Length; i++)
            {
                IArgument argument = Function.ParameterType[i];
                IParameterData parameter = Function.Parameters[i];
                // 如果参数值是直接传递值，或者参数没有使用ref或out修饰，则返回
                if (parameter.ParameterType == ParameterType.Value || argument.Modifier == ArgumentModifier.None)
                {
                    continue;
                }
                object value = Params[i];
                // variableName已经是运行时名称
                string runtimeVariableName = ModuleUtils.GetVariableNameFromParamValue(parameter.Value);
                Context.VariableMapper.SetParamValue(runtimeVariableName, parameter.Value, value);
                IVariable variable = CoreUtils.GetVariable(Context.Sequence, runtimeVariableName);
                if (variable.LogRecordLevel == RecordLevel.Trace)
                {
                    LogTraceVariable(variable, value);
                }
            }
        }

        private void LogTraceVariable(IVariable variable, object value)
        {
            const string variableLogFormat = "[Variable Trace] Name:{0}, Stack:{1}, Value: {2}.";
            string stackStr = CallStack.GetStack(Context.SessionId, StepData).ToString();
            string varValueStr;
            if (null != value)
            {
                varValueStr = variable.VariableType == VariableType.Class
                    ? JsonConvert.SerializeObject(value)
                    : value.ToString();
            }
            else
            {
                varValueStr = CommonConst.NullValue;
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