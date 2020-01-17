using Testflow.Data.Sequence;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式数据
    /// </summary>
    public interface IExpressionData : ISequenceDataContainer
    {
        /// <summary>
        /// 表达式的名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 表达式的上级节点
        /// </summary>
        ISequence Parent { get; set; }

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
    }
}