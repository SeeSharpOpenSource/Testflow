namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 序列流程容器的基类
    /// </summary>
    public interface ISequenceFlowContainer : ISequenceElement
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        string Description { get; set; }
    }
}