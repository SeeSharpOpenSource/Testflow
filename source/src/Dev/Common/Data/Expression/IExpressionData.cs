using System.Collections.Generic;
using Testflow.Data.Sequence;
using Testflow.Usr;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式数据
    /// </summary>
    public interface IExpressionData : ICloneableClass<IExpressionData>
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
        IList<IExpressionElement> Arguments { get; set; }

        /// <summary>
        /// 表达式操作的名称
        /// </summary>
        string Operation { get; set; }

        /// <summary>
        /// 使用所属序列对象初始化
        /// </summary>
        void Initialize(ISequenceFlowContainer parent);
    }
}