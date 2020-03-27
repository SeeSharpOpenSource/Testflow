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
        /// 根据AssemblyName获取对应的描述信息
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        IComInterfaceDescription GetComInterfaceByName(string assemblyName);

        /// <summary>
        /// 使用类型名获取TypeData，该类型必须是已经加载过的，否则返回null
        /// </summary>
        /// <param name="typename">类型名称</param>
        /// <param name="namespaceStr">命名空间</param>
        ITypeData GetTypeByName(string typename, string namespaceStr);

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
        /// 获取某个程序集的描述信息
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        IAssemblyInfo GetAssemblyInfo(string assemblyName);

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

        /// <summary>
        /// 获取枚举类型的子项信息
        /// </summary>
        /// <param name="typeData">类型描述信息</param>
        /// <returns></returns>
        string[] GetEnumItems(ITypeData typeData);

        /// <summary>
        /// 获取数据类型的接口描述
        /// </summary>
        /// <param name="typeData">类型描述信息</param>
        /// <param name="assemblyInfo">程序集描述信息</param>
        IClassInterfaceDescription GetClassDescriptionByType(ITypeData typeData, out IAssemblyInfo assemblyInfo);

        /// <summary>
        /// 使用类型名获取类接口，该类型可以通过在指定的Assembly中重新加载，如果path未配置且当前程序集未加载则返回null
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="namespaceStr">命名空间</param>
        /// <param name="typename">类型名称</param>
        /// <param name="path">程序集所在路径</param>
        IClassInterfaceDescription GetClassDescriptionByType(string assemblyName, string namespaceStr, string typename, string path = null);

        /// <summary>
        /// 获取所有接口描述信息的列表
        /// </summary>
        IList<IComInterfaceDescription> GetComponentDescriptions();

        /// <summary>
        /// 获取所有类型信息的列表
        /// </summary>
        IList<ITypeData> GetTypeDatas();

        /// <summary>
        /// 判断某个TypeData是不是baseType的衍生类型
        /// </summary>
        bool IsDerivedFrom(ITypeData typeData, ITypeData baseType);

        /// <summary>
        /// 判断某个TypeData是不是可以强制转换为某个类型
        /// </summary>
        /// <param name="sourceType">转换的源类型</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns></returns>
        bool IsCastable(ITypeData sourceType, ITypeData targetType);
    }
}