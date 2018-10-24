using System;

namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// 属性描述接口
    /// </summary>
    public interface IPropertyDescription : IInterfaceDescription
    {
        /// <summary>
        /// 参数默认值
        /// </summary>
        object DefaultValue { get; set; }
    }
}