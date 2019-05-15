using System.Collections.Generic;
using System.Xml.Serialization;

namespace Testflow.Data.Description
{
    /// <summary>
    /// 组件接口描述类，保存某个组件接口的描述信息。该类由ComInterfaceLoader生成。
    /// </summary>
    public interface IComInterfaceDescription : IDescriptionData
    {
        /// <summary>
        /// 组件在当前会话中的ID
        /// </summary>
        int ComponentId { get; set; }

        /// <summary>
        /// 组件在Testflow中的签名
        /// </summary>
        string Signature { get; }

        /// <summary>
        /// 配置集信息
        /// </summary>
        IAssemblyInfo Assembly { get; set; }

        /// <summary>
        /// 当前程序集中包含的所有类列表
        /// </summary>
        IList<IClassInterfaceDescription> Functions { get; }

        /// <summary>
        /// 该程序集支持对外暴露的变量类型
        /// </summary>
        IList<ITypeData> VariableTypes { get; set; }

        /// <summary>
        /// 库的类型
        /// </summary>
        string LibraryType { get; set; }
    }
}
