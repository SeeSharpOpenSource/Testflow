using System;
using System.Collections.Generic;

namespace Testflow.Common
{
    /// <summary>
    /// 属性可扩展组件的接口
    /// </summary>
    public interface IPropertyExtendable
    {
        /// <summary>
        /// 初始化可扩展属性
        /// </summary>
        void InitExtendProperties();

        /// <summary>
        /// 扩展属性
        /// </summary>
        ISerializableMap<string, object> Properties { get; }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">属性值</param>
        void SetProperty(string propertyName, object value);

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性的值</returns>
        object GetProperty(string propertyName);

        /// <summary>
        /// 获取属性的类型
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性的Type对象</returns>
        Type GetPropertyType(string propertyName);

        /// <summary>
        /// 是否包含某个属性
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns>是否包含属性</returns>
        bool ContainsProperty(string propertyName);

        /// <summary>
        /// 返回所有扩展属性的名称
        /// </summary>
        /// <returns>所有扩展属性名称的集合</returns>
        IList<string> GetPropertyNames();
    }
}