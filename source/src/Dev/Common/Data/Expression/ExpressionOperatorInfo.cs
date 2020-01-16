namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式操作符配置项
    /// </summary>
    public class ExpressionOperatorInfo
    {
        /// <summary>
        /// 操作符名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 操作符描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 操作符样式
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 运算符计算所在的程序集
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// 运算符所在的命名空间
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 运算法的计算类，该类必须继承自Testflow.Usr.Expression.IExpressionFunction
        /// </summary>
        public string CalculateClass { get; set; }
    }
}