using System;
using System.Collections.Generic;
using System.Reflection;
using Testflow.CoreCommon;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Expression;
using Testflow.SlaveCore.Runner.Model;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Actuators
{
    internal class PropertySetterActuator : ActuatorBase
    {
        public PropertySetterActuator(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
            _properties = new List<PropertyInfo>(step.Function.Parameters.Count);
            _params = new List<object>(step.Function.Parameters.Count);
        }

        protected override void GenerateInvokeInfo()
        {
            BindingFlags bindingFlags = BindingFlags.Public;
            bindingFlags |= (Function.Type == FunctionType.InstancePropertySetter)
                ? BindingFlags.Instance
                : BindingFlags.Static;
            for (int i = 0; i < Function.ParameterType.Count; i++)
            {
                if (Function.Parameters[i].ParameterType == ParameterType.NotAvailable)
                {
                    _properties.Add(null);
                    continue;
                }
                string propertyName = Function.ParameterType[i].Name;
                Type classType = Context.TypeInvoker.GetType(Function.ClassType);
                _properties.Add(classType.GetProperty(propertyName, bindingFlags));
            }

        }

        protected override void InitializeParamsValues()
        {
            string instanceVarName = null;
            if (!string.IsNullOrWhiteSpace(Function.Instance))
            {
                instanceVarName = ModuleUtils.GetVariableNameFromParamValue(Function.Instance);
                _instanceVar = ModuleUtils.GetVariableFullName(instanceVarName, StepData, Context.SessionId);
            }
            IParameterDataCollection parameters = Function.Parameters;
            for (int i = 0; i < _properties.Count; i++)
            {
                string paramValue = parameters[i].Value;
                IArgument argument = Function.ParameterType[i];
                if (null == _properties[i] || string.IsNullOrEmpty(paramValue))
                {
                    _params.Add(null);
                    continue;
                }
                switch (parameters[i].ParameterType)
                {
                    case ParameterType.NotAvailable:
                        _params.Add(null);
                        break;
                    case ParameterType.Value:
                        // 如果是简单类型，则直接转换，如果是类或结构体，则需要在运行时解析
                        _params.Add(Context.TypeInvoker.IsSimpleType(argument.Type)
                            ? Context.TypeInvoker.CastConstantValue(argument.Type, paramValue)
                            : null);
                        break;
                    case ParameterType.Variable:
                        string variableRawName = ModuleUtils.GetVariableNameFromParamValue(paramValue);
                        string varFullName = ModuleUtils.GetVariableFullName(variableRawName, StepData,
                            Context.SessionId);
                        // 将parameter的Value中，变量名称替换为运行时变量名
                        parameters[i].Value = ModuleUtils.GetFullParameterVariableName(varFullName, paramValue);
                        _params.Add(null);
                        break;
                    case ParameterType.Expression:
                        ExpressionProcessor expProcessor =
                            Context.CoroutineManager.GetCoroutineHandle(CoroutineId).ExpressionProcessor;
                        int expIndex = expProcessor.CompileExpression(paramValue, StepData);
                        // 在参数数据中写入表达式索引
                        parameters[i].Value = expIndex.ToString();
                        _params.Add(null);
                        break;
                    default:
                        throw new TestflowDataException(ModuleErrorCode.SequenceDataError,
                                Context.I18N.GetStr("InvalidParamVar"));
                        break;
                }
            }
            CommonStepDataCheck(instanceVarName);
        }

        private readonly List<PropertyInfo> _properties;

        private readonly List<object> _params;

        private string _instanceVar;

        public override StepResult InvokeStep(bool forceInvoke)
        {
            object instance = null;
            if (Function.Type == FunctionType.InstancePropertySetter)
            {
                instance = Context.VariableMapper.GetParamValue(_instanceVar, Function.Instance,
                    Function.ClassType);
            }
            IParameterDataCollection parameters = Function.Parameters;
            IArgumentCollection arguments = Function.ParameterType;
            // 开始计时
            StartTiming();
            for (int i = 0; i < _properties.Count; i++)
            {
                if (null == _properties[i])
                {
                    continue;
                }
                if (parameters[i].ParameterType == ParameterType.Variable)
                {
                    // 获取变量值的名称，该名称为变量的运行时名称，其值在InitializeParamValue方法里配置
                    string variableName = ModuleUtils.GetVariableNameFromParamValue(parameters[i].Value);
                    // 根据ParamString和变量对应的值配置参数。
                    _params[i] = Context.VariableMapper.GetParamValue(variableName, parameters[i].Value,
                        arguments[i].Type);
                    _properties[i].SetValue(instance, _params[i]);
                }
                else if (parameters[i].ParameterType == ParameterType.Expression)
                {
                    int expIndex = int.Parse(parameters[i].Value);
                    ExpressionProcessor expProcessor =
                        Context.CoroutineManager.GetCoroutineHandle(CoroutineId).ExpressionProcessor;
                    _params[i] = expProcessor.Calculate(expIndex, arguments[i].Type);
                    _properties[i].SetValue(instance, _params[i]);
                }
                // 如果参数类型为value且参数值为null且参数配置的字符不为空且参数类型是类或结构体，则需要实时计算该属性或字段的值
                else if (parameters[i].ParameterType == ParameterType.Value && null == _params[i] &&
                         !string.IsNullOrEmpty(parameters[i].Value) &&
                         !Context.TypeInvoker.IsSimpleType(_properties[i].PropertyType))
                {
                    object originalValue = _properties[i].GetValue(instance);
                    _params[i] = Context.TypeInvoker.CastConstantValue(_properties[i].PropertyType, parameters[i].Value,
                        originalValue);
                    // 如果原始值为空，则需要配置Value，否则其参数都已经写入，无需外部更新
                    if (null == originalValue)
                    {
                        _properties[i].SetValue(instance, _params[i]);
                    }
                }
                else
                {
                    _properties[i].SetValue(instance, _params[i]);
                }
            }
            // 停止计时
            EndTiming();
            return StepResult.Pass;
        }
    }
}