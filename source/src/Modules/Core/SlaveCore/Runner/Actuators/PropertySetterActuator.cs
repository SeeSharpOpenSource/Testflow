using System;
using System.Collections.Generic;
using System.Reflection;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;
using Testflow.SlaveCore.Runner.Model;

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
                        _params.Add(Context.TypeInvoker.CastConstantValue(argument.Type,
                            paramValue));
                        break;
                    case ParameterType.Variable:
                        string variableRawName = ModuleUtils.GetVariableNameFromParamValue(paramValue);
                        string varFullName = ModuleUtils.GetVariableFullName(variableRawName, StepData,
                            Context.SessionId);
                        // 将parameter的Value中，变量名称替换为运行时变量名
                        parameters[i].Value = ModuleUtils.GetFullParameterVariableName(varFullName, paramValue);
                        _params.Add(null);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
                }
                _properties[i].SetValue(instance, _params[i]);
            }
            // 停止计时
            EndTiming();
            return StepResult.Pass;
        }
    }
}