using System;
using System.Collections.Generic;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式描述信息集合
    /// </summary>
    [Serializable]
    public class ExpressionOperatorCollection : List<ExpressionOperatorInfo>
    {
        /// <summary>
        /// 构建空的ExpressionOperatorInfo集合
        /// </summary>
        public ExpressionOperatorCollection() : base()
        {
        }

        /// <summary>
        /// 构建空的ExpressionOperatorInfo集合
        /// </summary>
        public ExpressionOperatorCollection(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// 使用可遍历对象构建ExpressionOperatorInfo集合
        /// </summary>
        public ExpressionOperatorCollection(IEnumerable<ExpressionOperatorInfo> items) : base(items)
        {
        }
    }
}