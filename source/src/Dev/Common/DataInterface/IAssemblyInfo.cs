using System.Xml.Serialization;

namespace Testflow.DataInterface
{
    /// <summary>
    /// 保存单个Assembly的描述信息
    /// </summary>
    public interface IAssemblyInfo
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
        [XmlIgnore]
        bool Available { get; }

//        /// <summary>
//        /// 使用到的程序集的所有命名空间
//        /// </summary> 
//        [XmlIgnore]
//        ISet<string> NameSpaces { get; set; }

        /// <summary>
        /// 程序集的版本号
        /// </summary>
        string Version { get; set; }
    }
}