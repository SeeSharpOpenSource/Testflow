using System;
using System.Xml.Serialization;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 参数类
    /// </summary>
    public interface IArgumentData
    {
        /// <summary>
        /// 参数类
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 参数类所在的程序集
        /// </summary>
        IAssemblyDescription Assembly { get; set; }

        /// <summary>
        /// 参数类的Type对象
        /// </summary>
        Type Type { get; set; }

        /// <summary>
        /// 参数类的类型
        /// </summary>
        VariableType VariableType { get; set; }
    }
}