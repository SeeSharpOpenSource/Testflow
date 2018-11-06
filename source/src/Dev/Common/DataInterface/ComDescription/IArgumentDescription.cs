namespace Testflow.DataInterface.ComDescription
{
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
    }
}