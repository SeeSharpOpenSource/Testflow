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

        public override void GenerateInvokeInfo()
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

        public override void InitializeParamsValues()
        {
            if (!string.IsNullOrWhiteSpace(StepData.Function.Instance))
            {
                string instanceVarName = ModuleUtils.GetVariableNameFromParamValue(StepData.Function.Instance);
                _instanceVar = ModuleUtils.GetVariableFullName(instanceVarName, StepData, Context.SessionId);
            }
            for (int i = 0; i < _properties.Count; i++)
            {
                string paramValue = StepData.Function.Parameters[i].Value;
                IArgument argument = StepData.Function.ParameterType[i];
                if (null == _properties[i] || string.IsNullOrEmpty(paramValue))
                {
                    _params.Add(null);
                    continue;
                }
                switch (StepData.Function.Parameters[i].ParameterType)
                {
                    case ParameterType.NotAvailable:
                        _params.Add(null);
                        break;
                    case ParameterType.Value:
                        _params.Add(Context.TypeInvoker.CastValue(argument.Type,
                            paramValue));
                        break;
                    case ParameterType.Variable:
                        string variableRawName = ModuleUtils.GetVariableNameFromParamValue(paramValue);
                        _params.Add(ModuleUtils.GetVaraibleByRawVarName(variableRawName, StepData));
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
                instance = Context.VariableMapper.GetParamValue(_instanceVar, StepData.Function.Instance);
            }
            for (int i = 0; i < _properties.Count; i++)
            {
                if (null == _properties[i])
                {
                    continue;
                }
                if (StepData.Function.Parameters[i].ParameterType == ParameterType.Variable)
                {
                    // 获取变量值的名称
                    string variableName = CoreUtils.GetRuntimeVariableName(Context.SessionId, (IVariable) _params[i]);
                    _params[i] = Context.VariableMapper.GetParamValue(variableName, StepData.Function.Parameters[i].Value);
                }
                _properties[i].SetValue(instance, _params[i]);
            }
            this.Result = StepResult.Pass;
        }
    }
}