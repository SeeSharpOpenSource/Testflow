using System;
using System.Xml.Serialization;
using Testflow.DataInterface.ComDescription;

namespace Testflow.DataInterface.Sequence
{
    public interface IParameterData
    {
        /// <summary>
        /// 当前ParameterData在ParametorCollection中的索引
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 参数的值，如果是变量，则值为IVariableData
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// 当前的值是变量还是值
        /// </summary>
        ParameterType ParameterType { get; set; }

        /// <summary>
        /// 该参数的Type类型
        /// </summary>
        ITypeData Type { get; set; }
    }
}