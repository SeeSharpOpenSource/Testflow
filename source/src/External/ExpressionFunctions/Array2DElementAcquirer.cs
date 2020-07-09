using System;
using System.Collections;
using Testflow.Data.Expression;

namespace Testflow.External.ExpressionCalculators
{
    public class Array2DElementAcquirer : IExpressionCalculator
    {
        public string Operator { get; }

        public Array2DElementAcquirer()
        {
            this.Operator = "Get2DArrayElement";
        }

        public bool IsCalculable(object sourceValue, params object[] arguments)
        {
            Array sourceArray = sourceValue as Array;
            return sourceArray?.Rank == 2;
        }

        public object Calculate(object sourceValue, params object[] arguments)
        {
            Array sourceArray = sourceValue as Array;
            return sourceArray.GetValue((int) arguments[0], (int) arguments[1]);
        }
    }
}