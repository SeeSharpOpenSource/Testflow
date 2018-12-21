using System.Collections.Generic;
using Testflow.Data;
using Testflow.Data.Description;

namespace Testflow.Modules
{
    /// <summary>
    /// 组件接口加载模块
    /// </summary>
    public interface IComInterfaceManager : IController
    {
        /// <summary>
        /// 根据组件的索引号获取对应的接口描述信息
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        IComInterfaceDescription GetComInterfaceById(int componentId);

        /// <summary>
        /// 从指定路径加载某个组件的接口描述信息
        /// </summary>
        /// <param name="path">组件的路径</param>
        /// <returns>该组件的接口描述信息</returns>
        IComInterfaceDescription GetComponentInterface(string path);

        /// <summary>
        /// 加载某个程序配置集对应组件的接口描述信息
        /// </summary>
        /// <param name="assemblyInfo">程序集描述信息</param>
        /// <returns>该组件的接口描述信息</returns>
        IComInterfaceDescription GetComponentInterface(IAssemblyInfo assemblyInfo);

        /// <summary>
        /// 从指定路径加载多个个组件的接口描述信息
        /// </summary>
        /// <param name="paths">组件的路径集合</param>
        /// <returns>组件的接口描述信息</returns>
        IList<IComInterfaceDescription> GetComponentInterfaces(IList<string> paths);

        /// <summary>
        /// 获取AssemblyInfo集合对应的组件接口描述信息
        /// </summary>
        /// <param name="assemblyInfos"></param>
        /// <returns>组件的接口描述信息</returns>
        IList<IComInterfaceDescription> GetComponentInterfaces(IAssemblyInfoCollection assemblyInfos);

        /// <summary>
        /// 获取某个目录下所有组件的接口描述信息
        /// </summary>
        /// <param name="directory">目录的路径</param>
        /// <returns>组件的接口描述信息</returns>
        IList<IComInterfaceDescription> GetComponentInterfaces(string directory);

        /// <summary>
        /// 获取某个变量属性的类型信息
        /// </summary>
        /// <param name="variableType">变量的variableType</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        ITypeData GetPropertyType(ITypeData variableType, string propertyName);

        /// <summary>
        /// 获取某个类型的所有属性，可以使用propertyType进行过滤
        /// </summary>
        /// <param name="type">类型信息</param>
        /// <param name="propertyType">过滤的Type全名，包括命名空间。如果不指定可以配置为null</param>
        /// <returns></returns>
        IList<string> GetTypeProperties(ITypeData type, ITypeData propertyType = null);
    }
}