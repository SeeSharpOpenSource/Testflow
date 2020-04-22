namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 操作员面板信息
    /// </summary>
    public interface IOperationPanelInfo : ISequenceElement
    {
        /// <summary>
        /// 操作员面板所在程序集
        /// </summary>
        IAssemblyInfo Assembly { get; set; }

        /// <summary>
        /// 操作员面板所在类
        /// </summary>
        ITypeData OperationPanelClass { get; set; }

        /// <summary>
        /// 操作员面板参数
        /// </summary>
        string Parameters { get; set; }
    }
}