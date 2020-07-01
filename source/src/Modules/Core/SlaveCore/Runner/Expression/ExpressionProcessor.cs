using System;
using System.Collections.Generic;
using Testflow.Data.Expression;
using Testflow.Data.Sequence;
using Testflow.SlaveCore.Common;
using Testflow.Utility.Utils;

namespace Testflow.SlaveCore.Runner.Expression
{
    internal class ExpressionProcessor : IDisposable
    {
        private ExpressionParser _expParser;
        // 操作名称到计算对象的映射
        private readonly Dictionary<string, IList<IExpressionCalculator>> _calculators;

        private readonly int _coroutineId;

        private readonly SlaveContext _context;

        public ExpressionProcessor(SlaveContext context, int coroutineId)
        {
            this._coroutineId = coroutineId;
            this._context = context;

            ExpressionOperatorInfo[] operatorInfos = context.ExpOperatorInfos;
            _expParser = new ExpressionParser(operatorInfos);

            _calculators = new Dictionary<string, IList<IExpressionCalculator>>(operatorInfos.Length);
            foreach (ExpressionOperatorInfo operatorInfo in operatorInfos)
            {
                _calculators.Add(operatorInfo.Name, new List<IExpressionCalculator>(5));
            }
            ExpressionCalculatorInfo[] calculatorInfos = context.ExpCalculatorInfos;
            // 初始化所有计算类实例
            foreach (ExpressionCalculatorInfo calculatorInfo in calculatorInfos)
            {
                _calculators[calculatorInfo.OperatorName].Add(context.TypeInvoker.GetCalculatorInstance(calculatorInfo));
            }
        }

        public IExpressionData CompileExpression(string expression, ISequenceStep step)
        {
            // TODO 每次计算表达式都会获取一次，此处待优化
            // TODO 可以在此处使用栈优化计算速度，但是因为一般表达式不会太复杂，所以暂时不在测试生成时处理，待后续再处理
            return _expParser.ParseExpression(expression, step);
        }

        public void CalculateExpression()
        {

        }

        public void Dispose()
        {
            this._expParser = null;
        }
    }
}