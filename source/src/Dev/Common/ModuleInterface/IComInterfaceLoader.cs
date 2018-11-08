using System.Collections.Generic;
using Testflow.DataInterface;
using Testflow.DataInterface.ComDescription;

namespace Testflow.ModuleInterface
{
    /// <summary>
    /// 组件接口加载模块
    /// </summary>
    public interface IComInterfaceLoader : IController
    {
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
        IComInterfaceDescription GetComponentInterfaces(IList<string> paths);

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

    }
}