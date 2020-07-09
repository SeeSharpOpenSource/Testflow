using System;
using System.Collections;
using Testflow.Data.Expression;
using Testflow.Usr;

namespace Testflow.External.ExpressionCalculators
{
    public class ListElementAcquirer : IExpressionCalculator
    {
        public ListElementAcquirer()
        {
            this.Operator = "GetElement";
        }

        public string Operator { get; }
        public bool IsCalculable(object sourceValue, params object[] arguments)
        {
            return sourceValue is IList;
        }

        public object Calculate(object sourceValue, params object[] arguments)
        {
            return ((IList) sourceValue)[(int) arguments[0]];
        }
    }
}
