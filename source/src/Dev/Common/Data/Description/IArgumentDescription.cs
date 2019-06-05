namespace Testflow.Data.Description
{
    /// <summary>
    /// 参数描述接口
    /// </summary>
    public interface IArgumentDescription : IDescriptionData
    {
        /// <summary>
        /// 参数属于那种类型
        /// </summary>
        VariableType ArgumentType { get; set; }

        /// <summary>
        /// 参数的Type对象
        /// </summary>
        ITypeData Type { get; set; }

        /// <summary>
        /// 参数的修饰符
        /// </summary>
        ArgumentModifier Modifier { get; set; }

        /// <summary>
        /// 参数默认值
        /// </summary>
        string DefaultValue { get; set; }

        /// <summary>
        /// 是否为可选参数
        /// </summary>
        bool IsOptional { get; set; }
    }
}