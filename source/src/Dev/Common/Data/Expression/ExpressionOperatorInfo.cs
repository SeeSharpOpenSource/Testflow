using System;
using System.Xml.Serialization;

namespace Testflow.Data.Expression
{
    /// <summary>
    /// 表达式操作符配置项
    /// </summary>
    public class ExpressionOperatorInfo
    {
        /// <summary>
        /// 操作符名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 操作符描述信息
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// 操作符样式
        /// </summary>
        [XmlElement(Order = 1)]
        public string Symbol { get; set; }

        /// <summary>
        /// 操作符格式化字符串，0对应Source，1对应Target
        /// </summary>
        [XmlElement(Order = 2)]
        public string FormatString { get; set; }

        /// <summary>
        /// 运算的优先级，数值越大优先级越高
        /// </summary>
        [XmlElement(Order = 3)]
        public int Priority { get; }

        /// <summary>
        /// 参数的个数
        /// </summary>
        [XmlElement(Order = 4)]
        public int ArgumentsCount { get; }
    }
}