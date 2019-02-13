using System;

namespace Testflow.Data.Description
{
    /// <summary>
    /// 属性描述接口
    /// </summary>
    public interface IPropertyDescription : IDescriptionData
    {
        /// <summary>
        /// 属于那种类型
        /// </summary>
        VariableType ArgumentType { get; set; }

        /// <summary>
        /// 对应的Type对象
        /// </summary>
        ITypeData Type { get; set; }

        /// <summary>
        /// 参数默认值
        /// </summary>
        string DefaultValue { get; set; }
    }
}