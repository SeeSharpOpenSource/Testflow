using Testflow.Data.Sequence;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式数据
    /// </summary>
    public interface IExpression : ISequenceDataContainer
    {
        /// <summary>
        /// 表达式的名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 表达式中的源
        /// </summary>
        IExpressionElement Source { get; set; }

        /// <summary>
        /// 表达式中的目标
        /// </summary>
        IExpressionElement Target { get; set; }

        /// <summary>
        /// 表达式的名称
        /// </summary>
        string Operation { get; set; }

        /// <summary>
        /// 返回当前表达式是否是可以计算的
        /// </summary>
        bool IsCalculable();
    }
}