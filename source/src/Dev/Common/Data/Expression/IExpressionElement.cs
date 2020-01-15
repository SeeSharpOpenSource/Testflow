using Testflow.Data.Sequence;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式元素
    /// </summary>
    public interface IExpressionElement : ISequenceDataContainer
    {
        /// <summary>
        /// 表达式元素的类型
        /// </summary>
        ParameterType Type { get; set; }

        /// <summary>
        /// 表达式元素的值，仅在Type为Value或Variable时生效
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// 表达式元素的值，仅在Type为Expression时生效
        /// </summary>
        IExpression Expression { get; set; }

        /// <summary>
        /// 返回当前表达式元素是否可操作
        /// </summary>
        bool IsOperational(ISequenceFlowContainer parent);
    }
}