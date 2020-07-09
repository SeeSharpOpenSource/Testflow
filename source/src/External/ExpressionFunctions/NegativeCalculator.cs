using System;
using System.Collections;
using Testflow.Data.Expression;
using Testflow.Usr;

namespace Testflow.External.ExpressionCalculators
{
    public class NegativeCalculator : IExpressionCalculator
    {
        public NegativeCalculator()
        {
            this.Operator = "NegativeOperation";
        }

        public string Operator { get; }
        public bool IsCalculable(object sourceValue, params object[] arguments)
        {
            return true;
        }

        public object Calculate(object sourceValue, params object[] arguments)
        {
            return -1 * (double)sourceValue;
        }
    }
}
