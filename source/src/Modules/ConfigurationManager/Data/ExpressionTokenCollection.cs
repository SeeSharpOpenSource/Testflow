using System;
using System.Collections.Generic;
using Testflow.Data.Expression;

namespace Testflow.ConfigurationManager.Data
{
    /// <summary>
    /// 表达式描述信息集合
    /// </summary>
    [Serializable]
    public class ExpressionTokenCollection : List<ExpressionOperatorInfo>
    {
        /// <summary>
        /// 构建空的ExpressionOperatorInfo集合
        /// </summary>
        public ExpressionTokenCollection() : base()
        {
        }

        /// <summary>
        /// 构建空的ExpressionOperatorInfo集合
        /// </summary>
        public ExpressionTokenCollection(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// 使用可遍历对象构建ExpressionOperatorInfo集合
        /// </summary>
        public ExpressionTokenCollection(IEnumerable<ExpressionOperatorInfo> items) : base(items)
        {
        }
    }
}