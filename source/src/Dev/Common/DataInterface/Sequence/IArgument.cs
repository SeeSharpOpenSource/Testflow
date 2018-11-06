using System;
using System.Xml.Serialization;
using Testflow.DataInterface.ComDescription;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 参数类，描述FunctionData的接口
    /// </summary>
    public interface IArgument
    {
        /// <summary>
        /// 参数类
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 参数类的Type对象
        /// </summary>
        ITypeData Type { get; set; }

        /// <summary>
        /// 参数类的类型
        /// </summary>
        [XmlIgnore]
        VariableType VariableType { get; set; }
    }
}