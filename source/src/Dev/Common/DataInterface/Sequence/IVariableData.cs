using System;
using System.Xml.Serialization;
using Testflow.DataInterface.ComDescription;

namespace Testflow.DataInterface.Sequence
{
    /// <summary>
    /// 变量数据
    /// </summary>
    public interface IVariableData
    {
        /// <summary>
        /// 变量名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 变量的Type对象
        /// </summary>
        ITypeData Type { get; set; }

        /// <summary>
        /// 变量的类型
        /// </summary>
        VariableType VariableType { get; set; }
        
        /// <summary>
        /// 变量的值，如果没有则为null
        /// </summary>
        string Value { get; set; }
    }
}