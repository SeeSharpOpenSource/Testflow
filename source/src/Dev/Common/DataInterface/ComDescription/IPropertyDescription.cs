using System;

namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// 属性描述接口
    /// </summary>
    public interface IPropertyDescription
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 描述信息，如果没有则为string.Empty
        /// </summary>
        string Description { get; set; }

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