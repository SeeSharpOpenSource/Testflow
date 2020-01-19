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
        public string Name { get; set; }

        /// <summary>
        /// 操作符描述信息
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 操作符名称
        /// </summary>
        [XmlElement]
        public string OperatorName { get; set; }

        /// <summary>
        /// 表达式计算类的类型信息
        /// </summary>
        [XmlElement]
        public ExpressionTypeData CalculatorClass { get; set; }

        /// <summary>
        /// 源数据类型
        /// </summary>
        [XmlElement]
        public ExpressionTypeData SourceType { get; set; }

        /// <summary>
        /// 参数数据类型
        /// </summary>
        [XmlArrayItem("ArgumentType")]
        [XmlArray("ArgumentTypes")]
        public List<ExpressionTypeData> ArgumentsType { get; set; }
    }
}