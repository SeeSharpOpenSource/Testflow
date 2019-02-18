namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 变量的初始值
    /// </summary>
    public interface IVariableInitValue : ISequenceDataContainer
    {
         /// <summary>
         /// 变量名称
         /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 变量的值
        /// </summary>
        string Value { get; set; }
    }
}