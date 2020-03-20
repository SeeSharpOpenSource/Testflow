using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Testflow.Data.Sequence;

namespace Testflow.Data
{
    /// <summary>
    /// 保存类型信息
    /// </summary>
    public interface ITypeData : ISequenceElement
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
        /// 判断该Type是否是某个类型的TypeData
        /// </summary>
        /// <param name="assemblyName">代比较的程序集名称</param>
        /// <param name="namespaceStr">待比较的命名空间名称</param>
        /// <param name="name">代比较的类名称</param>
        /// <returns></returns>
        bool Equals(string assemblyName, string namespaceStr, string name);
    }
}