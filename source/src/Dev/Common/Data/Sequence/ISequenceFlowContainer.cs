using Testflow.Common;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 序列流程容器的基类
    /// </summary>
    public interface ISequenceFlowContainer : ISequenceElement, ICloneableClass<ISequenceFlowContainer>
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 上级流程容器
        /// </summary>
        ISequenceFlowContainer Parent { get; set; }
    }
}