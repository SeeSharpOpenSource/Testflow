using System.Collections.Generic;

namespace Testflow.DataInterface.ComDescription
{
    /// <summary>
    /// 保存单个Assembly的描述信息
    /// </summary>
    public interface IAssemblyDescription
    {
        /// <summary>
        /// 程序集名称
        /// </summary>
        string AssemblyName { get; set; }

        /// <summary>
        /// 程序集所在目录
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// 程序集是否可用
        /// </summary>
        bool Available { get; }

        /// <summary>
        /// 使用到的程序集的所有命名空间
        /// </summary> 
        ISet<string> NameSpaces { get; set; }

        /// <summary>
        /// 程序集的版本号
        /// </summary>
        string Version { get; set; }
    }
}