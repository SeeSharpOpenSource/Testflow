using System;
using System.Xml.Serialization;
using Testflow.Common;
using Testflow.Data.Description;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 参数数据类
    /// </summary>
    public interface IParameterData : ICloneableClass<IParameterData>
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
    }
}