using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式计算类信息
    /// </summary>
    [Serializable]
    public class ExpressionCalculatorInfo
    {
        /// <summary>
        /// 计算类名称
        /// </summary>
        [XmlAttribute]
        string Name { get; }

        /// <summary>
        /// 操作符名称
        /// </summary>
        [XmlElement(Order = 1)]
        string OperatorName { get; }

        /// <summary>
        /// 操作符描述信息
        /// </summary>
        [XmlElement(Order = 2)]
        string Description { get; }

        /// <summary>
        /// 表达式计算类的类型信息
        /// </summary>
        [XmlElement(Order = 3)]
        ExpressionTypeData CalculatorClass { get; set; }

        /// <summary>
        /// 源数据类型
        /// </summary>
        [XmlElement(Order = 4)]
        List<ExpressionTypeData> SourceType { get; set; }

        /// <summary>
        /// 参数数据类型
        /// </summary>
        [XmlElement(Order = 5)]
        List<ExpressionTypeData> ArgumentsType { get; set; }
    }
}