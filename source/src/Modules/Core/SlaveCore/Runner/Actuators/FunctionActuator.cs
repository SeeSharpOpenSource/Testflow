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
using Testflow.SlaveCore.Runner.Expression;
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

        private MethodInfo _method;

        private ConstructorInfo _constructor;

        private Assembly _structAssembly;

        private object[] _params;

        private string _instanceVar;

        private string _returnVar;

        #endregion

        public FunctionActuator(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
            int session = context.SessionId;

            this._method = null;
            this._params = new object[step.Function.Parameters?.Count ?? 0];
            this._constructor = null;
            this.FunctionType = step.Function.Type;

            if (CoreUtils.IsValidVaraible(step.Function.Instance))
            {
                string variableName = ModuleUtils.GetVariableNameFromParamValue(step.Function.Instance);
                this._instanceVar = ModuleUtils.GetVariableFullName(variableName, step, session);
            }
            if (CoreUtils.IsValidVaraible(step.Function.Return))
            {
                string variableName = ModuleUtils.GetVariableNameFromParamValue(step.Function.Return);
                this._returnVar = ModuleUtils.GetVariableFullName(variableName, step, session);
            }
        }

        protected override void GenerateInvokeInfo()
        {
            switch (FunctionType)
            {
                case FunctionType.StaticFunction:
                case FunctionType.InstanceFunction:
                    this._method = Context.TypeInvoker.GetMethod(Function);
                    if (null == _method)
                    {
                        throw new TestflowRuntimeException(ModuleErrorCode.RuntimeError,
                            Context.I18N.GetFStr("LoadFunctionFailed", Function.MethodName));
                    }
                    break;
                case FunctionType.Constructor:
                    this._constructor = Context.TypeInvoker.GetConstructor(Function);
                    if (null == _constructor)
                    {
                        throw new TestflowRuntimeException(ModuleErrorCode.RuntimeError,
                            Context.I18N.GetFStr("LoadFunctionFailed", Function.MethodName));
                    }
                    break;
                case FunctionType.StructConstructor:
                    Type classType = Context.TypeInvoker.GetType(Function.ClassType);
                    _structAssembly = classType.Assembly;
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
                switch (parameters[i].ParameterType)
                {
                    case ParameterType.Value:
                        _params[i] = Context.TypeInvoker.CastConstantValue(argumentInfos[i].Type, paramValue);
                        break;
                    case ParameterType.Variable:
                        // 如果是变量，则先获取对应的Varaible变量，真正的值在运行时才更新获取
                        string variableName = ModuleUtils.GetVariableNameFromParamValue(paramValue);
                        IVariable variable = ModuleUtils.GetVaraibleByRawVarName(variableName, StepData);
                        if (null == variable)
                        {
                            Context.LogSession.Print(LogLevel.Error, Context.SessionId,
                                $"Unexist variable '{variableName}' in sequence data.");
                            throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                                Context.I18N.GetFStr("UnexistVariable", variableName));
                        }
                        // 将变量的值保存到Parameter中
                        string varFullName = CoreUtils.GetRuntimeVariableName(Context.SessionId, variable);
                        parameters[i].Value = ModuleUtils.GetFullParameterVariableName(varFullName, parameters[i].Value);
                        _params[i] = null;
                        break;
                    case ParameterType.NotAvailable:
                        // 如果参数的修饰符为out，则可以不配置
                        if (argumentInfos[i].Modifier != ArgumentModifier.Out)
                        {
                            Context.LogSession.Print(LogLevel.Error, Context.SessionId,
                                $"The value of parameter '{argumentInfos[i].Name}' in step '{StepData.Name}' is not configured");
                            throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                                    Context.I18N.GetFStr("UnconfiguredParam", argumentInfos[i].Name));
                        }
                        break;
                    case ParameterType.Expression:
                        ExpressionProcessor expProcessor =
                            Context.CoroutineManager.GetCoroutineHandle(CoroutineId).ExpressionProcessor;
                        int expIndex = expProcessor.CompileExpression(paramValue, StepData);
                        // 在参数数据中写入表达式索引
                        parameters[i].Value = expIndex.ToString();
                        break;
                    default:
                        throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                                Context.I18N.GetStr("InvalidParamVar"));
                        break;
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
                _returnVar = CoreUtils.GetRuntimeVariableName(Context.SessionId, variable);
            }
            CommonStepDataCheck(_instanceVar);
        }

        public override StepResult InvokeStep(bool forceInvoke)
        {
            // 计算非常量参数的值
            SetNonconstantParamValue();
            // 调用方法
            InvokeFunction();
            // 更新所有被ref修饰的变量类型的值
            UpdateParamVariableValue();
            // 清除非常量参数的值
            ClearNonconstantParamValue();
            return StepResult.Pass;
        }
        
        private void SetNonconstantParamValue()
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
                    _params[i] = Context.VariableMapper.GetParamValue(variableName, parameters[i].Value,
                        arguments[i].Type);
                }
                else if (parameters[i].ParameterType == ParameterType.Expression)
                {
                    int expIndex = int.Parse(parameters[i].Value);
                    ExpressionProcessor expProcessor =
                        Context.CoroutineManager.GetCoroutineHandle(CoroutineId).ExpressionProcessor;
                    _params[i] = expProcessor.Calculate(expIndex, arguments[i].Type);
                }
            }
        }

        private void InvokeFunction()
        {
            object instance;
            object returnValue = null;
            switch (FunctionType)
            {
                case FunctionType.Constructor:
                    // 开始计时
                    StartTiming();
                    instance = _constructor.Invoke(_params);
                    // 停止计时
                    EndTiming();
                    if (CoreUtils.IsValidVaraible(_instanceVar))
                    {
                        Context.VariableMapper.SetParamValue(_instanceVar, Function.Instance, instance);
                        LogTraceVariable(Function.Instance, _instanceVar);
                    }
                    break;
                case FunctionType.StructConstructor:
                    // 开始计时
                    StartTiming();
                    instance = _structAssembly.CreateInstance(ModuleUtils.GetTypeFullName(Function.ClassType));
                    // 停止计时
                    EndTiming();
                    if (CoreUtils.IsValidVaraible(_instanceVar))
                    {
                        Context.VariableMapper.SetParamValue(_instanceVar, Function.Instance, instance);
                        LogTraceVariable(Function.Instance, _instanceVar);
                    }
                    break;
                case FunctionType.InstanceFunction:
                    instance = Context.VariableMapper.GetParamValue(_instanceVar, Function.Instance,
                        Function.ClassType);
                    // 开始计时
                    StartTiming();
                    returnValue = _method.Invoke(instance, _params);
                    // 停止计时
                    EndTiming();
                    if (CoreUtils.IsValidVaraible(_returnVar))
                    {
                        Context.VariableMapper.SetParamValue(_returnVar, Function.Return, returnValue);
                        LogTraceVariable(Function.Return, returnValue);
                    }
                    break;
                case FunctionType.StaticFunction:
                    // 开始计时
                    StartTiming();
                    returnValue = _method.Invoke(null, _params);
                    // 停止计时
                    EndTiming();
                    if (CoreUtils.IsValidVaraible(_returnVar))
                    {
                        Context.VariableMapper.SetParamValue(_returnVar, Function.Return, returnValue);
                        LogTraceVariable(Function.Return, returnValue);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            this.Return = returnValue;
        }

        // 因为Variable的值在整个过程中会变化，所以需要在运行前实时获取

        // 更新所有被ref或out修饰的参数值。如果变量的LogRecordLevel为Trace，则将更新的值写入日志。
        private void UpdateParamVariableValue()
        {
            for (int i = 0; i < _params.Length; i++)
            {
                IArgument argument = Function.ParameterType[i];
                IParameterData parameter = Function.Parameters[i];
                // 如果参数值不是变量赋值，或者参数没有使用ref或out修饰，则返回
                if (parameter.ParameterType != ParameterType.Variable || argument.Modifier == ArgumentModifier.None)
                {
                    continue;
                }
                object value = _params[i];
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
            Context.LogSession.Print(LogLevel.Debug, Context.SessionId, printStr);
        }

        private void ClearNonconstantParamValue()
        {
            IParameterDataCollection parameters = Function.Parameters;
            if (null == parameters)
            {
                return;
            }
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].ParameterType == ParameterType.Variable || parameters[i].ParameterType == ParameterType.Expression)
                {
                    // 清空非常量参数的值
                    _params[i] = null;
                }
            }
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