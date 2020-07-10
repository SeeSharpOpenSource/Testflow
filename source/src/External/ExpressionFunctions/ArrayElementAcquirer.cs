using System;
using System.Collections;
using Testflow.Data.Expression;
using Testflow.Usr;

namespace Testflow.External.ExpressionCalculators
{
    public class ArrayElementAcquirer : IExpressionCalculator
    {
        public ArrayElementAcquirer()
        {
            this.Operator = "GetElement";
        }

        public string Operator { get; }
        public bool IsCalculable(object sourceValue, params object[] arguments)
        {
            return sourceValue is Array && ((Array)sourceValue).Rank == 1;
        }

        public object Calculate(object sourceValue, params object[] arguments)
        {
            return ((Array) sourceValue).GetValue((int)arguments[0]);
        }
    }
}
