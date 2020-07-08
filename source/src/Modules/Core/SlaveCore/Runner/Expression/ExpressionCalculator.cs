using System;
using Testflow.CoreCommon;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;
using Testflow.Usr;

namespace Testflow.SlaveCore.Runner.Expression
{
    internal class ExpressionCalculator : IDisposable
    {
        private readonly IExpressionCalculator _calculator;
        private readonly ExpressionCalculatorInfo _calculatorInfo;
        private readonly SlaveContext _context;

        private Type[] _argumentType;
        private Type _sourceType;

        public ExpressionCalculator(SlaveContext context, ExpressionCalculatorInfo calculatorInfo)
        {
            this._calculator = context.TypeInvoker.GetCalculatorInstance(calculatorInfo);
            this._calculatorInfo = calculatorInfo;
            this._context = context;

            _sourceType = _context.TypeInvoker.GetExpArgumentType(calculatorInfo.SourceType);
            _argumentType = new Type[calculatorInfo?.ArgumentsType?.Count ?? 0];
            for (int i = 0; i < _argumentType.Length; i++)
            {
                _argumentType[i] = context.TypeInvoker.GetExpArgumentType(calculatorInfo.ArgumentsType[i]);
            }
        }

        public bool TryCalculate(ExpressionData expression)
        {
            object sourceValue;
            if (!TryGetElementValue(expression.Source, _sourceType, out sourceValue))
            {
                return false;
            }
            object[] argumentValues = new object[_argumentType.Length];
            object argumentValue;
            for (int i = 0; i < _argumentType.Length; i++)
            {
                if (!TryGetElementValue(expression.Arguments[i], _argumentType[i], out argumentValue))
                {
                    return false;
                }
                argumentValues[i] = argumentValue;
            }
            if (!_calculator.IsCalculable(sourceValue, argumentValues))
            {
                return false;
            }
            expression.ExpressionValue = _calculator.Calculate(sourceValue, argumentValues);
            return true;
        }

        private bool TryGetElementValue(IExpressionElement element, Type targetType, out object elementValue)
        {
            object originalValue = null;
            elementValue = null;
            switch (element.Type)
            {
                case ParameterType.Value:
                    originalValue = element.Value;
                    break;
                case ParameterType.Variable:
                    string variableName = ModuleUtils.GetVariableNameFromParamValue(element.Value);
                    originalValue = _context.VariableMapper.GetParamValue(variableName, element.Value);
                    break;
                default:
                    throw new TestflowRuntimeException(ModuleErrorCode.ExpressionError, "Invalid expression.");
            }
            if (null == originalValue)
            {
                _context.LogSession.Print(LogLevel.Error, _context.SessionId, $"The value of '{element.Value}' is null.");
                return false;
            }
            Type valueType = originalValue.GetType();
            if (valueType == targetType || valueType.IsSubclassOf(targetType))
            {
                elementValue = originalValue;
                return true;
            }
            if (!_context.Convertor.IsValidValueCast(valueType, targetType))
            {
                return false;
            }
            elementValue = _context.Convertor.CastValue(targetType, originalValue);
            return true;
        }

        public void Dispose()
        {
            this._argumentType = null;
            this._sourceType = null;
        }
    }
}