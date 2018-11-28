using System;
using System.Collections.Generic;

namespace Testflow.Data.Description
{
    /// <summary>
    /// 实例对象描述接口
    /// </summary>
    public interface IInstanceDescription : IDescriptionData
    {
        /// <summary>
        /// 所在命名空间
        /// </summary>
        string NameSpace { get; set; }

        /// <summary>
        /// 属于那种类型
        /// </summary>
        VariableType ArgumentType { get; set; }

        /// <summary>
        /// 对应的Type对象的索引号
        /// </summary>
        int TypeIndex { get; set; }

        /// <summary>
        /// 实例的所有属性
        /// </summary>
        IList<IPropertyDescription> Properties { get; set; }
    }
}