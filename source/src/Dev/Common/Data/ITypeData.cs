using System;
using System.Xml.Serialization;

namespace Testflow.Data
{
    /// <summary>
    /// 保存类型信息
    /// </summary>
    public interface ITypeData
    {
//        /// <summary>
//        /// 类型数据在当前容器中的索引
//        /// </summary>
//        int Index { get; set; }

        /// <summary>
        /// 类型所在配置集名称
        /// </summary>
        string AssemblyName { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        string Namespace { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 获取该类型的完整名称
        /// </summary>
        [XmlIgnore]
        string GetFullName { get; }

        /// <summary>
        /// 判断两个类型是否相等
        /// </summary>
        /// <param name="fullName">类型的完整名称</param>
        /// <returns></returns>
        bool Eqauls(string fullName);

//        /// <summary>
//        /// 类型的Type对象
//        /// </summary>
//        [XmlIgnore]
//        Type Type { get; set; }
    }
}