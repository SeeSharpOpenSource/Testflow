namespace Testflow.Data.Sequence
{
    public interface IVariableValue
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