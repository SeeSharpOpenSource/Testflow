namespace Testflow.DataInterface.ComDescription
{
    public interface IArgumentDescription
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 参数的功能描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 参数属于那种类型
        /// </summary>
        VariableType ArgumentType { get; set; }

        /// <summary>
        /// 参数的Type对象
        /// </summary>
        ITypeData Type { get; set; }

        /// <summary>
        /// 参数默认值
        /// </summary>
        string DefaultValue { get; set; }
    }
}