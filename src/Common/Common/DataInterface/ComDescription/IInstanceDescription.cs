using System;
using System.Collections.Generic;

namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// 实例对象描述接口
    /// </summary>
    public interface IInstanceDescription : IInterfaceDescription
    {

        /// <summary>
        /// 实例的所有属性
        /// </summary>
        IList<IPropertyDescription> Properties { get; set; }
    }
}