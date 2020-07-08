using System;
using System.Collections.Generic;
using System.Linq;
using Testflow.CoreCommon;
using Testflow.CoreCommon.Common;
using Testflow.Data;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;
using Testflow.Usr;
using Testflow.Utility.Utils;

namespace Testflow.SlaveCore.Runner.Expression
{
    internal class ExpressionProcessor : IDisposable
    {
        // 表达式中Source的参数索引
        private const int SourceIndex = -1;
        // 表达式计算时的起始索引值
        private const int StartIndex = -2;
        private ExpressionParser _expParser;
        // 操作名称到计算对象的映射
        private readonly Dictionary<string, IList<ExpressionCalculator>> _calculators;

        private readonly int _coroutineId;

        private readonly SlaveContext _context;

        private readonly Stack<int> _expArgIndexStack;
        private readonly Stack<IExpressionData> _expElementStack;

        private List<ExpressionData> _expressions;

        public ExpressionProcessor(SlaveContext context, int coroutineId)
        {
            this._coroutineId = coroutineId;
            this._context = context;
            _expArgIndexStack = new Stack<int>(Constants.DefaultRuntimeSize);
            _expElementStack = new Stack<IExpressionData>(Constants.DefaultRuntimeSize);

            ExpressionOperatorInfo[] operatorInfos = context.ExpOperatorInfos;
            _expParser = new ExpressionParser(operatorInfos);

            _calculators = new Dictionary<string, IList<ExpressionCalculator>>(operatorInfos.Length);
            foreach (ExpressionOperatorInfo operatorInfo in operatorInfos)
            {
                _calculators.Add(operatorInfo.Name, new List<ExpressionCalculator>(5));
            }
            ExpressionCalculatorInfo[] calculatorInfos = context.ExpCalculatorInfos;
            // 初始化所有计算类实例
            foreach (ExpressionCalculatorInfo calculatorInfo in calculatorInfos)
            {
                _calculators[calculatorInfo.OperatorName].Add(new ExpressionCalculator(context, calculatorInfo));
            }

            _expressions = new List<ExpressionData>(200);
        }

        /// <summary>
        /// 解析表达式，并返回表达式的缓存ID
        /// </summary>
        public int CompileExpression(string expression, ISequenceStep step)
        {
            const int sourceIndex = -1;
            const int startIndex = -2;
            // TODO 每次计算表达式都会获取一次，此处待优化
            // TODO 可以在此处使用栈优化计算速度，但是因为一般表达式不会太复杂，所以暂时不在测试生成时处理，待后续再处理
            IExpressionData expressionData = _expParser.ParseExpression(expression, step);
            _expArgIndexStack.Clear();
            _expElementStack.Clear();
            _expArgIndexStack.Push(startIndex);
            _expElementStack.Push(expressionData);
            while (_expElementStack.Count > 0)
            {
                IExpressionData currentExpression = _expElementStack.Peek();
                int currentIndex = _expArgIndexStack.Pop() + 1;
                // 当前层遍历结束后直接进入下一个循环
                if (currentIndex >= expressionData.Arguments.Count)
                {
                    _expElementStack.Pop();
                    continue;
                }
                IExpressionElement expElement = GetExpElement(currentExpression, currentIndex);
                _expArgIndexStack.Push(currentIndex);
                switch (expElement.Type)
                {
                    case ParameterType.NotAvailable:
                        break;
                    case ParameterType.Value:
                        break;
                    case ParameterType.Variable:
                        string variableName = ModuleUtils.GetVariableNameFromParamValue(expElement.Value);
                        IVariable variable = SequenceUtils.GetVariable(variableName, step);
                        string varRunName = CoreUtils.GetRuntimeVariableName(_context.SessionId, variable);
                        expElement.Value = ModuleUtils.GetFullParameterVariableName(varRunName, expElement.Value);
                        break;
                    case ParameterType.Expression:
                        _expArgIndexStack.Push(-2);
                        _expElementStack.Push(expElement.Expression);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            int index = _expressions.Count;
            _expressions.Add((ExpressionData) expressionData);
            return index;
        }

        public void TrimExpressionCache()
        {
            _expressions.TrimExcess();
        }

        public object Calculate(int index, ITypeData targetType)
        {
            ExpressionData expression = _expressions[index];
            try
            {
                _expArgIndexStack.Push(StartIndex);
                _expElementStack.Push(expression);
                while (_expElementStack.Count > 0)
                {
                    ExpressionData currentExpression = (ExpressionData) _expElementStack.Peek();
                    int currentIndex = _expArgIndexStack.Pop() + 1;
                    int elementCount = currentExpression.Arguments?.Count ?? 0;
                    bool existNonCalculatedExp = false;
                    for (int i = currentIndex; i < elementCount; i++)
                    {
                        IExpressionElement element = GetExpElement(currentExpression, i);
                        // 如果元素类型为表达式，且该表达式未计算，则将该表达式推入栈中，执行计算
                        if (element.Type == ParameterType.Expression && !((ExpressionData) element.Expression).IsValueSet)
                        {
                            _expArgIndexStack.Push(i);
                            _expArgIndexStack.Push(StartIndex);
                            _expElementStack.Push(element.Expression);
                            existNonCalculatedExp = true;
                            break;
                        }
                    }
                    // 如果该表达式元素中存在未计算的表达式则停止当前表达式求值，进入下个循环中计算未求解表达式的值
                    if (existNonCalculatedExp)
                    {
                        continue;
                    }
                    // 当前层所有元素的值都可用时，直接计算
                    CalculateSingleExpression(currentExpression);
                    _expElementStack.Pop();
                }
                object targetValue = _context.Convertor.CastValue(targetType, expression.ExpressionValue);
                return targetValue;
            }
            catch (TestflowException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TestflowRuntimeException(ModuleErrorCode.ExpressionError,
                    _context.I18N.GetStr("ExpressionCalculateError"), ex);
            }
            finally
            {
                _expArgIndexStack.Clear();
                _expElementStack.Clear();
                // 清空expression中的所有Value
                expression.Reset();
            }
        }

        private void CalculateSingleExpression(ExpressionData expData)
        {
            IList<ExpressionCalculator> calculators = _calculators[expData.Operation];
            // 按照定义顺序依次检查表达式能否被正确计算，如果存在计算结束的情况则返回
            if (calculators.Any(calculator => calculator.TryCalculate(expData)))
            {
                return;
            }
            string expSignature = $"Operator:'{expData.Operation}', {GetExpressionSignature(expData)}";
            _context.LogSession.Print(LogLevel.Error, _context.SessionId,
                $"Unable to find available calculator for expression '{expSignature}'.");
            throw new TestflowRuntimeException(ModuleErrorCode.ExpressionError, "");
        }

        private string GetExpressionSignature(ExpressionData expression)
        {
            string souroceSig = GetElementSignature(expression.Source);
            string[] argumentsSig = new string[expression.Arguments?.Count + 1 ?? 0 + 1];
            argumentsSig[0] = souroceSig;
            for (int i = 1; i < argumentsSig.Length; i++)
            {
                argumentsSig[i] = GetElementSignature(expression.Arguments[i - 1]);
            }
            ExpressionOperatorInfo operatorInfo =
                _context.ExpOperatorInfos.First(item => item.Name.Equals(expression.Operation));
            return string.Format(operatorInfo.FormatString, argumentsSig);
        }

        private string GetElementSignature(IExpressionElement element)
        {
            switch (element.Type)
            {
                case ParameterType.NotAvailable:
                    return string.Empty;
                    break;
                case ParameterType.Value:
                    return element.Value;
                    break;
                case ParameterType.Variable:
                    return element.Value;
                    break;
                case ParameterType.Expression:
                    return GetExpressionSignature((ExpressionData) element.Expression);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IExpressionElement GetExpElement(IExpressionData expressionData, int index)
        {
            return index < 0 ? expressionData.Source : expressionData.Arguments[index];
        }

        public void Dispose()
        {
            this._expParser = null;
            _expressions.Clear();
            _expressions = null;
        }
    }
}