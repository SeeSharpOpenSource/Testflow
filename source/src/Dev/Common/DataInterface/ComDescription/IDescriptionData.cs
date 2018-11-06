namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// Description对象的基类
    /// </summary>
    public interface IDescriptionData
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 参数的功能描述
        /// </summary>
        string Description { get; set; }
    }
}