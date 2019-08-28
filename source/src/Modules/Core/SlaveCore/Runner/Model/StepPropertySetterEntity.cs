using System;
using System.Collections.Generic;
using System.Reflection;
using Testflow.CoreCommon.Common;
using Testflow.Data;
using Testflow.Data.Sequence;
using Testflow.Runtime.Data;
using Testflow.SlaveCore.Common;

namespace Testflow.SlaveCore.Runner.Model
{
    internal class StepPropertySetterEntity : StepTaskEntityBase
    {
        public StepPropertySetterEntity(ISequenceStep step, SlaveContext context, int sequenceIndex) : base(step, context, sequenceIndex)
        {
            _properties = new List<PropertyInfo>(step.Function.Parameters.Count);
            _params = new List<object>(step.Function.Parameters.Count);
        }

        protected override void GenerateInvokeInfo()
        {
            BindingFlags bindingFlags = BindingFlags.Public;
            bindingFlags |= (StepData.Function.Type == FunctionType.InstancePropertySetter)
                ? BindingFlags.Instance
                : BindingFlags.Static;
            for (int i = 0; i < StepData.Function.ParameterType.Count; i++)
            {
                if (StepData.Function.Parameters[i].ParameterType == ParameterType.NotAvailable)
                {
                    _properties.Add(null);
                    continue;
                }
                string propertyName = StepData.Function.ParameterType[i].Name;
                Type classType = Context.TypeInvoker.GetType(StepData.Function.ClassType);
                _properties.Add(classType.GetProperty(propertyName, bindingFlags));
            }

        }

        protected override void InitializeParamsValues()
        {
            if (!string.IsNullOrWhiteSpace(StepData.Function.Instance))
            {
                string instanceVarName = ModuleUtils.GetVariableNameFromParamValue(StepData.Function.Instance);
                _instanceVar = ModuleUtils.GetVariableFullName(instanceVarName, StepData, Context.SessionId);
            }
            IParameterDataCollection parameters = StepData.Function.Parameters;
            for (int i = 0; i < _properties.Count; i++)
            {
                string paramValue = parameters[i].Value;
                IArgument argument = StepData.Function.ParameterType[i];
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
        }

        private readonly List<PropertyInfo> _properties;

        private readonly List<object> _params;

        private string _instanceVar;

        protected override void InvokeStep(bool forceInvoke)
        {
            this.Result = StepResult.Error;
            object instance = null;
            if (StepData.Function.Type == FunctionType.InstancePropertySetter)
            {
                instance = Context.VariableMapper.GetParamValue(_instanceVar, StepData.Function.Instance,
                    StepData.Function.ClassType);
            }
            IParameterDataCollection parameters = StepData.Function.Parameters;
            IArgumentCollection arguments = StepData.Function.ParameterType;
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
            this.Result = StepResult.Pass;
        }
    }
}