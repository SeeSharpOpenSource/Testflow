﻿using System;
using System.Xml.Serialization;
using Testflow.Data.Description;

namespace Testflow.Data.Sequence
{
    /// <summary>
    /// 变量数据
    /// </summary>
    public interface IVariable
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
        [XmlIgnore]
        string Value { get; set; }
    }
}