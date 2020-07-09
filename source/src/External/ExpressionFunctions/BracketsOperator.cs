using System;
using System.Collections;
using Testflow.Data.Expression;
using Testflow.Usr;

namespace Testflow.External.ExpressionCalculators
{
    public class BracketsOperator : IExpressionCalculator
    {
        public BracketsOperator()
        {
            this.Operator = "Brackets";
        }

        public string Operator { get; }
        public bool IsCalculable(object sourceValue, params object[] arguments)
        {
            return true;
        }

        public object Calculate(object sourceValue, params object[] arguments)
        {
            return sourceValue;
        }
    }
}
