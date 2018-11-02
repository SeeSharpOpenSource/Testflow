using System;
using System.Collections.Generic;

namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// 实例对象描述接口
    /// </summary>
    public interface IInstanceDescription
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
        /// 所在命名空间
        /// </summary>
        string NameSpace { get; set; }

        /// <summary>
        /// 属于那种类型
        /// </summary>
        VariableType ArgumentType { get; set; }

        /// <summary>
        /// 对应的Type对象
        /// </summary>
        ITypeData Type { get; set; }

        /// <summary>
        /// 实例的所有属性
        /// </summary>
        IList<IPropertyDescription> Properties { get; set; }
    }
}