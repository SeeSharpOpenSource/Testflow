using System;
using System.Collections;
using Testflow.Data.Expression;
using Testflow.Usr;

namespace Testflow.External.ExpressionCalculators
{
    public class ComplementationCalculator : IExpressionCalculator
    {
        public ComplementationCalculator()
        {
            this.Operator = "Complementation";
        }

        public string Operator { get; }
        public bool IsCalculable(object sourceValue, params object[] arguments)
        {
            return sourceValue is long && arguments[0] is long;
        }

        public object Calculate(object sourceValue, params object[] arguments)
        {
            return (long) sourceValue % (long) arguments[0];
        }
    }
}
